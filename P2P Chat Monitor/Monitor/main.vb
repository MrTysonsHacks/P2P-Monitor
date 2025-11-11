Imports System.Collections.Concurrent
Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks
Imports MaterialSkin
Imports MaterialSkin.Controls
Imports Microsoft.WindowsAPICodePack.Dialogs
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class main
    Private WithEvents cmbLogDir As MaterialSkin.Controls.MaterialComboBox
    Private isSyncingUI As Boolean = False
    Private Shared ReadOnly http As New Net.Http.HttpClient()
    Private robotoFont As Font = New Font("Roboto", 12.0F, FontStyle.Bold)
    Private monitoring As Boolean = False
    Private watcher As FileSystemWatcher
    Private lastProcessedTimes As New Dictionary(Of String, DateTime?)
    Private lastOffsets As New Dictionary(Of String, Long)
    Private LOG_DIR As String
    Private WEBHOOK_URL As String
    Private DISCORD_MENTION As String
    Private CHAT_CHANNEL As String
    Private ERROR_CHANNEL As String
    Private QUEST_CHANNEL As String
    Private TASK_CHANNEL As String
    Private SELFIE_CHANNEL As String
    Private monitorChat As Boolean
    Private takeSelfie As Boolean
    Private monitorQuests As Boolean
    Private monitorTasks As Boolean
    Private takeScreenshots As Boolean
    Private autoCleanup As Boolean
    Private DTMEnabled As Boolean
    Private checkInterval As Integer
    Private logTimer As System.Threading.Timer
    Private updateTimer As System.Threading.Timer
    Private updateCheckLock As Integer = 0
    Private lastFile As String = Nothing
    Friend Shared questFailureTriggers As New List(Of Regex)
    Friend Shared questFailureReasons As New List(Of KeyValuePair(Of Regex, String))
    Friend Shared skillFailureTriggers As New List(Of Regex)
    Friend Shared skillFailureReasons As New List(Of KeyValuePair(Of Regex, String))
    Friend Shared combatFailureTriggers As New List(Of Regex)
    Friend Shared combatFailureReasons As New List(Of KeyValuePair(Of Regex, String))
    Friend Shared botFailureTriggers As New List(Of Regex)
    Friend Shared botFailureReasons As New List(Of KeyValuePair(Of Regex, String))
    Private selfieTimer As System.Threading.Timer
    Public Shared accountBreakStates As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _q() As Keys = {Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Left, Keys.Right, Keys.B, Keys.A}
    Private _qix As Integer = 0
    Private _ovl As Control = Nothing
    Private tray As NotifyIcon
    Private trayMenu As ContextMenuStrip
    Private mnuShow As ToolStripMenuItem
    Private mnuStart As ToolStripMenuItem
    Private mnuStop As ToolStripMenuItem
    Private mnuExit As ToolStripMenuItem
    Private minimizeToTray As Boolean = True
    Private rememberCloseChoice As Boolean = False
    Private Enum CloseActionChoice
        None = 0
        MinimizeToTray = 1
        ExitMonitor = 2
    End Enum
    Private _selfieMinutes As Integer
    Private _selfieBusy As Integer = 0
    Private ReadOnly _earlyLog As New ConcurrentQueue(Of String)
    Private Class ThreadRoute
        Public Property Account As String
        Public Property ForumWebhook As String
        Public Property ShowChats As Boolean
        Public Property ShowTasks As Boolean
        Public Property ShowQuests As Boolean
        Public Property ShowErrors As Boolean
        Public Property ShowSelfies As Boolean
        Public Property ChatsId As String
        Public Property TasksId As String
        Public Property QuestsId As String
        Public Property ErrorsId As String
        Public Property SelfiesId As String
    End Class

    Private threadRouteCache As New List(Of ThreadRoute)()
    Private threadRouteMap As New Dictionary(Of String, ThreadRoute)(StringComparer.OrdinalIgnoreCase)

    Private Shared Function AppVersion() As String
        Try
            Return Application.ProductVersion
        Catch
            Return Assembly.GetExecutingAssembly().GetName().Version.ToString()
        End Try
    End Function

    Private Sub _kp(ByVal s As Object, ByVal e As KeyEventArgs)
        Dim k = e.KeyCode

        If k = _q(_qix) Then
            _qix += 1
            If _qix = _q.Length Then
                _qix = 0
                e.SuppressKeyPress = True
                __telemetry()
            End If
        Else
            _qix = If(k = _q(0), 1, 0)
        End If
    End Sub

    Private Sub __telemetry()
        If _ovl IsNot Nothing Then Return
        Dim fx As New _fxLayer(Me)
        _ovl = fx
        Me.Controls.Add(fx)
        fx.BringToFront()
        AddHandler fx.Disposed, Sub(_s As Object, _e As EventArgs) _ovl = Nothing
    End Sub


    Private NotInheritable Class _fxLayer
        Inherits Control

        Private ReadOnly _t As New System.Windows.Forms.Timer() With {.Interval = 16}
        Private ReadOnly _ttl As New System.Windows.Forms.Timer() With {.Interval = 6500}

        Private _ageMs As Integer
        Private ReadOnly _sw As New Stopwatch()
        Private ReadOnly _rnd As New Random()
        Private ReadOnly _p As New List(Of P)()

        Private Structure P
            Public X As Single
            Public Y As Single
            Public VX As Single
            Public VY As Single
            Public S As Single
            Public C As Color
            Public R As Single
            Public VR As Single
        End Structure

        Public Sub New(host As Form)
            SetStyle(
            ControlStyles.AllPaintingInWmPaint Or
            ControlStyles.UserPaint Or
            ControlStyles.OptimizedDoubleBuffer Or
            ControlStyles.SupportsTransparentBackColor,
            True)

            Dock = DockStyle.Fill
            BackColor = Color.Transparent
            Enabled = False

            Dim w = host.ClientSize.Width
            Dim h = host.ClientSize.Height
            If w <= 0 Then w = 1
            If h <= 0 Then h = 1

            Dim count As Integer = Math.Max(120, CInt(Math.Min(260, (w * h) / 8000.0)))

            For i = 0 To count - 1
                _p.Add(New P With {
                .X = _rnd.Next(0, w),
                .Y = -_rnd.Next(0, h \ 2),
                .VX = CSng((_rnd.NextDouble() - 0.5) * 3.5),
                .VY = CSng(1.2 + _rnd.NextDouble() * 2.0),
                .S = CSng(4 + _rnd.NextDouble() * 10),
                .C = Color.FromArgb(220, _rnd.Next(40, 255), _rnd.Next(40, 255), _rnd.Next(40, 255)),
                .R = CSng(_rnd.NextDouble() * Math.PI * 2),
                .VR = CSng((_rnd.NextDouble() - 0.5) * 0.25)
            })
            Next

            AddHandler _t.Tick, AddressOf OnTick
            AddHandler _ttl.Tick,
    Sub(_s As Object, _e As EventArgs)
        _ttl.Stop()
        Try : _t.Stop() : Catch : End Try
        Dispose()
    End Sub

            _sw.Start()
            _t.Start()
            _ttl.Start()
        End Sub

        Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
            MyBase.OnPaintBackground(e)
        End Sub

        Private Sub OnTick(sender As Object, e As EventArgs)
            Dim dtMs = CInt(_sw.ElapsedMilliseconds)
            _ageMs += dtMs
            _sw.Restart()

            For i = 0 To _p.Count - 1
                Dim q = _p(i)
                q.X += q.VX
                q.Y += q.VY
                q.VY += 0.035F
                q.R += q.VR
                _p(i) = q
            Next

            Invalidate()
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            Dim g = e.Graphics
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

            For Each q In _p
                Using br As New SolidBrush(q.C)
                    Dim s = q.S
                    g.TranslateTransform(q.X, q.Y)
                    g.RotateTransform(q.R * 57.29578F)
                    g.FillRectangle(br, -s * 0.5F, -s * 0.5F, s, s * 0.6F)
                    g.ResetTransform()
                End Using
            Next

            Dim rect = Me.ClientRectangle
            If rect.Width <= 0 OrElse rect.Height <= 0 Then Return

            Dim fSize As Single = Math.Max(28.0F, Math.Min(rect.Width, rect.Height) / 10.0F)
            Using f As New Font("Roboto", fSize, FontStyle.Bold)
                Dim sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                Dim txt As String = "Choco Is A Pussy"
                For dx = -2 To 2
                    For dy = -2 To 2
                        If dx = 0 AndAlso dy = 0 Then Continue For
                        Using sb As New SolidBrush(Color.FromArgb(180, 0, 0, 0))
                            g.DrawString(txt, f, sb, rect.Left + rect.Width \ 2 + dx, rect.Top + rect.Height \ 3 + dy, sf)
                        End Using
                    Next
                Next
                Using sb As New SolidBrush(Color.White)
                    g.DrawString(txt, f, sb, rect.Left + rect.Width \ 2, rect.Top + rect.Height \ 3, sf)
                End Using
            End Using
        End Sub
    End Class


    Private Sub InitTray()
        trayMenu = New ContextMenuStrip() With {.ShowImageMargin = False}

        mnuShow = New ToolStripMenuItem("Show Monitor")
        AddHandler mnuShow.Click, AddressOf RestoreFromTray

        mnuStart = New ToolStripMenuItem("Start Monitoring")
        AddHandler mnuStart.Click, Sub() btnStart.PerformClick()

        mnuStop = New ToolStripMenuItem("Stop Monitoring")
        AddHandler mnuStop.Click, Sub() btnStop.PerformClick()

        mnuExit = New ToolStripMenuItem("Exit")
        AddHandler mnuExit.Click, Sub()
                                      RemoveHandler Me.Resize, AddressOf Main_Resize
                                      tray.Visible = False
                                      tray.Dispose()
                                      minimizeToTray = False
                                      Me.Close()
                                  End Sub

        trayMenu.Items.AddRange(New ToolStripItem() {mnuShow, New ToolStripSeparator(), mnuStart, mnuStop, New ToolStripSeparator(), mnuExit})

        tray = New NotifyIcon() With {
        .Icon = Me.Icon,
        .Text = "P2P Monitor",
        .Visible = False,
        .ContextMenuStrip = trayMenu
    }
        AddHandler tray.DoubleClick, AddressOf RestoreFromTray
    End Sub

    Private Sub Main_Resize(sender As Object, e As EventArgs)
        If Me.WindowState = FormWindowState.Minimized Then
            HideToTray()
        End If
    End Sub

    Private Sub HideToTray()
        Try
            If Not tray.Visible Then
                tray.BalloonTipTitle = "P2P Monitor"
                tray.BalloonTipText = "Running in the background. Double-click to restore."
                tray.BalloonTipIcon = ToolTipIcon.Info
                tray.Visible = True
                tray.ShowBalloonTip(1500)
            End If
        Catch
        End Try

        ShowInTaskbar = False
        Me.Hide()
    End Sub

    Private Sub RestoreFromTray(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing)
        If Me.InvokeRequired Then
            Me.BeginInvoke(New Action(Sub() RestoreFromTray(sender, e)))
            Return
        End If
        If Me.IsDisposed Then Return
        ShowInTaskbar = True
        tray.Visible = False
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.TopMost = True
        Me.TopMost = False
        Me.BringToFront()
        Me.Activate()
        Me.Focus()
        Try
            If Hamburger IsNot Nothing AndAlso Not Hamburger.IsDisposed Then
                Dim wasEnabled = Hamburger.Enabled
                Hamburger.Enabled = False
                Hamburger.Enabled = wasEnabled
                Hamburger.Refresh()
            End If
        Catch
        End Try
    End Sub


    Private Shared Function ParseFolders(raw As String) As List(Of String)
        Dim list As New List(Of String)()
        If String.IsNullOrWhiteSpace(raw) Then Return list
        For Each s In raw.Split(";"c)
            Dim p = s.Trim()
            If p.Length > 0 AndAlso IO.Directory.Exists(p) Then
                If Not list.Contains(p, StringComparer.OrdinalIgnoreCase) Then
                    list.Add(p)
                End If
            End If
        Next
        Return list
    End Function

    Private Sub SyncComboFromText()
        If cmbLogDir Is Nothing Then Exit Sub
        isSyncingUI = True
        Try
            Dim keep As String = TryCast(cmbLogDir.SelectedItem, String)
            Dim items = ParseFolders(txtLogDir.Text)

            cmbLogDir.BeginUpdate()
            Try
                cmbLogDir.Items.Clear()
                If items.Count > 0 Then
                    cmbLogDir.Items.AddRange(items.Cast(Of Object).ToArray())
                    If Not String.IsNullOrEmpty(keep) AndAlso items.Contains(keep) Then
                        cmbLogDir.SelectedItem = keep
                    Else
                        cmbLogDir.SelectedIndex = 0
                    End If
                Else
                    cmbLogDir.SelectedIndex = -1
                End If
            Finally
                cmbLogDir.EndUpdate()
            End Try
        Finally
            isSyncingUI = False
        End Try
    End Sub

    Private Sub EnsureFolderInText(ByVal folder As String)
        Dim parts = txtLogDir.Text.Split(";"c).
        Select(Function(p) p.Trim()).
        Where(Function(p) p.Length > 0).
        Distinct(StringComparer.OrdinalIgnoreCase).
        ToList()

        If Not parts.Any(Function(p) p.Equals(folder, StringComparison.OrdinalIgnoreCase)) Then
            parts.Add(folder)
        End If

        Dim newRaw = String.Join(";", parts)
        If Not String.Equals(newRaw, txtLogDir.Text, StringComparison.Ordinal) Then
            txtLogDir.Text = newRaw
        Else
            SyncComboFromText()
        End If
    End Sub
    Private Sub TxtLogDir_TextChangedHandler(ByVal s As Object, ByVal ev As EventArgs)
        If isSyncingUI Then Return
        SyncComboFromText()
    End Sub

    Private Function GetAllParts() As List(Of String)
        Return ParseFolders(txtLogDir.Text)
    End Function

    Private Sub SetAllParts(parts As IEnumerable(Of String))
        Dim newRaw = String.Join(";", parts)
        If Not String.Equals(newRaw, txtLogDir.Text, StringComparison.Ordinal) Then
            txtLogDir.Text = newRaw
        End If
        SyncComboFromText()
    End Sub

    Private Sub StartUpdateTimer()
        StopUpdateTimer()
        updateTimer = New System.Threading.Timer(AddressOf UpdateTimerTick,
                                             Nothing,
                                             TimeSpan.FromMinutes(60),
                                             TimeSpan.FromMinutes(60))
    End Sub

    Private Sub StopUpdateTimer()
        If updateTimer IsNot Nothing Then
            updateTimer.Dispose()
            updateTimer = Nothing
        End If
    End Sub

    Private Sub UpdateTimerTick(state As Object)
        If Threading.Interlocked.Exchange(updateCheckLock, 1) = 1 Then Return
        Try
            If Me.IsHandleCreated AndAlso Not Me.IsDisposed Then
                Me.BeginInvoke(Async Sub()
                                   Try
                                       Await UpdateHelper.CheckForUpdatesAndPrompt(Me, AddressOf AppendLog,
                                                                                   Function() monitorAutoUpdate IsNot Nothing AndAlso monitorAutoUpdate.Checked)
                                   Catch ex As Exception
                                       AppendLog("⚠ Auto-update check failed: " & ex.Message)
                                   Finally
                                       Threading.Interlocked.Exchange(updateCheckLock, 0)
                                   End Try
                               End Sub)
            Else
                Threading.Interlocked.Exchange(updateCheckLock, 0)
            End If
        Catch
            Threading.Interlocked.Exchange(updateCheckLock, 0)
        End Try
    End Sub

    Private Shared Function TryCompileRegex(pattern As String) As Regex
        If String.IsNullOrWhiteSpace(pattern) Then Return Nothing
        Try
            Return New Regex(pattern, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
        Catch ex As ArgumentException
            Console.WriteLine("[FailRules] Invalid regex skipped: " & pattern & " :: " & ex.Message)
            Return Nothing
        End Try
    End Function

    Private Shared Sub addFailRule(category As String,
                               pattern As String,
                               Optional friendlyText As String = Nothing,
                               Optional isTrigger As Boolean = True)

        Dim rx As Regex = TryCompileRegex(pattern)
        If rx Is Nothing Then Exit Sub

        Select Case (If(category, String.Empty)).Trim().ToLowerInvariant()
            Case "quest"
                If isTrigger Then
                    questFailureTriggers.Add(rx)
                Else
                    questFailureReasons.Add(New KeyValuePair(Of Regex, String)(rx, If(friendlyText, String.Empty)))
                End If

            Case "skill"
                If isTrigger Then
                    skillFailureTriggers.Add(rx)
                Else
                    skillFailureReasons.Add(New KeyValuePair(Of Regex, String)(rx, If(friendlyText, String.Empty)))
                End If

            Case "combat"
                If isTrigger Then
                    combatFailureTriggers.Add(rx)
                Else
                    combatFailureReasons.Add(New KeyValuePair(Of Regex, String)(rx, If(friendlyText, String.Empty)))
                End If

            Case "bot"
                If isTrigger Then
                    botFailureTriggers.Add(rx)
                Else
                    botFailureReasons.Add(New KeyValuePair(Of Regex, String)(rx, If(friendlyText, String.Empty)))
                End If

            Case Else
                If isTrigger Then
                    botFailureTriggers.Add(rx)
                Else
                    botFailureReasons.Add(New KeyValuePair(Of Regex, String)(rx, If(friendlyText, String.Empty)))
                End If
        End Select
    End Sub

    Public Shared Sub GetAllFailRules(ByRef triggers As List(Of Regex),
                                  ByRef reasons As List(Of KeyValuePair(Of Regex, String)))
        triggers = New List(Of Regex)()
        reasons = New List(Of KeyValuePair(Of Regex, String))()

        triggers.AddRange(questFailureTriggers)
        triggers.AddRange(skillFailureTriggers)
        triggers.AddRange(combatFailureTriggers)
        triggers.AddRange(botFailureTriggers)

        reasons.AddRange(questFailureReasons)
        reasons.AddRange(skillFailureReasons)
        reasons.AddRange(combatFailureReasons)
        reasons.AddRange(botFailureReasons)
    End Sub

    Private Function GetFolderName(filePath As String) As String
        Dim dirPath As String = Path.GetDirectoryName(filePath)
        If String.IsNullOrWhiteSpace(dirPath) Then
            Return "Test Account"
        End If
        Return New DirectoryInfo(dirPath).Name
    End Function
    Private Const SW_RESTORE As Integer = 9
    Private Sub ChooseLogFolders(sender As Object, e As EventArgs) Handles btnBrowseLogDir.Click
        Using dlg As New CommonOpenFileDialog()
            dlg.IsFolderPicker = True
            dlg.Multiselect = True
            dlg.Title = "Select one or more log folders"
            dlg.EnsurePathExists = True
            dlg.EnsureValidNames = True

            If dlg.ShowDialog() = CommonFileDialogResult.Ok Then
                For Each p In dlg.FileNames
                    EnsureFolderInText(p)
                Next
                SyncComboFromText()
            End If
        End Using
    End Sub

    Private Async Sub OnStartup(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler Application.ThreadException, Sub(_s, ex) ErrorHandler.Report(ex.Exception, "UI Thread")
        AddHandler AppDomain.CurrentDomain.UnhandledException,
        Sub(_s, ex) ErrorHandler.Report(TryCast(ex.ExceptionObject, Exception), "Non-UI Thread")
        txtWebhook.Text = My.Settings.WebhookURL
        txtMention.Text = My.Settings.MentionID
        txtLogDir.Text = My.Settings.LogFolderPath
        chatID.Text = My.Settings.ChatID
        questID.Text = My.Settings.QuestID
        errorID.Text = My.Settings.ErrorID
        taskID.Text = My.Settings.TaskID
        selfieID.Text = My.Settings.SelfieID
        numIntervalSecond.Value = My.Settings.CheckInterval
        chatEmbed.Text = My.Settings.ChatEmbedSet
        errorEmbed.Text = My.Settings.ErrorEmbedSet
        questEmbed.Text = My.Settings.QuestEmbedSet
        selfieEmbed.Text = My.Settings.SelfieEmbedSet
        chkMonitorChat.Checked = My.Settings.CheckChat
        monitorQuest.Checked = My.Settings.CheckQuest
        captureWin.Checked = My.Settings.TakeScreenshots
        autoClean.Checked = My.Settings.AutoDelete
        combatError.Checked = My.Settings.CheckCombatErr
        questError.Checked = My.Settings.CheckQuestsErr
        skillIssue.Checked = My.Settings.CheckSkillsErr
        monitorTask.Checked = My.Settings.CheckTask
        taskEmbed.Text = My.Settings.TaskEmbedSet
        selfieMode.Checked = My.Settings.BotSelfie
        numSelfieInterval.Value = My.Settings.BotSelfieInterval
        obscureSS.Checked = My.Settings.BlurStats
        compositorSafe.Checked = My.Settings.CompositorSafe
        useDTM.Checked = My.Settings.useDTM
        monitorAutoUpdate.Checked = My.Settings.AutoUpdate
        ScreenshotHelpers.UseCompositorSafe = compositorSafe.Checked
        Me.KeyPreview = True
        AddHandler Me.KeyDown, AddressOf _kp
        numSelfieInterval.Value = If(My.Settings.BotSelfieInterval > 0, My.Settings.BotSelfieInterval, 60)
        numIntervalSecond.Value = If(My.Settings.CheckInterval > 0, My.Settings.CheckInterval, 5)
        DarkModeEnabled.Checked = My.Settings.DarkModeOn
        Dim SkinManager As MaterialSkinManager = MaterialSkinManager.Instance
        SkinManager.AddFormToManage(Me)
        ApplyTheme(DarkModeEnabled.Checked)

        txtLog.Font = robotoFont
        txtLogDir.Visible = False
        cmbLogDir = New MaterialSkin.Controls.MaterialComboBox() With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .MaxDropDownItems = 12,
            .Hint = "Log folders"
        }
        cmbLogDir.Left = txtLogDir.Left
        cmbLogDir.Top = txtLogDir.Top
        cmbLogDir.Width = txtLogDir.Width
        cmbLogDir.Height = txtLogDir.Height
        cmbLogDir.Anchor = txtLogDir.Anchor
        cmbLogDir.TabIndex = txtLogDir.TabIndex
        txtLogDir.Parent.Controls.Add(cmbLogDir)
        AddHandler txtLogDir.TextChanged, AddressOf TxtLogDir_TextChangedHandler

        SyncComboFromText()

        InitTray()
        AddHandler Me.Resize, AddressOf Main_Resize
        LoadCfg()

        If String.IsNullOrWhiteSpace(My.Settings.ChatEmbedSet) Then
            My.Settings.ChatEmbedSet = DiscordHelpers.defaultChatTemplate
            My.Settings.Save()
        End If
        chatEmbed.Text = My.Settings.ChatEmbedSet

        If String.IsNullOrWhiteSpace(My.Settings.QuestEmbedSet) Then
            My.Settings.QuestEmbedSet = DiscordHelpers.defaultQuestTemplate
            My.Settings.Save()
        End If
        questEmbed.Text = My.Settings.QuestEmbedSet

        If String.IsNullOrWhiteSpace(My.Settings.ErrorEmbedSet) Then
            My.Settings.ErrorEmbedSet = DiscordHelpers.defaultErrorTemplate
            My.Settings.Save()
        End If
        errorEmbed.Text = My.Settings.ErrorEmbedSet

        If String.IsNullOrWhiteSpace(My.Settings.TaskEmbedSet) Then
            My.Settings.TaskEmbedSet = DiscordHelpers.defaultTaskTemplate
            My.Settings.Save()
        End If
        taskEmbed.Text = My.Settings.TaskEmbedSet

        If String.IsNullOrWhiteSpace(My.Settings.SelfieEmbedSet) Then
            My.Settings.SelfieEmbedSet = DiscordHelpers.defaultSelfieTemplate
            My.Settings.Save()
        End If
        selfieEmbed.Text = My.Settings.SelfieEmbedSet
        MetricsHelper.StartMetrics(AddressOf AppendLog) 'Just comment out this line now if you don't wish to send updates for the live users
        Await FetchFailRules()
        Await UpdateHelper.CheckForUpdatesAndPrompt(Me, AddressOf AppendLog,
                                                    Function() monitorAutoUpdate IsNot Nothing AndAlso monitorAutoUpdate.Checked)
        StartUpdateTimer()
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        If e.CloseReason = CloseReason.UserClosing Then
            If rememberCloseChoice Then
                If minimizeToTray Then
                    e.Cancel = True
                    HideToTray()
                    AppendLog("Minimized to tray (right-click tray icon for options).")
                    Return
                Else
                End If
            Else
                Using dlg As New CloseChoiceDialog()
                    Dim result = dlg.ShowDialog(Me)
                    If result <> DialogResult.OK OrElse dlg.SelectedAction = CloseActionChoice.None Then
                        e.Cancel = True
                        Return
                    End If

                    If dlg.RememberChoice Then
                        rememberCloseChoice = True
                        minimizeToTray = (dlg.SelectedAction = CloseActionChoice.MinimizeToTray)
                        SaveCfg()
                    Else
                        minimizeToTray = (dlg.SelectedAction = CloseActionChoice.MinimizeToTray)
                    End If

                    If dlg.SelectedAction = CloseActionChoice.MinimizeToTray Then
                        e.Cancel = True
                        HideToTray()
                        AppendLog("Minimized to tray (right-click tray icon for options).")
                        Return
                    End If
                End Using
            End If
        End If
        MetricsHelper.StopMetrics()
        StopSelfieTimer()
        StopUpdateTimer()
        MyBase.OnFormClosing(e)
    End Sub

    Private Sub compositorSafe_CheckedChanged(sender As Object, e As EventArgs) Handles compositorSafe.CheckedChanged
        ScreenshotHelpers.UseCompositorSafe = compositorSafe.Checked
        My.Settings.CompositorSafe = compositorSafe.Checked
        My.Settings.Save()
        AppendLog(If(compositorSafe.Checked, "Compositor Safe Mode: ON (WGC path)", "Compositor Safe Mode: OFF (Legacy/PrintWindow mode)"))
    End Sub

    Private Sub ApplyTheme(enableDark As Boolean)
        If Me.InvokeRequired Then
            Me.BeginInvoke(New Action(Sub() ApplyTheme(enableDark)))
            Return
        End If
        If Not Me.IsHandleCreated OrElse Me.IsDisposed Then Return

        If Threading.Interlocked.Exchange(_applyingTheme, 1) = 1 Then Exit Sub
        Try
            Dim SkinManager As MaterialSkin.MaterialSkinManager = MaterialSkin.MaterialSkinManager.Instance
            If enableDark Then
                SkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK
                SkinManager.ColorScheme = New ColorScheme(Primary.Grey800, Primary.Grey900, Primary.Grey500, Accent.LightBlue200, TextShade.WHITE)
            Else
                SkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT
                SkinManager.ColorScheme = New ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE)
            End If
        Finally
            Threading.Interlocked.Exchange(_applyingTheme, 0)
        End Try
    End Sub

    Private Sub ToggleDarkMode(sender As Object, e As EventArgs) Handles DarkModeEnabled.CheckedChanged

        ApplyTheme(DarkModeEnabled.Checked)
        My.Settings.DarkModeOn = DarkModeEnabled.Checked
        My.Settings.Save()
    End Sub
    Private _applyingTheme As Integer = 0

    Public Sub AppendLog(msg As String)
        Try
            If Me.IsDisposed OrElse txtLog Is Nothing OrElse txtLog.IsDisposed Then Exit Sub

            If Me.IsHandleCreated AndAlso Me.InvokeRequired Then
                Me.BeginInvoke(New Action(Of String)(AddressOf AppendLog), msg)
                Return
            End If

            With txtLog
                .SelectionFont = robotoFont
                .AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}")
                .SelectionStart = .TextLength
                .ScrollToCaret()
            End With
        Catch
        End Try
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)
        If Me.IsDisposed OrElse txtLog Is Nothing OrElse txtLog.IsDisposed Then Return
        Dim sb As New System.Text.StringBuilder()
        Dim m As String = Nothing
        While _earlyLog.TryDequeue(m)
            sb.AppendFormat("[{0:HH:mm:ss}] {1}", DateTime.Now, m).AppendLine()
        End While
        If sb.Length > 0 Then
            With txtLog
                .SelectionFont = robotoFont
                .AppendText(sb.ToString())
                .SelectionStart = .TextLength
                .ScrollToCaret()
            End With
        End If
    End Sub

    Private watchers As New List(Of FileSystemWatcher)

    Private Shared Function LooksLikeUrl(s As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(s) AndAlso s.Trim().StartsWith("http", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function DetectCategory(embedTitle As String) As String
        If embedTitle.IndexOf("Chat", StringComparison.OrdinalIgnoreCase) >= 0 Then Return "chat"
        If embedTitle.IndexOf("Quest", StringComparison.OrdinalIgnoreCase) >= 0 Then Return "quest"
        If embedTitle.IndexOf("Error", StringComparison.OrdinalIgnoreCase) >= 0 Then Return "error"
        If embedTitle.IndexOf("Task", StringComparison.OrdinalIgnoreCase) >= 0 Then Return "task"
        Return ""
    End Function

    Private Function ResolveLegacyWebhookForCategory(category As String, baseWebhook As String) As String
        Dim overrideText As String = Nothing
        Select Case category.ToLowerInvariant()
            Case "chat" : overrideText = CHAT_CHANNEL
            Case "quest" : overrideText = QUEST_CHANNEL
            Case "error" : overrideText = ERROR_CHANNEL
            Case "task" : overrideText = TASK_CHANNEL
            Case Else : Return baseWebhook
        End Select

        If LooksLikeUrl(overrideText) Then
            Return overrideText
        End If
        Return baseWebhook
    End Function
    Private Function ResolveWebhookFor(embedTitle As String, baseWebhook As String, account As String) As String
        'no touchie, this is a sketch ass fix for some compiler errors.
        Return ResolveWebhookFor(embedTitle, baseWebhook, account, My.Settings.useDTM)
    End Function
    Private Function ResolveWebhookFor(embedTitle As String,
                                   baseWebhook As String,
                                   account As String,
                                   useDtm As Boolean) As String
        Dim category As String = DetectCategory(embedTitle)
        If Not useDtm Then
            Return ResolveLegacyWebhookForCategory(category, baseWebhook)
        End If

        Dim route As ThreadRoute = Nothing
        If Not String.IsNullOrWhiteSpace(account) AndAlso
       threadRouteMap.TryGetValue(account, route) AndAlso route IsNot Nothing Then

            Dim effectiveBase As String = If(Not String.IsNullOrWhiteSpace(route.ForumWebhook),
                                         route.ForumWebhook, baseWebhook)

            Select Case category
                Case "chat" : If route.ShowChats Then Return DiscordHelpers.WithThreadId(effectiveBase, route.ChatsId)
                Case "quest" : If route.ShowQuests Then Return DiscordHelpers.WithThreadId(effectiveBase, route.QuestsId)
                Case "error" : If route.ShowErrors Then Return DiscordHelpers.WithThreadId(effectiveBase, route.ErrorsId)
                Case "task" : If route.ShowTasks Then Return DiscordHelpers.WithThreadId(effectiveBase, route.TasksId)
            End Select
        End If

        Return ResolveLegacyWebhookForCategory(category, baseWebhook)
    End Function

    Private Sub StartMonitoring(sender As Object, e As EventArgs) Handles btnStart.Click
        If monitoring Then
            AppendLog("⚠ Already running.")
            Return
        End If

        My.Settings.ChatEmbedSet = chatEmbed.Text
        My.Settings.ErrorEmbedSet = errorEmbed.Text
        My.Settings.QuestEmbedSet = questEmbed.Text
        My.Settings.TaskEmbedSet = taskEmbed.Text
        My.Settings.CheckChat = chkMonitorChat.Checked
        My.Settings.CheckQuest = monitorQuest.Checked
        My.Settings.CheckTask = monitorTask.Checked
        My.Settings.TakeScreenshots = captureWin.Checked
        My.Settings.AutoDelete = autoClean.Checked
        My.Settings.CheckCombatErr = combatError.Checked
        My.Settings.CheckSkillsErr = skillIssue.Checked
        My.Settings.CheckQuestsErr = questError.Checked
        My.Settings.BlurStats = obscureSS.Checked
        My.Settings.WebhookURL = txtWebhook.Text.Trim()
        My.Settings.ChatID = chatID.Text.Trim()
        My.Settings.QuestID = questID.Text.Trim()
        My.Settings.ErrorID = errorID.Text.Trim()
        My.Settings.TaskID = taskID.Text.Trim()
        My.Settings.SelfieEmbedSet = selfieEmbed.Text.Trim()
        My.Settings.SelfieID = selfieID.Text.Trim()
        My.Settings.LogFolderPath = txtLogDir.Text.Trim()
        My.Settings.CheckInterval = numIntervalSecond.Value
        My.Settings.BotSelfieInterval = numSelfieInterval.Value
        My.Settings.BotSelfie = selfieMode.Checked
        My.Settings.DarkModeOn = DarkModeEnabled.Checked
        My.Settings.MentionID = txtMention.Text.Trim()
        My.Settings.CompositorSafe = compositorSafe.Checked
        My.Settings.useDTM = useDTM.Checked
        My.Settings.AutoUpdate = monitorAutoUpdate.Checked
        My.Settings.Save()

        WEBHOOK_URL = txtWebhook.Text.Trim()
        DISCORD_MENTION = txtMention.Text.Trim()
        LOG_DIR = My.Settings.LogFolderPath
        CHAT_CHANNEL = chatID.Text.Trim()
        ERROR_CHANNEL = errorID.Text.Trim()
        QUEST_CHANNEL = questID.Text.Trim()
        TASK_CHANNEL = taskID.Text.Trim()
        SELFIE_CHANNEL = selfieID.Text.Trim()
        checkInterval = CInt(numIntervalSecond.Value)

        ScreenshotHelpers.UseCompositorSafe = compositorSafe.Checked
        monitorChat = chkMonitorChat.Checked
        monitorQuests = monitorQuest.Checked
        monitorTasks = monitorTask.Checked
        takeSelfie = selfieMode.Checked
        takeScreenshots = captureWin.Checked
        autoCleanup = autoClean.Checked

        SaveCfg()

        If String.IsNullOrWhiteSpace(WEBHOOK_URL) OrElse Not WEBHOOK_URL.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
            AppendLog("⚠ Please enter a valid Discord Webhook URL.")
            Return
        End If

        If Not monitorChat AndAlso Not monitorQuests AndAlso Not questError.Checked AndAlso
       Not skillIssue.Checked AndAlso Not combatError.Checked AndAlso
       Not monitorTask.Checked AndAlso Not takeSelfie Then
            AppendLog("⚠ No monitoring options selected.")
            Return
        End If

        Dim logDirs = txtLogDir.Text.Split(";"c).
        Select(Function(p) p.Trim()).
        Where(Function(p) Directory.Exists(p)).ToList()

        monitoring = True
        watchers.Clear()

        For Each folder In logDirs
            For Each f In Directory.GetFiles(folder, "logfile-*.log", SearchOption.TopDirectoryOnly)
                LogHelper.JumpToEnd(f, lastOffsets, lastProcessedTimes)
            Next
        Next

        LogHelper.ResetSeen()
        Dim latestPer = LogHelper.GetLatestPerFolder(LOG_DIR)
        LogHelper.AnnounceLatestOnce(latestPer, AddressOf AppendLog, lastOffsets, lastProcessedTimes)

        For Each folder In logDirs
            For Each f In Directory.GetFiles(folder, "logfile-*.log", SearchOption.TopDirectoryOnly)
                LogHelper.JumpToEnd(f, lastOffsets, lastProcessedTimes)
            Next
        Next

        Dim onAnyChange As FileSystemEventHandler =
        Async Sub(s, eArgs)
            Await LogHelper.OnLogChanged(
            s, eArgs,
            monitoring, lastOffsets, lastProcessedTimes,
            monitorChat, monitorQuests, takeScreenshots, monitorTasks,
            questError.Checked, skillIssue.Checked, combatError.Checked,
            questFailureTriggers, questFailureReasons,
            skillFailureTriggers, skillFailureReasons,
            combatFailureTriggers, combatFailureReasons,
            AddressOf AppendLog, AddressOf SendSegments,
            AddressOf PostFailAlert, AddressOf GetFolderName)
        End Sub
        Dim onAnyError As ErrorEventHandler = Nothing
        onAnyError =
            Sub(_s, errArgs)
                Try
                    Dim folder As String = ""
                    Dim w0 = TryCast(_s, FileSystemWatcher)
                    If w0 IsNot Nothing Then
                        Try : folder = IO.Path.GetDirectoryName(w0.Path) : Catch : folder = w0.Path : End Try
                    End If

                    AppendLog($"⚠ Watcher error in {folder}: {errArgs.GetException()?.Message}")
                    MessageBox.Show(
                        $"Watcher error in {folder}:{Environment.NewLine}{errArgs.GetException()}",
                        "Watcher Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Try : w0?.Dispose() : Catch : End Try

                    Dim w As New FileSystemWatcher(folder, "logfile-*.log") With {
                        .NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.Size,
                        .IncludeSubdirectories = False,
                        .EnableRaisingEvents = True
                    }
                    AddHandler w.Created, onAnyChange
                    AddHandler w.Changed, onAnyChange
                    AddHandler w.Error, onAnyError

                    watchers.Add(w)
                    AppendLog($"🔁 Watcher restarted for {folder}.")
                Catch rex As Exception
                    ErrorHandler.Report(rex, "Recreate Watcher")
                End Try
            End Sub
        For Each folder In logDirs
            Dim watcher As New FileSystemWatcher(folder, "logfile-*.log")
            watcher.NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.Size
            watcher.IncludeSubdirectories = False
            watcher.InternalBufferSize = 64 * 1024
            watcher.EnableRaisingEvents = True
            AddHandler watcher.Changed, onAnyChange
            AddHandler watcher.Created, onAnyChange
            AddHandler watcher.Error, onAnyError
            watchers.Add(watcher)
        Next

        logTimer = New System.Threading.Timer(
        Sub(state) LogHelper.HeartbeatTick(state, AddressOf AppendLog, lastFile, checkInterval, LOG_DIR, lastOffsets, lastProcessedTimes),
        Nothing, checkInterval * 1000, checkInterval * 1000)

        AppendLog($"▶ Monitoring started in {logDirs.Count} folder(s).")

        If takeSelfie Then
            StartSelfieTimer()
        End If
    End Sub

    Private activityWatch As System.Threading.Timer

    Private Sub StartActivityWatchdog()
        activityWatch = New System.Threading.Timer(
        Sub(state As Object)
            Try
                Dim idle = DateTime.UtcNow - LogHelper.LastActivityUtc
                If idle > TimeSpan.FromMinutes(15) Then
                    AppendLog($"⏱ No processed activity in {CInt(idle.TotalMinutes)} minutes. Reinitializing watchers.")
                    MessageBox.Show(
                        $"No processed activity in {CInt(idle.TotalMinutes)} minutes. Watchers will be restarted.",
                        "Activity Watchdog", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Me.BeginInvoke(Sub()
                                       Try
                                           btnStop.PerformClick()
                                           btnStart.PerformClick()
                                       Catch ex As Exception
                                           ErrorHandler.Report(ex, "ActivityWatch Restart")
                                       End Try
                                   End Sub)
                End If
            Catch ex As Exception
                ErrorHandler.Report(ex, "ActivityWatch")
            End Try
        End Sub,
        Nothing, TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(5))
    End Sub

    Private Sub StopMonitoring(sender As Object, e As EventArgs) Handles btnStop.Click
        For Each watcher In watchers
            watcher.EnableRaisingEvents = False
            watcher.Dispose()
        Next
        watchers.Clear()

        If logTimer IsNot Nothing Then
            logTimer.Dispose()
            logTimer = Nothing
        End If

        StopSelfieTimer()

        monitoring = False
        AppendLog("⏹ Monitoring stopped.")
    End Sub

    Private Async Function PostFailAlert(trigger As String, reason As String, filePath As String, FailureType As String) As Task
        Dim accountName As String = GetFolderName(filePath)
        Dim targetWebhook As String = ResolveWebhookFor("Error", WEBHOOK_URL, accountName)
        Dim payload As String = errorEmbed.Text.Trim()
        If String.IsNullOrWhiteSpace(payload) Then
            AppendLog("⚠ Error embed textbox is empty, cannot send embed.")
            Return
        End If

        Dim filename As String = System.IO.Path.GetFileName(filePath)
        payload = DiscordHelpers.BuildErrorPayload(payload, DISCORD_MENTION, FailureType, trigger, reason, filename, GetFolderName(filePath), DateTime.Now)

        Try
            Await DiscordHelpers.PostJson(targetWebhook, payload)
            AppendLog($"🚨 {FailureType} Failure reported to Discord.")
        Catch ex As Exception
            AppendLog($"{FailureType} Failure webhook error: " & ex.Message)
        End Try
    End Function
    Private Async Function FetchFailRules() As Task

        Dim url = "https://docs.google.com/spreadsheets/d/1kLmq1Fj2OaT7BMQEF1N1dZq7Z-SIbhAFaPLJtOhGczE/export?format=csv"
        Try
            Dim csvData = Await DiscordHelpers.FetchText(url)
            Dim lines = csvData.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

            questFailureTriggers.Clear()
            questFailureReasons.Clear()
            skillFailureTriggers.Clear()
            skillFailureReasons.Clear()
            combatFailureTriggers.Clear()
            combatFailureReasons.Clear()
            botFailureTriggers.Clear()
            botFailureReasons.Clear()

            For i = 1 To lines.Length - 1
                Dim parts = lines(i).Split(","c)
                If parts.Length < 3 Then Continue For

                Dim category = parts(0).Trim().ToLower()
                Dim pattern = parts(1).Trim()
                Dim entryType = parts(2).Trim().ToLower()
                Dim friendly = If(parts.Length > 3, parts(3).Trim(), "")

                Select Case category
                    Case "quest", "skill", "combat", "bot"
                        If entryType = "trigger" Then
                            addFailRule(category, pattern, isTrigger:=True)
                        ElseIf entryType = "reason" Then
                            addFailRule(category, pattern, friendly, isTrigger:=False)
                        End If
                End Select
            Next
            AppendLog("✅ Failure rules loaded from Google Sheets.")
        Catch ex As Exception
            AppendLog("⚠ Failed to load rules from Google Sheets: " & ex.Message)
        End Try
    End Function

    Private Async Function SendSegments(segments As List(Of List(Of String)),
                                   path As String,
                                   embedTitle As String,
                                   embedColor As Integer,
                                   Optional screenshotPath As String = Nothing,
                                   Optional failureTrigger As String = "",
                                   Optional failureReason As String = "") As Task

        Dim screenshotRef As String = ""
        If Not String.IsNullOrWhiteSpace(screenshotPath) AndAlso IO.File.Exists(screenshotPath) Then
            screenshotRef = IO.Path.GetFileName(screenshotPath)
        End If

        For segIdx = 0 To segments.Count - 1
            Dim seg = segments(segIdx)

            Dim payload As String = Nothing

            If embedTitle.Contains("Chat") Then
                Dim rawChat As String = If(seg.Count > 1, String.Join(Environment.NewLine, seg.Take(seg.Count - 1)), seg.First())
                Dim rawResponse As String = If(seg.Count > 0, seg.Last(), "")
                Dim chatText As String = System.Text.RegularExpressions.Regex.Replace(rawChat, ".*CHAT:\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                Dim response As String = System.Text.RegularExpressions.Regex.Replace(rawResponse, ".*SLOWLY TYPING RESPONSE:\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                payload = DiscordHelpers.BuildChatPayload(
                chatEmbed.Text,
                DISCORD_MENTION,
                chatText,
                response,
                screenshotRef,
                IO.Path.GetFileName(path),
                GetFolderName(path),
                DateTime.Now,
                segIdx + 1
            )

            ElseIf embedTitle.Contains("Quest") Then
                Dim questText As String = String.Join(Environment.NewLine, seg)
                payload = DiscordHelpers.BuildQuestPayload(
                questEmbed.Text,
                DISCORD_MENTION,
                questText,
                screenshotRef,
                IO.Path.GetFileName(path),
                GetFolderName(path),
                DateTime.Now,
                segIdx + 1
            )

            ElseIf embedTitle.Contains("Task") Then
                Dim taskText As String = If(seg.Count > 0, seg(0), String.Empty)
                Dim activityText As String = If(seg.Count > 1, seg(1), String.Empty)
                payload = DiscordHelpers.BuildTaskPayload(
                taskEmbed.Text,
                DISCORD_MENTION,
                taskText,
                activityText,
                screenshotRef,
                IO.Path.GetFileName(path),
                GetFolderName(path),
                DateTime.Now,
                segIdx + 1
            )

            ElseIf embedTitle.Contains("Error") Then
                payload = DiscordHelpers.BuildErrorPayload(
                errorEmbed.Text,
                DISCORD_MENTION,
                embedTitle,
                failureTrigger,
                failureReason,
                IO.Path.GetFileName(path),
                GetFolderName(path),
                DateTime.Now
            )
            End If

            Dim accountName As String = GetFolderName(path)
            Dim targetWebhook As String = ResolveWebhookFor(embedTitle, WEBHOOK_URL, accountName)

            If String.IsNullOrWhiteSpace(targetWebhook) Then
                AppendLog($"🚫 {embedTitle}: disabled by Thread Manager for account '{accountName}'.")
                Continue For
            End If


            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                If Not String.IsNullOrWhiteSpace(screenshotPath) AndAlso IO.File.Exists(screenshotPath) Then
                    Await DiscordHelpers.UploadFile(targetWebhook, screenshotPath, payload, AddressOf AppendLog)
                    If autoCleanup Then
                        Await Task.Run(Async Function()
                                           Await Task.Delay(5000)
                                           Try
                                               IO.File.Delete(screenshotPath)
                                               AppendLog($"🗑 Deleted screenshot: {IO.Path.GetFileName(screenshotPath)}")
                                           Catch ex As Exception
                                               AppendLog($"⚠ Failed to delete screenshot: {ex.Message}")
                                           End Try
                                       End Function)
                    End If
                Else
                    Await DiscordHelpers.PostJson(targetWebhook, payload, AddressOf AppendLog)
                End If

                AppendLog($"✅ {embedTitle} embed sent (Segment {segIdx + 1}).")
            Else
                AppendLog($"⚠ Invalid JSON payload for {embedTitle} segment {segIdx + 1}: {err}")
            End If
        Next
    End Function

    Private Function GetSelfieIntervalMinutes() As Integer
        Dim m As Integer = System.Threading.Volatile.Read(_selfieMinutes)
        If m <= 0 Then
            m = Math.Max(0, CInt(My.Settings.BotSelfieInterval))
        End If
        Return m
    End Function

    Private Sub StartSelfieTimer()
        StopSelfieTimer()

        Dim url = txtWebhook.Text.Trim()
        If String.IsNullOrWhiteSpace(url) OrElse Not url.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
            AppendLog("⚠ Selfie mode: default Discord webhook is empty/invalid.")
            selfieMode.Checked = False
            Exit Sub
        End If

        Dim minutes As Integer = GetSelfieIntervalMinutes()
        Dim periodMs As Integer = minutes * 60 * 1000
        selfieTimer = New System.Threading.Timer(AddressOf SelfieTick, Nothing, periodMs, periodMs)

        AppendLog($"📸 Selfie mode enabled. Interval: {minutes} minute(s).")
    End Sub

    Private Async Sub SelfieTick(state As Object)
        Dim dtmEnabled As Boolean
        Try
            If Me.IsHandleCreated AndAlso Not Me.IsDisposed Then
                Me.Invoke(Sub() dtmEnabled = useDTM.Checked)
            Else
                dtmEnabled = My.Settings.useDTM
            End If
        Catch
            dtmEnabled = My.Settings.useDTM
        End Try

        If Threading.Interlocked.Exchange(_selfieBusy, 1) = 1 Then
            AppendLog("⏳ SelfieTick skipped (previous still running).")
            Return
        End If
        Try
            Dim selfieUrl As String = Nothing
            Dim defaultUrl As String = Nothing
            Dim rawLogDirs As String = Nothing

            Try
                If Me.IsHandleCreated AndAlso Not Me.IsDisposed Then
                    Me.Invoke(Sub()
                                  selfieUrl = selfieID.Text.Trim()
                                  defaultUrl = txtWebhook.Text.Trim()
                                  rawLogDirs = txtLogDir.Text
                              End Sub)
                Else
                    selfieUrl = My.Settings.SelfieID
                    defaultUrl = My.Settings.WebhookURL
                    rawLogDirs = My.Settings.LogFolderPath
                End If
            Catch
                selfieUrl = My.Settings.SelfieID
                defaultUrl = My.Settings.WebhookURL
                rawLogDirs = My.Settings.LogFolderPath
            End Try

            Dim baseWebhook As String =
            If(Not String.IsNullOrWhiteSpace(selfieUrl) AndAlso selfieUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase),
               selfieUrl,
               defaultUrl)

            If String.IsNullOrWhiteSpace(baseWebhook) OrElse Not baseWebhook.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
                AppendLog("⚠ Selfie mode: selfie/default Discord webhook is empty/invalid.")
                Return
            End If

            Dim roots = rawLogDirs.Split(";"c).
            Select(Function(p) p.Trim()).
            Where(Function(p) p.Length > 0 AndAlso IO.Directory.Exists(p)).
            ToList()

            If roots.Count = 0 Then
                AppendLog("⚠ Selfie mode: no valid log folders configured.")
                Return
            End If

            For Each accountFolder In roots
                Try
                    Dim folderName As String = New IO.DirectoryInfo(accountFolder).Name
                    Dim accountName As String = folderName

                    If accountBreakStates.ContainsKey(folderName) AndAlso accountBreakStates(folderName) Then
                        AppendLog($"⏸ Skipping selfie for {folderName} (on break).")
                        Continue For
                    End If

                    Dim selfieDir As String = IO.Path.Combine(accountFolder, "Selfies")
                    If Not IO.Directory.Exists(selfieDir) Then
                        IO.Directory.CreateDirectory(selfieDir)
                    End If

                    Dim shotPath As String = Await ScreenshotHelpers.CaptureBotSelfie(folderName, selfieDir, AddressOf AppendLog)
                    If String.IsNullOrWhiteSpace(shotPath) OrElse Not IO.File.Exists(shotPath) Then
                        AppendLog($"⚠ Failed to capture selfie for {folderName}.")
                        Continue For
                    End If

                    Dim effectiveWebhook As String =
                    If(LooksLikeUrl(selfieUrl), selfieUrl, baseWebhook)

                    If dtmEnabled Then
                        Dim route As ThreadRoute = Nothing
                        If threadRouteMap.TryGetValue(accountName, route) AndAlso route IsNot Nothing Then
                            If route.ShowSelfies Then
                                Dim forumBase As String = If(Not String.IsNullOrWhiteSpace(route.ForumWebhook), route.ForumWebhook, baseWebhook)
                                effectiveWebhook = DiscordHelpers.WithThreadId(forumBase, route.SelfiesId)
                            Else
                                effectiveWebhook = If(LooksLikeUrl(selfieUrl), selfieUrl, baseWebhook)
                            End If
                        End If
                    End If

                    Dim tmpl As String = If(String.IsNullOrWhiteSpace(My.Settings.SelfieEmbedSet),
                        DiscordHelpers.defaultSelfieTemplate,
                        My.Settings.SelfieEmbedSet)

                    Dim mention As String = ""
                    Try
                        If Me.IsHandleCreated AndAlso Not Me.IsDisposed Then
                            Me.Invoke(Sub() mention = txtMention.Text.Trim())
                        Else
                            mention = My.Settings.MentionID
                        End If
                    Catch
                        mention = My.Settings.MentionID
                    End Try

                    Dim payload As String = DiscordHelpers.BuildSelfiePayload(
                                                                              tmpl,
                                                                              mention:=mention,
                                                                              fileName:=IO.Path.GetFileName(shotPath),
                                                                              folder:=folderName,
                                                                              accountName:=accountName,
                                                                              timestamp:=DateTime.Now,
                                                                              index:=1,
                                                                              screenshotRef:=IO.Path.GetFileName(shotPath)
                                                                              )
                    Await DiscordHelpers.UploadFile(effectiveWebhook, shotPath, payload, AddressOf AppendLog)
                    AppendLog($"✅ Uploaded selfie for {folderName}.")

                    If autoCleanup AndAlso IO.File.Exists(shotPath) Then
                        Await Task.Run(Async Function()
                                           Await Task.Delay(5000)
                                           Try
                                               IO.File.Delete(shotPath)
                                               AppendLog($"🗑 Deleted selfie: {IO.Path.GetFileName(shotPath)}")
                                           Catch ex As Exception
                                               AppendLog($"⚠ Failed to delete selfie: {ex.Message}")
                                           End Try
                                       End Function)
                    End If

                Catch ex As Exception
                    AppendLog($"⚠ Selfie capture/upload error for {accountFolder}: {ex.Message}")
                End Try
            Next

        Catch ex As Exception
            AppendLog($"⚠ SelfieTick fatal error: {ex.Message}")
        Finally
            Threading.Interlocked.Exchange(_selfieBusy, 0)
        End Try
    End Sub

    Private Sub StopSelfieTimer()
        If selfieTimer IsNot Nothing Then
            selfieTimer.Dispose()
            selfieTimer = Nothing
            AppendLog("⏹ Selfie mode disabled.")
        End If
    End Sub

    Private Class AppCfg
        Public WebhookURL As String
        Public MentionID As String
        Public LogFolderPath As String
        Public ChatID As String
        Public QuestID As String
        Public ErrorID As String
        Public TaskID As String
        Public SelfieID As String
        Public CheckInterval As Integer
        Public BotSelfie As Boolean
        Public BotSelfieInterval As Integer
        Public TakeScreenshots As Boolean
        Public AutoDelete As Boolean
        Public CheckCombatErr As Boolean
        Public CheckSkillsErr As Boolean
        Public CheckQuestsErr As Boolean
        Public CheckChat As Boolean
        Public CheckQuest As Boolean
        Public CheckTask As Boolean
        Public DarkModeOn As Boolean
        Public BlurStats As Boolean
        Public monitorAutoUpdate As Boolean
        Public compositorSafe As Boolean
        Public useDTM As Boolean
        Public RememberCloseChoice As Boolean
        Public CloseToTray As Boolean
        Public ClientId As String
        Public ChatEmbedSet As String
        Public ErrorEmbedSet As String
        Public QuestEmbedSet As String
        Public TaskEmbedSet As String
        Public SelfieEmbedSet As String
        Public ThreadRoutes As List(Of ThreadRoute)
    End Class

    Private Shared Function GetCfgPath() As String
        Dim root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Dim dir = IO.Path.Combine(root, "DreamBot", "P2P Monitor")
        If Not IO.Directory.Exists(dir) Then IO.Directory.CreateDirectory(dir)
        Return IO.Path.Combine(dir, "settings.cfg")
    End Function

    Private Sub SaveCfg()
        Try
            Dim cfg As New AppCfg With {
            .WebhookURL = txtWebhook.Text.Trim(),
            .MentionID = txtMention.Text.Trim(),
            .LogFolderPath = txtLogDir.Text.Trim(),
            .ChatID = chatID.Text.Trim(),
            .QuestID = questID.Text.Trim(),
            .ErrorID = errorID.Text.Trim(),
            .TaskID = taskID.Text.Trim(),
            .SelfieID = selfieID.Text.Trim(),
            .CheckInterval = CInt(numIntervalSecond.Value),
            .BotSelfie = selfieMode.Checked,
            .BotSelfieInterval = CInt(numSelfieInterval.Value),
            .TakeScreenshots = captureWin.Checked,
            .AutoDelete = autoClean.Checked,
            .CheckCombatErr = combatError.Checked,
            .CheckSkillsErr = skillIssue.Checked,
            .CheckQuestsErr = questError.Checked,
            .CheckChat = chkMonitorChat.Checked,
            .CheckTask = monitorTask.Checked,
            .CheckQuest = monitorQuest.Checked,
            .DarkModeOn = DarkModeEnabled.Checked,
            .BlurStats = obscureSS.Checked,
            .monitorAutoUpdate = monitorAutoUpdate.Checked,
            .compositorSafe = compositorSafe.Checked,
            .useDTM = useDTM.Checked,
            .RememberCloseChoice = rememberCloseChoice,
            .CloseToTray = minimizeToTray,
            .ClientId = MetricsHelper.CLIENT_ID,
            .ChatEmbedSet = chatEmbed.Text,
            .ErrorEmbedSet = errorEmbed.Text,
            .QuestEmbedSet = questEmbed.Text,
            .TaskEmbedSet = taskEmbed.Text,
            .SelfieEmbedSet = selfieEmbed.Text,
            .ThreadRoutes = threadRouteCache
        }
            Dim json = Newtonsoft.Json.JsonConvert.SerializeObject(cfg, Formatting.Indented)
            IO.File.WriteAllText(GetCfgPath(), json, Encoding.UTF8)
            AppendLog("Settings saved to settings.cfg")
        Catch ex As Exception
            AppendLog($"⚠ Failed to save settings.cfg: {ex.Message}")
        End Try
    End Sub

    Private Sub LoadCfg()
        Try
            Dim path = GetCfgPath()
            If Not IO.File.Exists(path) Then
                MetricsHelper.CLIENT_ID = MetricsHelper.GenerateUuid()
                SaveCfg()
                AppendLog("Settings.cfg not found (using defaults/My.Settings).")
                Exit Sub
            End If

            Dim json = IO.File.ReadAllText(path, Encoding.UTF8)
            Dim cfg = Newtonsoft.Json.JsonConvert.DeserializeObject(Of AppCfg)(json)
            threadRouteCache = If(cfg.ThreadRoutes, New List(Of ThreadRoute)())
            threadRouteMap.Clear()
            For Each r In threadRouteCache
                If Not String.IsNullOrWhiteSpace(r.Account) Then
                    threadRouteMap(r.Account) = r
                End If
            Next
            If cfg Is Nothing Then
                AppendLog("⚠ Settings.cfg invalid; ignoring.")
                Exit Sub
            End If
            Dim root As JObject = JObject.Parse(json)

            If root("ClientId") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("ClientId"))) Then
                MetricsHelper.CLIENT_ID = CStr(root("ClientId"))
            Else
                MetricsHelper.CLIENT_ID = MetricsHelper.GenerateUuid()
                SaveCfg()
            End If

            If root("WebhookURL") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("WebhookURL"))) Then
                txtWebhook.Text = CStr(root("WebhookURL"))
            End If
            If root("MentionID") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("MentionID"))) Then
                txtMention.Text = CStr(root("MentionID"))
            End If
            If root("LogFolderPath") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("LogFolderPath"))) Then
                txtLogDir.Text = CStr(root("LogFolderPath"))
            End If
            If root("ChatID") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("ChatID"))) Then chatID.Text = CStr(root("ChatID"))
            If root("QuestID") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("QuestID"))) Then questID.Text = CStr(root("QuestID"))
            If root("ErrorID") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("ErrorID"))) Then errorID.Text = CStr(root("ErrorID"))
            If root("TaskID") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("TaskID"))) Then taskID.Text = CStr(root("TaskID"))
            If root("SelfieID") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("SelfieID"))) Then selfieID.Text = CStr(root("SelfieID"))
            If root("CheckInterval") IsNot Nothing Then
                Dim v As Integer = CInt(root("CheckInterval"))
                If v > 0 Then numIntervalSecond.Value = v
            End If
            If root("BotSelfieInterval") IsNot Nothing Then
                Dim v As Integer = CInt(root("BotSelfieInterval"))
                If v > 0 Then numSelfieInterval.Value = v
            End If
            If root("ChatEmbedSet") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("ChatEmbedSet"))) Then chatEmbed.Text = CStr(root("ChatEmbedSet"))
            If root("ErrorEmbedSet") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("ErrorEmbedSet"))) Then errorEmbed.Text = CStr(root("ErrorEmbedSet"))
            If root("QuestEmbedSet") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("QuestEmbedSet"))) Then questEmbed.Text = CStr(root("QuestEmbedSet"))
            If root("TaskEmbedSet") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("TaskEmbedSet"))) Then taskEmbed.Text = CStr(root("TaskEmbedSet"))
            If root("SelfieEmbedSet") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(CStr(root("SelfieEmbedSet"))) Then selfieEmbed.Text = CStr(root("SelfieEmbedSet"))
            If root("CheckChat") IsNot Nothing Then chkMonitorChat.Checked = CBool(root("CheckChat"))
            If root("CheckQuest") IsNot Nothing Then monitorQuest.Checked = CBool(root("CheckQuest"))
            If root("CheckTask") IsNot Nothing Then monitorTask.Checked = CBool(root("CheckTask"))
            If root("TakeScreenshots") IsNot Nothing Then captureWin.Checked = CBool(root("TakeScreenshots"))
            If root("AutoDelete") IsNot Nothing Then autoClean.Checked = CBool(root("AutoDelete"))
            If root("CheckCombatErr") IsNot Nothing Then combatError.Checked = CBool(root("CheckCombatErr"))
            If root("CheckSkillsErr") IsNot Nothing Then skillIssue.Checked = CBool(root("CheckSkillsErr"))
            If root("CheckQuestsErr") IsNot Nothing Then questError.Checked = CBool(root("CheckQuestsErr"))
            If root("BotSelfie") IsNot Nothing Then selfieMode.Checked = CBool(root("BotSelfie"))
            If root("DarkModeOn") IsNot Nothing Then DarkModeEnabled.Checked = CBool(root("DarkModeOn"))
            If root("BlurStats") IsNot Nothing Then obscureSS.Checked = CBool(root("BlurStats"))
            If root("useDTM") IsNot Nothing Then useDTM.Checked = CBool(root("useDTM"))
            If root("RememberCloseChoice") IsNot Nothing Then rememberCloseChoice = CBool(root("RememberCloseChoice"))
            If root("CloseToTray") IsNot Nothing Then minimizeToTray = CBool(root("CloseToTray"))
            If root("monitorAutoUpdate") IsNot Nothing Then monitorAutoUpdate.Checked = CBool(root("monitorAutoUpdate"))
            If root("compositorSafe") IsNot Nothing Then compositorSafe.Checked = CBool(root("compositorSafe"))
            SyncComboFromText()
            ApplyTheme(DarkModeEnabled.Checked)
            AppendLog("Settings.cfg loaded.")
        Catch ex As Exception
            AppendLog($"⚠ Failed to load settings.cfg: {ex.Message}")
        End Try
    End Sub

    Public Shared Function GetLiveLogFolderPaths() As List(Of String)
        Try
            Dim inst = Application.OpenForms.OfType(Of main)().FirstOrDefault()
            If inst IsNot Nothing AndAlso inst.IsHandleCreated AndAlso Not inst.IsDisposed Then
                Dim raw As String = Nothing
                inst.Invoke(Sub() raw = inst.txtLogDir.Text)
                Return ParseFolders(raw)
            End If
        Catch
        End Try
        Return ParseFolders(My.Settings.LogFolderPath)
    End Function

    Private Sub CommonButtons(sender As Object, e As EventArgs) Handles wikiBtn.Click, p2pdiscordBtn.Click, dbdiscordBtn.Click, dbforumBtn.Click, p2psalesBtn.Click, p2psetupBtn.Click, p2psurvivalBtn.Click, p2pgearBtn.Click, monitorDiscord.Click

        Dim url As String = Nothing
        Dim failMsg = ""

        Select Case True
            Case sender Is wikiBtn
                url = "https://wiki.aeglen.net/Main_Page"
                failMsg = "Failed to open wiki: "
            Case sender Is p2pdiscordBtn
                url = "https://discord.gg/5GVDqRhcM7"
                failMsg = "Failed to open P2P Discord: "
            Case sender Is dbdiscordBtn
                url = "https://discord.gg/bku99tbY"
                failMsg = "Failed to open Dreambot Discord: "
            Case sender Is dbforumBtn
                url = "https://dreambot.org/forums"
                failMsg = "Failed to open Dreambot forum: "
            Case sender Is p2psalesBtn
                url = "https://dreambot.org/forums/index.php?/store/product/597-p2p-master-ai"
                failMsg = "Failed to open P2P sales page: "
            Case sender Is p2psetupBtn
                url = "https://wiki.aeglen.net/Getting_Started"
                failMsg = "Failed to open P2P setup page: "
            Case sender Is p2psurvivalBtn
                url = "https://docs.google.com/spreadsheets/d/1G03zGPeqEc3jOrmGUeSccNbuoVpFCzMjC4R-Sy0lVo8/edit?usp=sharing"
                failMsg = "Failed to open P2P survival page: "
            Case sender Is p2pgearBtn
                url = "https://wiki.aeglen.net/Supported_Gear"
                failMsg = "Failed to open P2P supported gear page: "
            Case sender Is monitorDiscord
                url = "https://discord.gg/EpuaMTCzx5"
                failMsg = "Failed to open monitor Discord invite: "
        End Select

        If Not String.IsNullOrWhiteSpace(url) Then
            Try
                Process.Start(New ProcessStartInfo(url) With {.UseShellExecute = True})
            Catch ex As Exception
                AppendLog(failMsg & ex.Message)
            End Try
        End If
    End Sub

    Private Async Sub SendTestWebhooks(sender As Object, e As EventArgs) Handles testBtn.Click
        Dim webhookMap As New Dictionary(Of String, String) From {
        {"Default Webhook", txtWebhook.Text.Trim},
        {"Quest Webhook", questID.Text.Trim},
        {"Error Webhook", errorID.Text.Trim},
        {"Chat Webhook", chatID.Text.Trim},
        {"Task Webhook", taskID.Text.Trim},
        {"Selfie Webhook", selfieID.Text.Trim}
    }

        For Each entry In webhookMap
            Dim name = entry.Key
            Dim url = entry.Value

            If String.IsNullOrWhiteSpace(url) OrElse Not url.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
                AppendLog($"⚠ {name} is empty or invalid, skipping.")
                Continue For
            End If

            Dim payload =
            "{" &
            $"""content"": ""<@{DISCORD_MENTION}> – Test Embed""," &
            """embeds"": [{" &
            $"""title"": ""Test Webhook Embed""," &
            $"""description"": ""This is a test embed sent at {Date.Now}.""," &
            $"""color"": 6029136" &
            "}]}"
            Dim ok = Await DiscordHelpers.PostJsonOk(url, payload, AddressOf AppendLog)
            If ok Then
                AppendLog($"✅ Test embed sent to {name}.")
            Else
                AppendLog($"🚫 Test embed NOT sent to {name} (see error above).")
            End If
        Next

        Dim nowTs As DateTime = DateTime.Now
        Dim taskLine As String = "Simulated in-game task"
        Dim activityLine As String = "Simulated activity"
        Dim filePath As String = "test_log.log"
        Dim filename As String = IO.Path.GetFileName(filePath)
        Dim screenshotRef As String = ""

        If Not String.IsNullOrWhiteSpace(questEmbed.Text.Trim()) AndAlso Not String.IsNullOrWhiteSpace(questID.Text.Trim()) Then
            Dim payload = DiscordHelpers.BuildQuestPayload(
            questEmbed.Text.Trim(), DISCORD_MENTION, "Simulated quest text", screenshotRef,
            filename, GetFolderName(filePath), nowTs, 1)
            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                Dim ok = Await DiscordHelpers.PostJsonOk(questID.Text.Trim(), payload, AddressOf AppendLog)
                AppendLog(If(ok, "✅ Test Quest embed sent.", "🚫 Test Quest embed failed (see error above)."))
            Else
                AppendLog($"⚠ Invalid JSON in Quest embed: {err}")
            End If
        End If

        If Not String.IsNullOrWhiteSpace(taskEmbed.Text.Trim()) AndAlso Not String.IsNullOrWhiteSpace(taskID.Text.Trim()) Then
            Dim payload = DiscordHelpers.BuildTaskPayload(
            taskEmbed.Text.Trim(), DISCORD_MENTION, taskLine, activityLine, screenshotRef,
            filename, GetFolderName(filePath), nowTs, 1)
            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                Dim ok = Await DiscordHelpers.PostJsonOk(taskID.Text.Trim(), payload, AddressOf AppendLog)
                AppendLog(If(ok, "✅ Test Task embed sent (Task webhook).", "🚫 Test Task embed failed (see error above)."))
            Else
                AppendLog($"⚠ Invalid JSON in Task embed: {err}")
            End If
        End If

        If Not String.IsNullOrWhiteSpace(selfieID.Text.Trim()) AndAlso selfieID.Text.Trim().StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
            Dim selfiePayload As String = "{""content"": ""Test Selfie""}"
            Try
                Await DiscordHelpers.PostJson(selfieID.Text.Trim(), selfiePayload)
                AppendLog("✅ Test Selfie message sent (Selfie webhook).")
            Catch ex As Exception
                AppendLog("❌ Failed to send Test Selfie message: " & ex.Message)
            End Try
        Else
            AppendLog("ℹ Selfie webhook empty/invalid; skipping Test Selfie.")
        End If

        If useDTM.Checked Then
            If threadRouteMap Is Nothing OrElse threadRouteMap.Count = 0 Then
                AppendLog("ℹ Use Discord Threads is ON, but no routes are configured; skipping DTM tests.")
                Exit Sub
            End If

            Dim EffectiveBase As Func(Of ThreadRoute, String) =
                Function(r As ThreadRoute) As String
                    Dim baseUrlLocal As String = txtWebhook.Text.Trim()
                    If r IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(r.ForumWebhook) Then
                        baseUrlLocal = r.ForumWebhook
                    End If
                    Return baseUrlLocal
                End Function

            For Each kv In threadRouteMap
                Dim account As String = kv.Key
                Dim route = kv.Value
                Dim fakePath As String = IO.Path.Combine("C:\DTMTest", account, "logfile-test.log")
                Dim acctFilename As String = IO.Path.GetFileName(fakePath)

                If route.ShowChats Then
                    Dim payload = DiscordHelpers.BuildChatPayload(
                    chatEmbed.Text, DISCORD_MENTION,
                    "Simulated chat line", "Simulated response", screenshotRef,
                    acctFilename, account, nowTs, 1)
                    Dim target = ResolveWebhookFor("Chat", WEBHOOK_URL, account)
                    If Not String.IsNullOrWhiteSpace(target) Then
                        Dim err As String = Nothing
                        Dim ok = Await DiscordHelpers.PostJsonOk(target, payload, AddressOf AppendLog)
                        If ok Then
                            AppendLog($"✅ DTM test Chat sent for '{account}'.")
                        Else
                            AppendLog($"🚫 DTM test Chat failed for '{account}' (see error above).")
                        End If
                    Else
                        AppendLog($"🚫 Chat disabled by Thread Manager for '{account}'.")
                    End If
                End If

                If route.ShowQuests Then
                    Dim payload = DiscordHelpers.BuildQuestPayload(
                    questEmbed.Text, DISCORD_MENTION,
                    "Simulated quest text", screenshotRef,
                    acctFilename, account, nowTs, 1)
                    Dim target = ResolveWebhookFor("Quest", WEBHOOK_URL, account)
                    If Not String.IsNullOrWhiteSpace(target) Then
                        Dim err As String = Nothing
                        Dim ok = Await DiscordHelpers.PostJsonOk(target, payload, AddressOf AppendLog)
                        If ok Then
                            AppendLog($"✅ DTM test Quest sent for '{account}'.")
                        Else
                            AppendLog($"🚫 DTM test Quest failed for '{account}' (see error above).")
                        End If
                    Else
                        AppendLog($"🚫 Quest disabled by Thread Manager for '{account}'.")
                    End If
                End If

                If route.ShowTasks Then
                    Dim payload = DiscordHelpers.BuildTaskPayload(
                    taskEmbed.Text, DISCORD_MENTION,
                    taskLine, activityLine, screenshotRef,
                    acctFilename, account, nowTs, 1)
                    Dim target = ResolveWebhookFor("Task", WEBHOOK_URL, account)
                    If Not String.IsNullOrWhiteSpace(target) Then
                        Dim err As String = Nothing
                        Dim ok = Await DiscordHelpers.PostJsonOk(target, payload, AddressOf AppendLog)
                        If ok Then
                            AppendLog($"✅ DTM test Task sent for '{account}'.")
                        Else
                            AppendLog($"🚫 DTM test Task failed for '{account}' (see error above).")
                        End If
                    Else
                        AppendLog($"🚫 Task disabled by Thread Manager for '{account}'.")
                    End If
                End If

                If route.ShowErrors Then
                    Dim payload = DiscordHelpers.BuildErrorPayload(
                    errorEmbed.Text, DISCORD_MENTION,
                    "Test Error", "Simulated trigger", "Simulated reason",
                    acctFilename, account, nowTs)
                    Dim target = ResolveWebhookFor("Error", WEBHOOK_URL, account)
                    If Not String.IsNullOrWhiteSpace(target) Then
                        Dim err As String = Nothing
                        Dim ok = Await DiscordHelpers.PostJsonOk(target, payload, AddressOf AppendLog)
                        If ok Then
                            AppendLog($"✅ DTM test Error sent for '{account}'.")
                        Else
                            AppendLog($"🚫 DTM test Error failed for '{account}' (see error above).")
                        End If
                    Else
                        AppendLog($"🚫 Error disabled by Thread Manager for '{account}'.")
                    End If
                End If

                If route.ShowSelfies Then
                    Dim baseUrlDTM As String = EffectiveBase(route)
                    If Not String.IsNullOrWhiteSpace(baseUrlDTM) AndAlso
                    baseUrlDTM.StartsWith("http", StringComparison.OrdinalIgnoreCase) AndAlso Not String.IsNullOrWhiteSpace(route.SelfiesId) Then
                        Dim selfieUrl As String = DiscordHelpers.WithThreadId(baseUrlDTM, route.SelfiesId)
                        Dim tmplSelfie As String = If(String.IsNullOrWhiteSpace(selfieEmbed.Text),
                                      DiscordHelpers.defaultSelfieTemplate,
                                      selfieEmbed.Text)

                        Dim selfiePayload As String = DiscordHelpers.BuildSelfiePayload(
                                                                                        template:=tmplSelfie,
                                                                                        mention:=DISCORD_MENTION,
                                                                                        fileName:=acctFilename,
                                                                                        folder:=account,
                                                                                        accountName:=account,
                                                                                        timestamp:=nowTs,
                                                                                        index:=1,
                                                                                        screenshotRef:="")
                        Try
                            Await DiscordHelpers.PostJson(selfieUrl, selfiePayload)
                            AppendLog($"✅ DTM test Selfie sent for '{account}'.")
                        Catch ex As Exception
                            AppendLog($"❌ Failed to send DTM test Selfie for '{account}': {ex.Message}")
                        End Try
                    Else
                        AppendLog($"🚫 Selfie disabled or missing thread id for '{account}'.")
                    End If
                End If
            Next
        End If
    End Sub


    Private Sub embedEditors_Click(sender As Object, e As EventArgs) Handles embedEditors.Click
        Dim selector As New EmbedSelector()
        selector.ShowDialog(Me)
    End Sub

    Private Sub btnCleanLog_Click(sender As Object, e As EventArgs) Handles btnCleanLog.Click
        Try
            Using dlg As New CleanLogForm(LOG_DIR, AddressOf AppendLog)
                Dim result = dlg.ShowDialog(Me)
                If result = DialogResult.OK AndAlso Not String.IsNullOrWhiteSpace(dlg.OutputPath) Then
                    AppendLog($"Cleaned log saved: {dlg.OutputPath}")
                    Try
                        Process.Start("explorer.exe", "/select,""" & dlg.OutputPath & """")
                    Catch
                    End Try
                Else
                    AppendLog("Clean log cancelled.")
                End If
            End Using
        Catch ex As Exception
            AppendLog($"Clean log failed: {ex.Message}")
        End Try
    End Sub
    Private Sub btnAddAcc_Click(sender As Object, e As EventArgs) Handles btnAddAcc.Click
        Dim newName As String = P2P_Chat_Monitor.PromptForAccountName(Me)
        If Not String.IsNullOrWhiteSpace(newName) Then
            If Not accountNames.Items.Contains(newName) Then
                accountNames.Items.Add(newName)
                AppendLog($"Added account: {newName}")
            Else
                AppendLog($"⚠ Account '{newName}' already exists.")
            End If
        End If
    End Sub
    Private Sub btnDBPath_Click(sender As Object, e As EventArgs) Handles btnDBPath.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Java Archives (*.jar)|*.jar|All files (*.*)|*.*"
            ofd.Title = "Select a .jar file"
            ofd.Multiselect = False

            If ofd.ShowDialog() = DialogResult.OK Then
                dbPath.Text = ofd.FileName
                AppendLog($"Selected JAR: {ofd.FileName}")
            End If
        End Using
    End Sub
    Private Sub createCLI_Click(sender As Object, e As EventArgs) Handles createCLI.Click
        P2P_Chat_Monitor.GenerateBatchFile(
            dbPath.Text,
            ramNum.Value,
            covertMode.Checked,
            freshStart.Checked,
            accountNames.Items.Cast(Of String)(),
            cliOutput,
            launchP2P.Checked
        )
    End Sub

    Private Sub useDTM_CheckedChanged(sender As Object, e As EventArgs) Handles useDTM.CheckedChanged
        If useDTM.Checked Then
            dtmBtn.Visible = True
        Else
            dtmBtn.Visible = False
        End If
    End Sub

    Private Sub dtmBtn_Click(sender As Object, e As EventArgs) Handles dtmBtn.Click
        Using frm As New DiscordThreadManager()
            frm.ShowDialog(Me)
        End Using
        LoadCfg()
    End Sub

    Private Sub btnBackToMenu_Click(sender As Object, e As EventArgs) Handles btnBackToMenu.Click
        Try
            RemoveHandler Me.Resize, AddressOf Main_Resize
        Catch
        End Try
        Me.ShowInTaskbar = False
        Me.Hide()
        Dim menu = Application.OpenForms.OfType(Of Launcher).FirstOrDefault()
        If menu Is Nothing OrElse menu.IsDisposed Then
            menu = New Launcher()
        End If
        menu.StartPosition = FormStartPosition.CenterScreen
        menu.Show()
        menu.Activate()
    End Sub

    Private Async Sub btnCheckForUpdate_Click(sender As Object, e As EventArgs) Handles btnCheckUpdate.Click

        If Threading.Interlocked.Exchange(updateCheckLock, 1) = 1 Then
            AppendLog("⏳ Update check already running.")
            Return
        End If

        Try
            Await UpdateHelper.CheckForUpdatesAndPrompt(
            Me,
            AddressOf AppendLog,
            Function() monitorAutoUpdate IsNot Nothing AndAlso monitorAutoUpdate.Checked
        )
        Catch ex As Exception
            AppendLog("⚠ Manual update check failed: " & ex.Message)
        Finally
            Threading.Interlocked.Exchange(updateCheckLock, 0)
        End Try
    End Sub

    Private Class CloseChoiceDialog
        Inherits MaterialForm

        Public Property SelectedAction As CloseActionChoice = CloseActionChoice.None
        Public Property RememberChoice As Boolean = False

        Private ReadOnly btnTray As MaterialButton
        Private ReadOnly btnExit As MaterialButton
        Private ReadOnly chkRemember As MaterialCheckbox

        Public Sub New()
            Me.Text = "Close P2P Monitor"
            Me.StartPosition = FormStartPosition.CenterParent
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Sizable = False
            Me.Size = New Size(420, 220)

            Dim skin = MaterialSkinManager.Instance
            skin.AddFormToManage(Me)

            chkRemember = New MaterialCheckbox() With {
                .Text = "Remember my choice",
                .Location = New Point(40, 80),
                .AutoSize = True
            }

            btnTray = New MaterialButton() With {
                .Text = "Minimize to tray",
                .Location = New Point(40, 130),
                .AutoSizeMode = AutoSizeMode.GrowAndShrink
            }

            btnExit = New MaterialButton() With {
                .Text = "Exit Monitor",
                .Location = New Point(220, 130),
                .AutoSizeMode = AutoSizeMode.GrowAndShrink
            }

            AddHandler btnTray.Click,
                Sub()
                    SelectedAction = CloseActionChoice.MinimizeToTray
                    RememberChoice = chkRemember.Checked
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                End Sub

            AddHandler btnExit.Click,
                Sub()
                    SelectedAction = CloseActionChoice.ExitMonitor
                    RememberChoice = chkRemember.Checked
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                End Sub

            Me.Controls.Add(chkRemember)
            Me.Controls.Add(btnTray)
            Me.Controls.Add(btnExit)
        End Sub
    End Class

    Private Sub deleteLog_Click(sender As Object, e As EventArgs) Handles deleteLog.Click
        Dim sel = TryCast(cmbLogDir.SelectedItem, String)
        If String.IsNullOrWhiteSpace(sel) Then Return
        Dim parts = GetAllParts().Where(Function(p) Not p.Equals(sel, StringComparison.OrdinalIgnoreCase)).ToList()
        SetAllParts(parts)
    End Sub
End Class