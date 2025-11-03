Imports System
Imports System.Diagnostics
Imports System.Net.Http
Imports System.Windows.Forms
Imports Newtonsoft.Json.Linq
Imports System.Drawing
Imports MaterialSkin
Imports MaterialSkin.Controls
Imports System.IO

Public Class UpdateHelper
    Private Shared ReadOnly currentVersion As Version = New Version("1.4.4")

    Private Const RepoOwner As String = "MrTysonsHacks"
    Private Const RepoName As String = "P2P-Monitor"

    Private Shared suppressedTag As String = Nothing

    Private Shared ReadOnly TagsApiUrl As String =
        $"https://api.github.com/repos/{RepoOwner}/{RepoName}/tags"

    Private Shared ReadOnly ReleasesLatestUrl As String =
        $"https://github.com/{RepoOwner}/{RepoName}/releases/latest"

    Private Shared ReadOnly ReleasesLatestApi As String =
        $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest"

    Public Shared Async Function CheckForUpdates(logAction As Action(Of String)) As Task
        If logAction Is Nothing Then logAction = Sub(_msg) Return
        Try
            logAction("Checking for updates.")
            Dim tv = Await GetLatestTagVersionAsync()
            If tv Is Nothing Then
                logAction("⚠ No tags found on GitHub.")
                Return
            End If

            If tv.Version > currentVersion Then
                logAction($"Update available: {tv.Tag} (you have v{currentVersion})")
            Else
                logAction($"✅ You are up to date (v{currentVersion}).")
            End If
        Catch ex As Exception
            logAction("❌ Update check failed: " & ex.Message)
        End Try
    End Function

    Public Shared Async Function CheckForUpdatesAndPrompt(owner As Form,
                                                          logAction As Action(Of String),
                                                          Optional forcePrompt As Boolean = False) As Task
        Await CheckForUpdatesAndPrompt(owner, logAction, Function() False, forcePrompt)
    End Function

    Public Shared Async Function CheckForUpdatesAndPrompt(owner As Form,
                                                          logAction As Action(Of String),
                                                          autoUpdateEnabled As Func(Of Boolean),
                                                          Optional forcePrompt As Boolean = False) As Task
        If logAction Is Nothing Then logAction = Sub(_msg) Return
        Try
            logAction("Checking for updates...")
            Dim tv = Await GetLatestTagVersionAsync()
            If tv Is Nothing Then
                logAction("⚠ No tags found on GitHub.")
                Return
            End If

            If tv.Version > currentVersion Then
                If Not forcePrompt AndAlso Not String.IsNullOrWhiteSpace(suppressedTag) AndAlso
                   String.Equals(suppressedTag, tv.Tag, StringComparison.OrdinalIgnoreCase) Then
                    logAction("Update previously declined this session; skipping prompt.")
                    Return
                End If

                logAction($"Update available: {tv.Tag} (you have v{currentVersion})")

                Dim title = "Update Available"
                Dim msg = $"Update available {tv.Tag} (you have v{currentVersion})." & Environment.NewLine &
                          "Open the latest release page?"

                Dim choice As DialogResult = ShowMaterialYesNo(owner, title, msg)

                If choice = DialogResult.Yes Then
                    Dim doAuto As Boolean = False
                    Try
                        If autoUpdateEnabled IsNot Nothing Then doAuto = autoUpdateEnabled()
                    Catch
                    End Try

                    If doAuto Then
                        Await PerformAutoUpdateAsync(owner, logAction)
                    Else
                        Try
                            Dim psi As New ProcessStartInfo(ReleasesLatestUrl) With {.UseShellExecute = True}
                            Process.Start(psi)
                            logAction("Opening latest release page...")
                        Catch ex As Exception
                            logAction("❌ Failed to open browser: " & ex.Message)
                        End Try
                    End If
                Else
                    suppressedTag = tv.Tag
                    logAction("Update skipped by user (suppressed for this session).")
                End If
            Else
                logAction($"✅ You are up to date (v{currentVersion}).")
            End If
        Catch ex As Exception
            logAction("❌ Update check failed: " & ex.Message)
        End Try
    End Function

    Private Class ReleaseInfo
        Public Property Tag As String
        Public Property ExeUrl As String
        Public Property AssetName As String
    End Class

    Private Shared Async Function PerformAutoUpdateAsync(owner As Form, log As Action(Of String)) As Task
        Try
            log("⬇️ Preparing auto-update...")

            Dim rel = Await GetLatestReleaseExecutableAsync()
            If rel Is Nothing OrElse String.IsNullOrWhiteSpace(rel.ExeUrl) Then
                log("⚠ Could not find a .exe asset on the latest release; opening releases page instead.")
                Try
                    Dim psi As New ProcessStartInfo(ReleasesLatestUrl) With {.UseShellExecute = True}
                    Process.Start(psi)
                Catch
                End Try
                Return
            End If

            Dim tempRoot = Path.Combine(Path.GetTempPath(), "P2PMonitorUpdate_" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tempRoot)

            Dim downloadedExe = Path.Combine(tempRoot, If(String.IsNullOrWhiteSpace(rel.AssetName), "update.exe", rel.AssetName))

            log($"⬇️ Downloading: {Path.GetFileName(downloadedExe)}")
            Using http As New HttpClient()
                http.Timeout = TimeSpan.FromMinutes(5)
                http.DefaultRequestHeaders.UserAgent.ParseAdd("P2P-Monitor-Updater")
                Using s = Await http.GetStreamAsync(rel.ExeUrl)
                    Using fs = File.Create(downloadedExe)
                        Await s.CopyToAsync(fs)
                    End Using
                End Using
            End Using
            Dim currentExe As String = Application.ExecutablePath
            Dim updaterBat As String = Path.Combine(tempRoot, "apply_update.bat")
            Dim bat As String =
"@echo off
setlocal enableextensions
set DST_EXE=%~1
set NEW_EXE=%~2

REM Give the app a moment to exit
ping 127.0.0.1 -n 2 >nul

REM Try until we can delete the old exe (means process fully exited)
:wait_loop
del ""%DST_EXE%"" >nul 2>&1
if exist ""%DST_EXE%"" (
  ping 127.0.0.1 -n 2 >nul
  goto wait_loop
)

REM Copy the new exe into place
copy ""%NEW_EXE%"" ""%DST_EXE%"" /Y >nul

REM Relaunch
start """" ""%DST_EXE%""

endlocal
"

            File.WriteAllText(updaterBat, bat)
            log("🔁 Replacing executable and restarting into the new version...")
            Dim psi2 As New ProcessStartInfo("cmd.exe", $"/C """"{updaterBat}"" ""{currentExe}"" ""{downloadedExe}""""") With {
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .WorkingDirectory = tempRoot
            }
            Process.Start(psi2)
            Try
                If owner IsNot Nothing AndAlso Not owner.IsDisposed Then
                    owner.BeginInvoke(Sub() Application.Exit())
                Else
                    Application.Exit()
                End If
            Catch
                Application.Exit()
            End Try
        Catch ex As Exception
            log("❌ Auto-update failed: " & ex.Message)
        End Try
    End Function

    Private Shared Async Function GetLatestReleaseExecutableAsync() As Task(Of ReleaseInfo)
        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromSeconds(15)
            client.DefaultRequestHeaders.UserAgent.ParseAdd("P2P-Monitor")
            Dim resp = Await client.GetAsync(ReleasesLatestApi)
            resp.EnsureSuccessStatusCode()
            Dim json = Await resp.Content.ReadAsStringAsync()
            Dim o = JObject.Parse(json)
            Dim tag As String = o.Value(Of String)("tag_name")
            Dim assets = TryCast(o("assets"), JArray)
            If assets IsNot Nothing Then
                For Each a In assets
                    Dim name As String = a.Value(Of String)("name")
                    Dim url As String = a.Value(Of String)("browser_download_url")
                    If Not String.IsNullOrWhiteSpace(url) AndAlso
                       name IsNot Nothing AndAlso
                       name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) AndAlso
                       Not name.Contains("Source Code", StringComparison.OrdinalIgnoreCase) Then
                        Return New ReleaseInfo With {.Tag = tag, .ExeUrl = url, .AssetName = name}
                    End If
                Next
            End If
            Return New ReleaseInfo With {.Tag = tag, .ExeUrl = Nothing, .AssetName = Nothing}
        End Using
    End Function

    Private Class TagVersion
        Public Property Tag As String
        Public Property Version As Version
    End Class

    Private Shared Async Function GetLatestTagVersionAsync() As Task(Of TagVersion)
        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromSeconds(5)
            client.DefaultRequestHeaders.UserAgent.ParseAdd("P2P-Monitor")

            Dim resp = Await client.GetAsync(TagsApiUrl)
            resp.EnsureSuccessStatusCode()
            Dim json = Await resp.Content.ReadAsStringAsync()

            Dim arr = JArray.Parse(json)
            If arr Is Nothing OrElse arr.Count = 0 Then
                Return Nothing
            End If

            Dim latestTag As String = arr(0)("name").ToString()

            Dim latestVer As Version = ParseSemVer(latestTag)
            If latestVer Is Nothing Then
                For i As Integer = 1 To arr.Count - 1
                    Dim t = arr(i)("name").ToString()
                    Dim candidate = ParseSemVer(t)
                    If candidate IsNot Nothing Then
                        Return New TagVersion With {.Tag = t, .Version = candidate}
                    End If
                Next
                Return Nothing
            End If

            Return New TagVersion With {.Tag = latestTag, .Version = latestVer}
        End Using
    End Function

    Private Shared Function ParseSemVer(tag As String) As Version
        Try
            Dim s = tag
            If s.StartsWith("v", StringComparison.OrdinalIgnoreCase) Then s = s.Substring(1)
            Dim dash = s.IndexOf("-"c)
            If dash >= 0 Then s = s.Substring(0, dash)
            Return New Version(s)
        Catch
            Return Nothing
        End Try
    End Function

    Private Shared Function ShowMaterialYesNo(owner As Form, title As String, message As String) As DialogResult
        Dim host As Form = If(owner, TryCast(Form.ActiveForm, Form))
        Using dlg As New UpdatePromptForm(title, message, host)
            If host IsNot Nothing Then
                Return dlg.ShowDialog(host)
            Else
                Return dlg.ShowDialog()
            End If
        End Using
    End Function

    Private NotInheritable Class UpdatePromptForm
        Inherits MaterialForm

        Private ReadOnly lbl As MaterialSkin.Controls.MaterialLabel
        Private ReadOnly btnYes As MaterialSkin.Controls.MaterialButton
        Private ReadOnly btnNo As MaterialSkin.Controls.MaterialButton
        Private _isReady As Boolean = False

        Public Sub New(title As String, message As String, themeHost As Form)
            Me.Text = title
            Me.StartPosition = FormStartPosition.CenterParent
            Me.MinimumSize = New Size(350, 240)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Sizable = False

            Me.SuspendLayout()
            Me.Padding = New Padding(3, 64, 3, 3)

            Dim manager = MaterialSkinManager.Instance
            manager.EnforceBackcolorOnAllComponents = False
            manager.AddFormToManage(Me)

            lbl = New MaterialSkin.Controls.MaterialLabel() With {
                .Text = message,
                .AutoSize = False,
                .Location = New Point(24, 80),
                .Size = New Size(460, 80)
            }
            Controls.Add(lbl)

            btnYes = New MaterialSkin.Controls.MaterialButton() With {.Text = "Yes", .AutoSize = True}
            AddHandler btnYes.Click, Sub(_s, _e)
                                         Me.DialogResult = DialogResult.Yes
                                         Me.Close()
                                     End Sub
            Controls.Add(btnYes)

            btnNo = New MaterialSkin.Controls.MaterialButton() With {.Text = "No", .AutoSize = True}
            AddHandler btnNo.Click, Sub(_s, _e)
                                        Me.DialogResult = DialogResult.No
                                        Me.Close()
                                    End Sub
            Controls.Add(btnNo)

            Me.AcceptButton = btnYes
            Me.CancelButton = btnNo

            _isReady = True
            LayoutButtons()
            Me.ResumeLayout(performLayout:=True)
        End Sub

        Protected Overrides Sub OnResize(e As EventArgs)
            MyBase.OnResize(e)
            If Not _isReady Then Return
            LayoutButtons()
        End Sub

        Private Sub LayoutButtons()
            If btnYes Is Nothing OrElse btnNo Is Nothing Then Return

            Dim yesSize As Size = If(btnYes.Width > 0 AndAlso btnYes.Height > 0,
                                     New Size(btnYes.Width, btnYes.Height),
                                     btnYes.GetPreferredSize(Size.Empty))
            Dim noSize As Size = If(btnNo.Width > 0 AndAlso btnNo.Height > 0,
                                    New Size(btnNo.Width, btnNo.Height),
                                    btnNo.GetPreferredSize(Size.Empty))

            Dim spacing As Integer = 16
            Dim totalButtonsWidth As Integer = yesSize.Width + spacing + noSize.Width
            Dim x As Integer = Math.Max(12, (ClientSize.Width - totalButtonsWidth) \ 2)
            Dim y As Integer = ClientSize.Height - Math.Max(yesSize.Height, noSize.Height) - 24

            btnYes.Location = New Point(x, y)
            btnNo.Location = New Point(x + yesSize.Width + spacing, y)
        End Sub
    End Class
End Class
