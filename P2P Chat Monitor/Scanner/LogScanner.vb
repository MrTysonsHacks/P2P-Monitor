Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Net.Http
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports System.Drawing
Imports MaterialSkin
Imports MaterialSkin.Controls
Imports Microsoft.WindowsAPICodePack.Dialogs


Public Class LogScanner
    Inherits MaterialForm
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or &H2000000 ' WS_EX_COMPOSITED
            Return cp
        End Get
    End Property

    Private txtFolder As MaterialTextBox2
    Private btnBrowse As MaterialButton
    Private btnScan As MaterialButton
    Private btnBackToMenu As MaterialButton
    Private tvResults As MaterialTreeView
    Private lblSummary As MaterialLabel
    Private pbProgress As AccentProgressBar
    Private dlg As New FolderBrowserDialog()
    Private glyphs As ImageList

    Private Const SHEET_CSV_URL As String = "https://docs.google.com/spreadsheets/d/1kLmq1Fj2OaT7BMQEF1N1dZq7Z-SIbhAFaPLJtOhGczE/export?format=csv"

    Private Shared ReadOnly CATS As String() = {"quest", "skill", "combat", "bot"}

    Public Sub New()
        Me.ShowInTaskbar = True
        Try
            Dim launch = Application.OpenForms.OfType(Of Launcher)().FirstOrDefault()
            If launch IsNot Nothing AndAlso launch.Icon IsNot Nothing Then
                Me.Icon = launch.Icon
            Else
                Dim res As New System.ComponentModel.ComponentResourceManager(GetType(Launcher))
                Dim ico = TryCast(res.GetObject("$this.Icon"), Icon)
                If ico IsNot Nothing Then Me.Icon = ico
            End If
        Catch
        End Try
        Dim mgr = MaterialSkinManager.Instance
        mgr.AddFormToManage(Me)
        mgr.Theme = MaterialSkinManager.Themes.DARK
        mgr.ColorScheme = New ColorScheme(Primary.Grey800, Primary.Grey900, Primary.Grey500, Accent.LightBlue200, TextShade.WHITE)

        Me.Text = "P2P Log Scanner"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(900, 680)

        txtFolder = New MaterialTextBox2() With {
            .Hint = "Logs root folder (you can use ; to separate multiple roots)",
            .Location = New Point(24, 80),
            .Width = 650
        }
        Me.Controls.Add(txtFolder)

        btnBrowse = New MaterialButton() With {
            .Text = "Browse",
            .Location = New Point(690, 86),
            .AutoSize = True
        }
        AddHandler btnBrowse.Click, AddressOf BrowseForFolders
        Me.Controls.Add(btnBrowse)

        btnScan = New MaterialButton() With {
            .Text = "Scan",
            .Location = New Point(24, 140),
            .AutoSize = True,
            .HighEmphasis = True
        }
        AddHandler btnScan.Click, AddressOf OnScanClick
        Me.Controls.Add(btnScan)

        btnBackToMenu = New MaterialButton() With {
            .Text = "Main Menu",
            .AutoSize = False,
            .Size = New Size(168, 36),
            .Location = New Point(24, 635),
            .HighEmphasis = True
        }
        AddHandler btnBackToMenu.Click, AddressOf BackToMenu_Click
        Me.Controls.Add(btnBackToMenu)

        lblSummary = New MaterialLabel() With {
            .Text = "",
            .Location = New Point(135, 146),
            .AutoSize = True
        }
        Me.Controls.Add(lblSummary)

        tvResults = New MaterialTreeView() With {
            .Location = New Point(24, 190),
            .Size = New Size(830, 400),
            .HideSelection = False,
            .ShowNodeToolTips = True
        }
        Me.Controls.Add(tvResults)

        pbProgress = New AccentProgressBar() With {
            .Location = New Point(24, 600),
            .Size = New Size(830, 6),
            .Value = 0
        }
        Me.Controls.Add(pbProgress)

        tvResults.ImageList = glyphs
    End Sub

    Private Async Sub OnScanClick(sender As Object, e As EventArgs)
        tvResults.Nodes.Clear()
        lblSummary.Text = ""
        pbProgress.Value = 0

        Dim rootsText As String = If(txtFolder.Text, String.Empty).Trim()
        If String.IsNullOrWhiteSpace(rootsText) Then
            tvResults.Nodes.Add("Choose a valid folder.")
            Return
        End If

        Dim roots = rootsText.Split(";"c).
            Select(Function(p) p.Trim()).
            Where(Function(p) p <> "" AndAlso Directory.Exists(p)).
            ToList()

        If roots.Count = 0 Then
            tvResults.Nodes.Add("Choose a valid folder.")
            Return
        End If

        btnScan.Enabled = False
        btnScan.Text = "Scanning…"

        Dim totalFiles As Integer = 0
        Dim filesWithFindings As Integer = 0
        Dim totalPairs As Integer = 0

        Dim trigByCat As Dictionary(Of String, List(Of Regex)) = Nothing
        Dim reasByCat As Dictionary(Of String, List(Of KeyValuePair(Of Regex, String))) = Nothing

        Dim rr = Await FetchRulesFromSheet(SHEET_CSV_URL)
        If rr Is Nothing OrElse Not rr.Success Then
            If Not TryGetRulesByCategoryFromMain(trigByCat, reasByCat) Then
                tvResults.Nodes.Add("Failed to load rules from Google Sheet and no cached rules found.")
                btnScan.Enabled = True : btnScan.Text = "Scan"
                Return
            End If
        Else
            trigByCat = rr.Triggers
            reasByCat = rr.Reasons
        End If


        Dim allFiles As New List(Of String)
        For Each root In roots
            Try
                allFiles.AddRange(Directory.GetFiles(root, "logfile-*.log", SearchOption.AllDirectories))
            Catch
            End Try
        Next
        Dim totalToScan As Integer = allFiles.Count
        pbProgress.Maximum = Math.Max(1, totalToScan)
        pbProgress.Value = 0

        Dim folderNodes As New Dictionary(Of String, TreeNode)(StringComparer.OrdinalIgnoreCase)
        Dim processed As Integer = 0

        Await Task.Run(
            Sub()
                For Each root In roots
                    Dim files As String() = {}
                    Try
                        files = Directory.GetFiles(root, "logfile-*.log", SearchOption.AllDirectories)
                    Catch ex As Exception
                        Dim capRoot = root
                        Me.Invoke(Sub() AddErrorNode(capRoot & ": " & ex.Message))
                    End Try

                    For Each f In files
                        totalFiles += 1
                        Try
                            Dim rawLines As List(Of String) = ReadAllLinesShared(f)
                            If rawLines Is Nothing OrElse rawLines.Count = 0 Then
                                processed += 1
                                Me.Invoke(Sub() UpdateProgress(processed, totalToScan))
                                Continue For
                            End If

                            Dim lines As New List(Of String)(rawLines.Count)
                            Dim stamps As New List(Of String)(rawLines.Count)
                            For Each s As String In rawLines
                                stamps.Add(ExtractTimestamp(s))
                                lines.Add(CleanLogPrefix(s))
                            Next

                            Dim pairs As New List(Of Tuple(Of String, String, String, String, String))()

                            For i As Integer = 0 To lines.Count - 1
                                Dim triggerLine As String = lines(i)

                                Dim cat As String = GetTriggerCategory(triggerLine, trigByCat)
                                If cat Is Nothing Then Continue For

                                Dim reason As String = String.Empty
                                Dim extra As String = String.Empty
                                Dim reasonIdx As Integer = -1
                                If TryFindReasonAround(lines, i, reasByCat(cat), 10, reason, extra, reasonIdx) Then
                                    Dim trigTs As String = If(i >= 0 AndAlso i < stamps.Count, stamps(i), "")
                                    Dim reasTs As String = If(reasonIdx >= 0 AndAlso reasonIdx < stamps.Count, stamps(reasonIdx), "")
                                    pairs.Add(Tuple.Create(triggerLine, reason, extra, trigTs, reasTs))
                                End If
                            Next

                            If pairs.Count > 0 Then
                                filesWithFindings += 1
                                totalPairs += pairs.Count
                                Me.Invoke(Sub() AddFileResultsToTree(tvResults, folderNodes, root, f, pairs))
                            End If

                        Catch ex As Exception
                            Dim capF = f
                            Me.Invoke(Sub() AddErrorNode(capF & ": [scan error] " & ex.Message))
                        Finally
                            processed += 1
                            Me.Invoke(Sub() UpdateProgress(processed, totalToScan))
                        End Try
                    Next
                Next
            End Sub)

        tvResults.BeginUpdate()
        For Each n As TreeNode In tvResults.Nodes
            n.Expand()
        Next
        tvResults.EndUpdate()

        lblSummary.Text = String.Format("Scanned {0} file(s) • {1} with matches • {2} Error(s) total",
                                       totalFiles, filesWithFindings, totalPairs)
        If tvResults.Nodes.Count = 0 Then tvResults.Nodes.Add("No failures found.")

        btnScan.Enabled = True
        btnScan.Text = "Scan"
        pbProgress.Value = pbProgress.Maximum
    End Sub

    Private Sub BackToMenu_Click(sender As Object, e As EventArgs)
        Me.ShowInTaskbar = False
        Me.Hide()

        Dim menu = Application.OpenForms.OfType(Of Launcher)().FirstOrDefault()
        If menu Is Nothing OrElse menu.IsDisposed Then
            menu = New Launcher()
        End If

        menu.StartPosition = FormStartPosition.CenterScreen
        menu.Show()
        menu.Activate()
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        If e.CloseReason = CloseReason.UserClosing Then
            Try
                Application.Exit()
            Finally
                Environment.Exit(0)
            End Try
            Return
        End If

        MyBase.OnFormClosing(e)
    End Sub
    Private Class RulesResult
        Public Property Success As Boolean
        Public Property Triggers As Dictionary(Of String, List(Of Regex))
        Public Property Reasons As Dictionary(Of String, List(Of KeyValuePair(Of Regex, String)))
    End Class

    Private Async Function FetchRulesFromSheet(csvUrl As String) As Task(Of RulesResult)
        Dim trigByCat As New Dictionary(Of String, List(Of Regex))(StringComparer.OrdinalIgnoreCase)
        Dim reasByCat As New Dictionary(Of String, List(Of KeyValuePair(Of Regex, String)))(StringComparer.OrdinalIgnoreCase)
        For Each c In CATS
            trigByCat(c) = New List(Of Regex)()
            reasByCat(c) = New List(Of KeyValuePair(Of Regex, String))()
        Next

        Try
            Dim csv As String
            Using client As New HttpClient()
                csv = Await client.GetStringAsync(csvUrl).ConfigureAwait(False)
            End Using
            If String.IsNullOrWhiteSpace(csv) Then
                Return New RulesResult With {.Success = False, .Triggers = trigByCat, .Reasons = reasByCat}
            End If

            Dim lines = csv.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).
                        Split({vbLf}, StringSplitOptions.RemoveEmptyEntries)
            If lines.Length = 0 Then
                Return New RulesResult With {.Success = False, .Triggers = trigByCat, .Reasons = reasByCat}
            End If

            Dim header = ParseCsvLine(lines(0))
            Dim idxCategory = IndexOfHeader(header, {"category"})
            Dim idxPattern = IndexOfHeader(header, {"pattern"})
            Dim idxType = IndexOfHeader(header, {"type"})
            Dim idxText = IndexOfHeader(header, {"friendlytext", "friendly_text", "text", "reasontext"})
            If idxCategory = -1 OrElse idxPattern = -1 OrElse idxType = -1 Then
                Return New RulesResult With {.Success = False, .Triggers = trigByCat, .Reasons = reasByCat}
            End If

            For i = 1 To lines.Length - 1
                Dim row = ParseCsvLine(lines(i))
                If row.Count <= Math.Max(Math.Max(idxCategory, idxPattern), Math.Max(idxType, If(idxText >= 0, idxText, -1))) Then Continue For

                Dim catRaw = row(idxCategory)
                Dim pat = row(idxPattern)
                Dim typ = row(idxType)
                Dim friendly = If(idxText >= 0 AndAlso idxText < row.Count, row(idxText), Nothing)

                Dim cat = NormalizeCategory(catRaw)
                If String.IsNullOrWhiteSpace(cat) OrElse String.IsNullOrWhiteSpace(pat) OrElse String.IsNullOrWhiteSpace(typ) Then Continue For

                Dim rx As Regex = TryCompileRegex(pat)
                If rx Is Nothing Then Continue For

                If typ.Trim().Equals("trigger", StringComparison.OrdinalIgnoreCase) Then
                    trigByCat(cat).Add(rx)
                ElseIf typ.Trim().Equals("reason", StringComparison.OrdinalIgnoreCase) Then
                    reasByCat(cat).Add(New KeyValuePair(Of Regex, String)(rx, If(friendly, String.Empty)))
                End If
            Next

            Dim ok As Boolean = CATS.Any(Function(c) trigByCat(c).Count > 0)
            Return New RulesResult With {.Success = ok, .Triggers = trigByCat, .Reasons = reasByCat}

        Catch
            Return New RulesResult With {.Success = False, .Triggers = trigByCat, .Reasons = reasByCat}
        End Try
    End Function

    Private Sub BrowseForFolders(sender As Object, e As EventArgs)
        Using picker As New CommonOpenFileDialog()
            picker.IsFolderPicker = True
            picker.Multiselect = True
            picker.Title = "Select one or more log folders"
            picker.EnsurePathExists = True
            picker.EnsureValidNames = True

            If picker.ShowDialog() = CommonFileDialogResult.Ok Then
                Dim parts As List(Of String) =
                If(If(txtFolder.Text, String.Empty).
                   Split(";"c).
                   Select(Function(s) s.Trim()).
                   Where(Function(s) s.Length > 0).
                   Distinct(StringComparer.OrdinalIgnoreCase).
                   ToList(),
                   New List(Of String)())

                For Each p In picker.FileNames
                    Dim pp = p.Trim()
                    If pp.Length = 0 Then Continue For
                    If parts.FindIndex(Function(x) x.Equals(pp, StringComparison.OrdinalIgnoreCase)) = -1 Then
                        parts.Add(pp)
                    End If
                Next

                txtFolder.Text = String.Join(";", parts)
            End If
        End Using
    End Sub

    Private Function NormalizeCategory(s As String) As String
        If s Is Nothing Then Return Nothing
        Dim t = s.Trim().ToLowerInvariant()
        If t = "quest" OrElse t = "questing" Then Return "quest"
        If t = "skill" OrElse t = "skills" Then Return "skill"
        If t = "combat" Then Return "combat"
        If t = "bot" OrElse t = "other" Then Return "bot"
        Return Nothing
    End Function

    Private Function TryCompileRegex(pattern As String) As Regex
        If String.IsNullOrWhiteSpace(pattern) Then Return Nothing
        Try
            Return New Regex(pattern, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
        Catch
            Return Nothing
        End Try
    End Function

    Private Function IndexOfHeader(header As List(Of String), candidates As IEnumerable(Of String)) As Integer
        For i = 0 To header.Count - 1
            Dim h = header(i)
            For Each cand In candidates
                If String.Equals(h.Trim(), cand, StringComparison.OrdinalIgnoreCase) Then
                    Return i
                End If
            Next
        Next
        Return -1
    End Function

    Private Function ParseCsvLine(line As String) As List(Of String)
        Dim res As New List(Of String)()
        If line Is Nothing Then Return res
        Dim sb As New StringBuilder()
        Dim inQuotes As Boolean = False

        Dim i As Integer = 0
        While i < line.Length
            Dim ch = line(i)
            If ch = """"c Then
                If inQuotes AndAlso i + 1 < line.Length AndAlso line(i + 1) = """"c Then
                    sb.Append(""""c) : i += 1
                Else
                    inQuotes = Not inQuotes
                End If
            ElseIf ch = ","c AndAlso Not inQuotes Then
                res.Add(sb.ToString()) : sb.Clear()
            Else
                sb.Append(ch)
            End If
            i += 1
        End While
        res.Add(sb.ToString())

        Return res
    End Function

    Private Sub UpdateProgress(done As Integer, total As Integer)
        If total <= 0 Then
            pbProgress.Value = 0
            lblSummary.Text = ""
            Return
        End If
        pbProgress.Value = Math.Min(pbProgress.Maximum, Math.Max(0, done))
        lblSummary.Text = "Scanning " & done & " / " & total
    End Sub

    Private Sub AddErrorNode(msg As String)
        Dim n = tvResults.Nodes.Add(msg)
        n.ImageKey = "error"
        n.SelectedImageKey = "error"
        n.ToolTipText = msg
    End Sub

    Private Sub AddFileResultsToTree(tv As TreeView,
                                     folderNodes As Dictionary(Of String, TreeNode),
                                     rootPath As String,
                                     filePath As String,
                                     pairs As List(Of Tuple(Of String, String, String, String, String)))
        tv.BeginUpdate()
        Try
            Dim folderNode As TreeNode = Nothing
            If Not folderNodes.TryGetValue(rootPath, folderNode) Then
                folderNode = tv.Nodes.Add(rootPath)
                folderNode.ToolTipText = rootPath
                folderNode.ImageKey = "folder" : folderNode.SelectedImageKey = "folder"
                folderNodes(rootPath) = folderNode
            End If

            Dim relative As String
            Try
                If filePath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase) Then
                    relative = filePath.Substring(rootPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                Else
                    relative = filePath
                End If
            Catch
                relative = Path.GetFileName(filePath)
            End Try

            Dim fileNode As TreeNode = folderNode.Nodes.Add(relative)
            fileNode.ToolTipText = filePath
            fileNode.ImageKey = "file" : fileNode.SelectedImageKey = "file"

            Dim idx As Integer = 1
            For Each p In pairs
                Dim findingNode As TreeNode = fileNode.Nodes.Add("#" & idx.ToString())
                findingNode.ToolTipText = "Failure #" & idx.ToString()
                findingNode.ImageKey = "error" : findingNode.SelectedImageKey = "error"

                Dim triggerLabel As String = If(String.IsNullOrEmpty(p.Item4),
                                                "Trigger: " & p.Item1,
                                                "Trigger: [" & p.Item4 & "] " & p.Item1)
                Dim reasonBase As String = If(String.IsNullOrWhiteSpace(p.Item3),
                                              p.Item2,
                                              p.Item2 & " (" & p.Item3 & ")")
                Dim reasonLabel As String = If(String.IsNullOrEmpty(p.Item5),
                                               "Reason: " & reasonBase,
                                               "Reason: [" & p.Item5 & "] " & reasonBase)

                Dim triggerNode = findingNode.Nodes.Add(triggerLabel)
                triggerNode.ToolTipText = triggerLabel
                triggerNode.ImageKey = "trigger" : triggerNode.SelectedImageKey = "trigger"

                Dim reasonNode = findingNode.Nodes.Add(reasonLabel)
                reasonNode.ToolTipText = reasonLabel
                reasonNode.ImageKey = "reason" : reasonNode.SelectedImageKey = "reason"

                idx += 1
            Next
        Finally
            tv.EndUpdate()
        End Try
    End Sub

    Private Function ExtractTimestamp(src As String) As String
        If String.IsNullOrEmpty(src) Then Return ""
        Dim m = Regex.Match(src, "^\s*(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})")
        If m.Success Then Return m.Groups(1).Value
        Return ""
    End Function

    Private Function ReadAllLinesShared(path As String,
                                        Optional maxRetries As Integer = 4,
                                        Optional delayMs As Integer = 120) As List(Of String)
        For attempt As Integer = 0 To maxRetries
            Try
                Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
                    Using sr As New StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True)
                        Dim lines As New List(Of String)
                        While Not sr.EndOfStream
                            lines.Add(sr.ReadLine())
                        End While
                        Return lines
                    End Using
                End Using
            Catch ex As IOException
                If attempt = maxRetries Then Throw
                Threading.Thread.Sleep(delayMs)
            Catch ex As UnauthorizedAccessException
                If attempt = maxRetries Then Throw
                Threading.Thread.Sleep(delayMs)
            End Try
        Next
        Return New List(Of String)
    End Function

    Private Function CleanLogPrefix(src As String) As String
        If String.IsNullOrEmpty(src) Then Return src
        Return Regex.Replace(src,
                             "^\s*\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\s+\[[A-Z]+\]\s*>?\s*",
                             "",
                             RegexOptions.IgnoreCase).Trim()
    End Function

    Private Function TryGetRulesByCategoryFromMain(ByRef trigByCat As Dictionary(Of String, List(Of Regex)),
                                                   ByRef reasByCat As Dictionary(Of String, List(Of KeyValuePair(Of Regex, String)))) As Boolean
        trigByCat = New Dictionary(Of String, List(Of Regex))(StringComparer.OrdinalIgnoreCase)
        reasByCat = New Dictionary(Of String, List(Of KeyValuePair(Of Regex, String)))(StringComparer.OrdinalIgnoreCase)
        For Each c In CATS
            trigByCat(c) = New List(Of Regex)()
            reasByCat(c) = New List(Of KeyValuePair(Of Regex, String))()
        Next

        Try
            Dim t = GetType(main)

            AddRegexListFromMain(t, trigByCat, "quest", "questFailureTriggers")
            AddRegexListFromMain(t, trigByCat, "skill", "skillFailureTriggers")
            AddRegexListFromMain(t, trigByCat, "combat", "combatFailureTriggers")
            AddRegexListFromMain(t, trigByCat, "bot", "botFailureTriggers")

            AddReasonListFromMain(t, reasByCat, "quest", "questFailureReasons")
            AddReasonListFromMain(t, reasByCat, "skill", "skillFailureReasons")
            AddReasonListFromMain(t, reasByCat, "combat", "combatFailureReasons")
            AddReasonListFromMain(t, reasByCat, "bot", "botFailureReasons")

            For Each c In CATS
                If trigByCat(c).Count > 0 Then Return True
            Next
            Return False
        Catch
            Return False
        End Try
    End Function

    Private Shared Sub AddRegexListFromMain(t As Type,
                                            dict As Dictionary(Of String, List(Of Regex)),
                                            cat As String,
                                            fieldName As String)
        Dim f = t.GetField(fieldName, Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static)
        If f Is Nothing Then Exit Sub
        Dim v = TryCast(f.GetValue(Nothing), List(Of Regex))
        If v IsNot Nothing Then dict(cat).AddRange(v)
    End Sub

    Private Shared Sub AddReasonListFromMain(t As Type,
                                             dict As Dictionary(Of String, List(Of KeyValuePair(Of Regex, String))),
                                             cat As String,
                                             fieldName As String)
        Dim f = t.GetField(fieldName, Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static)
        If f Is Nothing Then Exit Sub
        Dim v = TryCast(f.GetValue(Nothing), List(Of KeyValuePair(Of Regex, String)))
        If v IsNot Nothing Then dict(cat).AddRange(v)
    End Sub

    Private Function GetTriggerCategory(line As String,
                                        trigByCat As Dictionary(Of String, List(Of Regex))) As String
        For Each c In CATS
            Dim list = trigByCat(c)
            If list Is Nothing OrElse list.Count = 0 Then Continue For
            For Each rx In list
                If rx IsNot Nothing AndAlso rx.IsMatch(line) Then
                    Return c
                End If
            Next
        Next
        Return Nothing
    End Function

    Private Function TryFindReasonAround(lines As List(Of String),
                                         lockIdx As Integer,
                                         catReasons As List(Of KeyValuePair(Of Regex, String)),
                                         window As Integer,
                                         ByRef reasonOut As String,
                                         ByRef extraOut As String,
                                         ByRef reasonLineIdx As Integer) As Boolean
        reasonOut = Nothing
        extraOut = Nothing
        reasonLineIdx = -1
        If catReasons Is Nothing OrElse catReasons.Count = 0 Then Return False

        Dim upStart As Integer = Math.Max(0, lockIdx - window)
        Dim downEnd As Integer = Math.Min(lines.Count - 1, lockIdx + window)

        For i As Integer = lockIdx - 1 To upStart Step -1
            If TryMatchReasonLine(lines(i), catReasons, reasonOut, extraOut) Then
                reasonLineIdx = i
                Return True
            End If
        Next
        For i As Integer = lockIdx + 1 To downEnd
            If TryMatchReasonLine(lines(i), catReasons, reasonOut, extraOut) Then
                reasonLineIdx = i
                Return True
            End If
        Next

        Return False
    End Function

    Private Function TryMatchReasonLine(line As String,
                                        reasons As List(Of KeyValuePair(Of Regex, String)),
                                        ByRef reasonOut As String,
                                        ByRef extraOut As String) As Boolean
        For Each kv In reasons
            Dim rx = kv.Key
            Dim friendly = kv.Value
            If rx Is Nothing Then Continue For

            Dim m = rx.Match(line)
            If m.Success Then
                reasonOut = If(String.IsNullOrWhiteSpace(friendly), rx.ToString(), friendly)
                Dim extraMatch = Regex.Match(line, "\[(.*?)\]")
                extraOut = If(extraMatch.Success, extraMatch.Groups(1).Value, Nothing)
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub InitializeComponent()
    End Sub

End Class

Public Class MaterialTreeView
    Inherits TreeView

    Private ReadOnly _bg As Color = Color.FromArgb(33, 33, 33)
    Private ReadOnly _bgAlt As Color = Color.FromArgb(38, 38, 38)
    Private ReadOnly _fg As Color = Color.White
    Private ReadOnly _muted As Color = Color.FromArgb(200, 200, 200)
    Private ReadOnly _accent As Color = Color.FromArgb(129, 212, 250)
    Private ReadOnly _selBg As Color = Color.FromArgb(55, 71, 79)
    Private ReadOnly _border As Color = Color.FromArgb(60, 60, 60)

    Public Sub New()
        MyBase.New()
        Me.DrawMode = TreeViewDrawMode.OwnerDrawText
        Me.BorderStyle = BorderStyle.None
        Me.FullRowSelect = True
        Me.ShowLines = False
        Me.HideSelection = False
        Me.Indent = 18
        Me.ItemHeight = 28
        Me.BackColor = _bg
        Me.ForeColor = _fg
        Me.Scrollable = True
    End Sub

    Protected Overrides Sub OnDrawNode(e As DrawTreeNodeEventArgs)
        e.DrawDefault = False

        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim bounds = e.Bounds
        bounds.Inflate(-2, 0)

        Dim level = e.Node.Level
        Dim isSelected = (e.State And TreeNodeStates.Selected) = TreeNodeStates.Selected
        Dim rowBack = If((e.Node.Index Mod 2 = 0), _bg, _bgAlt)

        Using backBrush As New SolidBrush(If(isSelected, _selBg, rowBack))
            g.FillRectangle(backBrush, New Rectangle(0, bounds.Top, Me.ClientSize.Width, bounds.Height))
        End Using

        Dim xLeft As Integer = 8 + level * Me.Indent
        Dim yMid As Integer = bounds.Top + bounds.Height \ 2
        Using guidePen As New Pen(_border)
            g.DrawLine(guidePen, xLeft - 10, yMid, xLeft - 4, yMid)
        End Using

        Dim iconX As Integer = Math.Max(2, xLeft - 6)
        Dim iconY As Integer = bounds.Top + (bounds.Height - 16) \ 2
        If Me.ImageList IsNot Nothing Then
            Dim img As Image = Nothing
            If Not String.IsNullOrEmpty(e.Node.ImageKey) AndAlso Me.ImageList.Images.ContainsKey(e.Node.ImageKey) Then
                img = Me.ImageList.Images(e.Node.ImageKey)
            End If
            If img IsNot Nothing Then
                g.DrawImage(img, iconX, iconY, 16, 16)
            End If
        End If

        Dim text As String = e.Node.Text
        Dim font As New Font(Me.Font, If(level = 0, FontStyle.Bold, FontStyle.Regular))
        Dim color As Color = If(level = 0, _accent, If(level = 1, _fg, _muted))

        Using txtBrush As New SolidBrush(If(isSelected, _fg, color))
            Dim textX As Integer = xLeft + 10
            g.DrawString(text, font, txtBrush, textX, bounds.Top + 5)
        End Using

        Using p As New Pen(_border)
            g.DrawLine(p, 0, bounds.Bottom - 1, Me.ClientSize.Width, bounds.Bottom - 1)
        End Using
    End Sub

    Protected Overrides Sub OnAfterExpand(e As TreeViewEventArgs)
        MyBase.OnAfterExpand(e)
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnAfterCollapse(e As TreeViewEventArgs)
        MyBase.OnAfterCollapse(e)
        Me.Invalidate()
    End Sub
End Class

Public Class AccentProgressBar
    Inherits MaterialSkin.Controls.MaterialProgressBar

    Public Property AccentColor As Color = Color.FromArgb(129, 212, 250)
    Public Property TrackColor As Color = Color.FromArgb(60, 60, 60)

    Public Sub New()
        MyBase.New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
        DoubleBuffered = True
        Me.Height = 6
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using track As New SolidBrush(TrackColor)
            e.Graphics.FillRectangle(track, Me.ClientRectangle)
        End Using
        Dim frac As Single = 0F
        If Me.Maximum > 0 Then frac = CSng(Me.Value) / CSng(Me.Maximum)
        Dim fillW As Integer = CInt(Me.ClientSize.Width * Math.Min(1.0F, Math.Max(0.0F, frac)))
        If fillW > 0 Then
            Using fill As New SolidBrush(AccentColor)
                e.Graphics.FillRectangle(fill, 0, 0, fillW, Me.ClientSize.Height)
            End Using
        End If
    End Sub
End Class
