Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks
Imports MaterialSkin
Imports MaterialSkin.Controls
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Http
Imports System.Reflection

Public Class Form1
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
    Private COLOR_DEFAULT As Integer = &H7289DA 'not really used anymore since adding user defined embeds, remove before release
    Private COLOR_QUEST As Integer = &HFFD700 'not really used anymore since adding user defined embeds, remove before release
    Private monitorChat As Boolean
    Private monitorQuests As Boolean
    Private takeScreenshots As Boolean
    Private autoCleanup As Boolean
    Private checkInterval As Integer
    Private logTimer As System.Threading.Timer
    Private lastFile As String = Nothing
    Private newLinesSinceLastTick As Boolean = False

    Private questFailureTriggers As New List(Of Regex)
    Private questFailureReasons As New List(Of KeyValuePair(Of Regex, String))

    Private skillFailureTriggers As New List(Of Regex)
    Private skillFailureReasons As New List(Of KeyValuePair(Of Regex, String))

    Private combatFailureTriggers As New List(Of Regex)
    Private combatFailureReasons As New List(Of KeyValuePair(Of Regex, String))

    Private Sub addFailRule(category As String, pattern As String, Optional friendlyText As String = Nothing, Optional isTrigger As Boolean = True)

        Dim regex As New Regex(Regex.Escape(pattern), RegexOptions.IgnoreCase)

        Select Case category.ToLower()
            Case "quest"
                If isTrigger Then
                    questFailureTriggers.Add(regex)
                Else
                    questFailureReasons.Add(New KeyValuePair(Of Regex, String)(regex, friendlyText))
                End If

            Case "skill"
                If isTrigger Then
                    skillFailureTriggers.Add(regex)
                Else
                    skillFailureReasons.Add(New KeyValuePair(Of Regex, String)(regex, friendlyText))
                End If

            Case "combat"
                If isTrigger Then
                    combatFailureTriggers.Add(regex)
                Else
                    combatFailureReasons.Add(New KeyValuePair(Of Regex, String)(regex, friendlyText))
                End If
        End Select
    End Sub


    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
    End Function




    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowRect(hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function


    <DllImport("user32.dll")>
    Private Shared Function IsIconic(hWnd As IntPtr) As Boolean
    End Function



    <DllImport("user32.dll")>
    Private Shared Function ShowWindow(hWnd As IntPtr, nCmdShow As Integer) As Boolean
    End Function




    <DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function EnumWindows(lpEnumFunc As EnumWindowsProc, lParam As IntPtr) As Boolean
    End Function




    Public Delegate Function EnumWindowsProc(hWnd As IntPtr, lParam As IntPtr) As Boolean

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function GetWindowTextLength(hWnd As IntPtr) As Integer
    End Function


    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function GetWindowText(hWnd As IntPtr, lpString As System.Text.StringBuilder, nMaxCount As Integer) As Integer
    End Function


    <StructLayout(LayoutKind.Sequential)>
    Public Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure
    Private Function GetFolderName(filePath As String) As String

        Dim dirPath As String = System.IO.Path.GetDirectoryName(filePath)
        If String.IsNullOrWhiteSpace(dirPath) Then Return ""
        Return New System.IO.DirectoryInfo(dirPath).Name
    End Function


    Private Const SW_RESTORE As Integer = 9
    Private Function PickBotWindow(folderName As String) As IntPtr
        If String.IsNullOrWhiteSpace(folderName) Then Return IntPtr.Zero
        Dim result As IntPtr = IntPtr.Zero
        Dim needle As String = folderName.Trim()
        EnumWindows(Function(h, p)
                        Dim len = GetWindowTextLength(h)
                        If len > 0 Then
                            Dim sb As New System.Text.StringBuilder(len + 1)
                            GetWindowText(h, sb, sb.Capacity)
                            Dim title = sb.ToString()
                            If title.IndexOf("DreamBot", StringComparison.OrdinalIgnoreCase) >= 0 AndAlso title.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0 Then
                                result = h
                                Return False
                            End If
                        End If

                        Return True
                    End Function, IntPtr.Zero)
        Return result
    End Function

    Function FindByTitleChunk(titleContains As String) As IntPtr
        Dim result As IntPtr = IntPtr.Zero
        EnumWindows(Function(h, p)
                        Dim len = GetWindowTextLength(h)
                        If len > 0 Then
                            Dim sb As New System.Text.StringBuilder(len + 1)
                            GetWindowText(h, sb, sb.Capacity)
                            If sb.ToString().IndexOf(titleContains, StringComparison.OrdinalIgnoreCase) >= 0 Then
                                result = h
                                Return False
                            End If
                        End If

                        Return True
                    End Function, IntPtr.Zero)
        Return result
    End Function




    Private Function IsJson(json As String, ByRef errorMessage As String) As Boolean
        Try
            JToken.Parse(json)
            Return True
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function


    Private Sub ChooseLogFolders(sender As Object, e As EventArgs) Handles btnBrowseLogDir.Click
        Using fbd As New FolderBrowserDialog()
            fbd.Description = "Select one or more log folders (choose one at a time)"
            If fbd.ShowDialog() = DialogResult.OK Then
                If String.IsNullOrWhiteSpace(txtLogDir.Text) Then
                    txtLogDir.Text = fbd.SelectedPath
                Else
                    txtLogDir.Text &= ";" & fbd.SelectedPath
                End If
            End If
        End Using
    End Sub





    Private Async Sub OnStartup(sender As Object, e As EventArgs) Handles MyBase.Load
        txtWebhook.Text = My.Settings.WebhookURL
        txtMention.Text = My.Settings.MentionID
        txtLogDir.Text = My.Settings.LogFolderPath
        chatID.Text = My.Settings.ChatID
        questID.Text = My.Settings.QuestID
        errorID.Text = My.Settings.ErrorID
        numIntervalSecond.Value = My.Settings.CheckInterval

        numIntervalSecond.Value = If(My.Settings.CheckInterval > 0, My.Settings.CheckInterval, 5)
        DarkModeEnabled.Checked = My.Settings.DarkModeOn
        Dim SkinManager As MaterialSkinManager = MaterialSkinManager.Instance
        SkinManager.AddFormToManage(Me)
        ApplyTheme(DarkModeEnabled.Checked)

        combatError.Visible = False
        questError.Visible = False
        skillIssue.Visible = False
        chatErrors.Visible = False
        txtLog.Font = robotoFont

        If Not String.IsNullOrWhiteSpace(My.Settings.ChatEmbedSet) Then
            chatEmbed.Text = My.Settings.ChatEmbedSet
        Else
            resetChat.PerformClick()
        End If

        If Not String.IsNullOrWhiteSpace(My.Settings.ErrorEmbedSet) Then
            errorEmbed.Text = My.Settings.ErrorEmbedSet
        Else
            ResetError.PerformClick()
        End If
        'possibly move failure rules to a more permanent solution in the future
        Await FetchFailRules()
    End Sub




    Private Sub ToggleErrorPanel(sender As Object, e As EventArgs) Handles monitorError.CheckedChanged

        Dim show As Boolean = monitorError.Checked
        combatError.Visible = show
        questError.Visible = show
        skillIssue.Visible = show
        chatErrors.Visible = show
    End Sub



    Private Sub ToggleDarkMode(sender As Object, e As EventArgs) Handles DarkModeEnabled.CheckedChanged

        ApplyTheme(DarkModeEnabled.Checked)
        My.Settings.DarkModeOn = DarkModeEnabled.Checked
        My.Settings.Save()
    End Sub



    Private Sub ApplyTheme(enableDark As Boolean)

        Dim SkinManager As MaterialSkinManager = MaterialSkinManager.Instance
        If enableDark Then
            SkinManager.Theme = MaterialSkinManager.Themes.DARK
            SkinManager.ColorScheme = New ColorScheme(Primary.Grey800, Primary.Grey900, Primary.Grey500, Accent.LightBlue200, TextShade.WHITE)
        Else
            SkinManager.Theme = MaterialSkinManager.Themes.LIGHT
            SkinManager.ColorScheme = New ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE)
        End If

    End Sub


    Private Sub AppendLog(msg As String)

        If txtLog.InvokeRequired Then
            txtLog.Invoke(New Action(Of String)(AddressOf AppendLog), msg)
        Else
            With txtLog
                .SelectionFont = robotoFont
                .AppendText("[" & DateTime.Now.ToString("HH:mm:ss") & "] " & msg & Environment.NewLine)
                .SelectionStart = .TextLength
                .ScrollToCaret()
            End With
        End If
    End Sub



    Private Sub HeartbeatTick(state As Object)

        If Not monitoring Then Return

        Dim latest = GetLatestLogFile()
        If String.IsNullOrWhiteSpace(latest) Then
            AppendLog("No log files found.")
            Return
        End If

        If lastFile Is Nothing OrElse latest <> lastFile Then
            AppendLog("📄 New log file detected: " & System.IO.Path.GetFileName(latest))
            lastFile = latest
            JumpToEnd(latest)
        End If

        If Not newLinesSinceLastTick Then
            AppendLog($"No new entries. (interval {checkInterval} sec)")
        End If

        newLinesSinceLastTick = False
    End Sub




    Private watchers As New List(Of FileSystemWatcher)


    Private Sub JumpToEnd(path As String)
        Try
            lastOffsets(path) = New FileInfo(path).Length
            lastProcessedTimes(path) = DateTime.Now
        Catch
        End Try
    End Sub

    Private Sub StartMonitoring(sender As Object, e As EventArgs) Handles btnStart.Click

        If monitoring Then
            AppendLog("⚠ Already running.")
            Return
        End If


        My.Settings.WebhookURL = txtWebhook.Text.Trim()
        My.Settings.ChatID = chatID.Text.Trim()
        My.Settings.QuestID = questID.Text.Trim()
        My.Settings.ErrorID = errorID.Text.Trim()
        My.Settings.LogFolderPath = txtLogDir.Text.Trim()
        My.Settings.CheckInterval = numIntervalSecond.Value
        My.Settings.DarkModeOn = DarkModeEnabled.Checked
        My.Settings.Save()

        WEBHOOK_URL = My.Settings.WebhookURL
        DISCORD_MENTION = txtMention.Text.Trim()
        My.Settings.MentionID = DISCORD_MENTION
        LOG_DIR = My.Settings.LogFolderPath
        CHAT_CHANNEL = My.Settings.ChatID
        ERROR_CHANNEL = My.Settings.ErrorID
        QUEST_CHANNEL = My.Settings.QuestID
        checkInterval = CInt(numIntervalSecond.Value)

        monitorChat = chkMonitorChat.Checked
        monitorQuests = monitorQuest.Checked
        takeScreenshots = captureWin.Checked
        autoCleanup = autoClean.Checked

        If monitorChat Then
            If String.IsNullOrWhiteSpace(CHAT_CHANNEL) OrElse Not CHAT_CHANNEL.StartsWith("http") Then
                AppendLog("⚠ Chat Webhook empty, defaulting to Discord Webhook.")
                CHAT_CHANNEL = WEBHOOK_URL
            End If
        End If

        If monitorQuests Then
            If String.IsNullOrWhiteSpace(QUEST_CHANNEL) OrElse Not QUEST_CHANNEL.StartsWith("http") Then
                AppendLog("⚠ Quest Webhook empty, defaulting to Discord Webhook.")
                QUEST_CHANNEL = WEBHOOK_URL
            End If

        End If

        If questError.Checked Or skillIssue.Checked Or combatError.Checked Then
            If String.IsNullOrWhiteSpace(ERROR_CHANNEL) OrElse Not ERROR_CHANNEL.StartsWith("http") Then
                AppendLog("⚠ Error Webhook empty, defaulting to Discord Webhook.")
                ERROR_CHANNEL = WEBHOOK_URL
            End If
        End If

        If String.IsNullOrWhiteSpace(WEBHOOK_URL) OrElse Not WEBHOOK_URL.StartsWith("http") Then
            AppendLog("⚠ Please enter a valid Discord Webhook URL.")
            Return
        End If
        If Not monitorChat AndAlso Not monitorQuests AndAlso Not questError.Checked AndAlso Not skillIssue.Checked AndAlso Not combatError.Checked Then
            AppendLog("⚠ No monitoring options selected.")
            Return
        End If

        Dim logDirs = txtLogDir.Text.Split(";"c).Select(Function(p) p.Trim()).Where(Function(p) Directory.Exists(p)).ToList()
        

        monitoring = True
        watchers.Clear()



        For Each folder In logDirs
            For Each f In Directory.GetFiles(folder, "logfile-*.log", SearchOption.TopDirectoryOnly)
                JumpToEnd(f)
            Next
        Next
        lastFile = GetLatestLogFile()
        For Each folder In logDirs
            Dim watcher As New FileSystemWatcher(folder, "logfile-*.log")
            watcher.NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.Size
            watcher.IncludeSubdirectories = False
            AddHandler watcher.Changed, AddressOf OnLogChanged
            AddHandler watcher.Created, AddressOf OnLogChanged
            watcher.SynchronizingObject = Me
            watcher.EnableRaisingEvents = True

            watchers.Add(watcher)
        Next

        logTimer = New System.Threading.Timer(AddressOf HeartbeatTick, Nothing, checkInterval * 1000, checkInterval * 1000) 'broken
        AppendLog($"▶ monitoring started in {logDirs.Count} folder(s).")
    End Sub



    Private Sub StopMonitoring(sender As Object, e As EventArgs) Handles btnStop.Click

        For Each watcher In watchers
            watcher.EnableRaisingEvents = False
            RemoveHandler watcher.Changed, AddressOf OnLogChanged
            RemoveHandler watcher.Created, AddressOf OnLogChanged
            watcher.Dispose()
        Next
        watchers.Clear()

        If logTimer IsNot Nothing Then
            logTimer.Dispose()
            logTimer = Nothing
        End If

        monitoring = False
        AppendLog("⏹ Monitoring stopped.")
    End Sub




    Private Async Sub OnLogChanged(sender As Object, e As FileSystemEventArgs)
        If Not monitoring Then Return
        Try
            Dim path As String = e.FullPath
            If Not File.Exists(path) Then Return

            If Not lastOffsets.ContainsKey(path) Then
                JumpToEnd(path)
                Return
            End If

            If e.ChangeType = WatcherChangeTypes.Created Then
                JumpToEnd(path)
                Return
            End If


            Dim offset As Long = If(lastOffsets.ContainsKey(path), lastOffsets(path), 0L)
            Dim newLines As List(Of String) = TailReadNewLines(path, offset)
            lastOffsets(path) = offset
            If newLines.Count = 0 Then Return

            Dim cutoff As DateTime? = If(lastProcessedTimes.ContainsKey(path), lastProcessedTimes(path), Nothing)
            Dim onlyNew = New List(Of String)
            For Each line In newLines
                Dim ts = ParseLogDate(line)
                If ts.HasValue AndAlso (cutoff Is Nothing OrElse ts > cutoff) Then
                    onlyNew.Add(line)
                End If
            Next
            If onlyNew.Count = 0 Then Return

            If monitorChat Then
                Dim chatSegments = SliceChatSegments(onlyNew)
                If chatSegments.Count > 0 Then
                    AppendLog($"📨 Found {chatSegments.Count} chat event(s)")
                    If takeScreenshots Then Await SnapAndSend(path)
                    Await SendSegments(chatSegments, path, "P2P Chat Event", COLOR_DEFAULT)
                End If
            End If

            If monitorQuests Then
                Dim questSegments = SliceQuests(onlyNew)
                If questSegments.Count > 0 Then
                    AppendLog($"🏆 Found {questSegments.Count} quest(s)")
                End If
            End If


            If questError.Checked Then
                Dim questFailures = ScanFailures(onlyNew, questFailureTriggers, questFailureReasons, "Quest")
                For Each failure In questFailures
                    AppendLog($"❌ Quest Failure detected: {failure.Trigger} / {failure.Reason}")
                    Await PostFailAlert(failure.Trigger, failure.Reason, path, "Quest")
                Next
            End If

            If skillIssue.Checked Then
                Dim skillFailures = ScanFailures(onlyNew, skillFailureTriggers, skillFailureReasons, "Skill")
                For Each failure In skillFailures
                    AppendLog($"❌ Skill Failure detected: {failure.Trigger} / {failure.Reason}")
                    Await PostFailAlert(failure.Trigger, failure.Reason, path, "Skill")
                Next
            End If

            If combatError.Checked Then
                Dim combatFailures = ScanFailures(onlyNew, combatFailureTriggers, combatFailureReasons, "Combat")
                For Each failure In combatFailures
                    AppendLog($"❌ Combat Failure detected: {failure.Trigger} / {failure.Reason}")
                    Await PostFailAlert(failure.Trigger, failure.Reason, path, "Combat")
                Next
            End If

            Dim maxTs = onlyNew.Select(Function(line) ParseLogDate(line)).Where(Function(ts) ts.HasValue).Max()
            If maxTs.HasValue Then lastProcessedTimes(path) = maxTs

        Catch ex As Exception
            AppendLog("Watcher error: " & ex.Message)
        End Try
        newLinesSinceLastTick = True
    End Sub


    Private Function ScanFailures(lines As List(Of String), triggers As List(Of Regex), reasons As List(Of KeyValuePair(Of Regex, String)), failureType As String) As List(Of (Trigger As String, Reason As String))

        Dim results As New List(Of (Trigger As String, Reason As String))

        For i = 0 To lines.Count - 1
            Dim currentLine = lines(i)

            If triggers.Any(Function(rx) rx.IsMatch(currentLine)) Then
                Dim matchedReason As String

                Dim startIdx = Math.Max(0, i - 10)
                Dim endIdx = Math.Min(lines.Count - 1, i + 10)

                For j = startIdx To endIdx
                    For Each r In reasons
                        If r.Key.IsMatch(lines(j)) Then
                            matchedReason = r.Value
                            Exit For
                        End If
                    Next
                    If matchedReason IsNot Nothing Then Exit For
                Next

                If String.IsNullOrWhiteSpace(matchedReason) Then
                    matchedReason = "Unknown reason, please send logs to CaS5"
                End If

                results.Add((currentLine, matchedReason))
            End If
        Next

        Return results
    End Function




    Private Function JsonSafe(s As String) As String
        If s Is Nothing Then Return ""
        Dim t = s.Replace("\", "\\").Replace("""", "\""")
        t = t.Replace(vbCrLf, "\n").Replace(vbCr, "\n").Replace(vbLf, "\n").Replace(vbTab, "\t")
        Return t
    End Function



    Private Async Function PostFailAlert(trigger As String, reason As String, filePath As String, FailureType As String) As Task

        Dim targetWebhook As String = ERROR_CHANNEL
        Dim payload As String = errorEmbed.Text.Trim()
        If String.IsNullOrWhiteSpace(payload) Then
            AppendLog("⚠ Error embed textbox is empty, cannot send embed.")
            Return
        End If


        Dim filename As String = System.IO.Path.GetFileName(filePath)

        payload = payload _
        .Replace("{mention}", JsonSafe(DISCORD_MENTION)) _
        .Replace("{type}", JsonSafe(FailureType)) _
        .Replace("{trigger}", JsonSafe(trigger)) _
        .Replace("{reason}", JsonSafe(reason)) _
        .Replace("{filename}", JsonSafe(filename)) _
        .Replace("{folder}", JsonSafe(GetFolderName(filePath))) _
        .Replace("{time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

        Try
            Await PostJson(targetWebhook, payload)
            AppendLog($"🚨 {FailureType} Failure reported to Discord.")
        Catch ex As Exception
            AppendLog($"{FailureType} Failure webhook error: " & ex.Message)
        End Try
    End Function


    Private Function TailReadNewLines(path As String, ByRef offset As Long) As List(Of String)

        Dim result As New List(Of String)

        Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            fs.Seek(offset, SeekOrigin.Begin)

            Using sr As New StreamReader(fs)
                While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()
                    result.Add(line)
                End While

                offset = fs.Position
            End Using
        End Using

        Return result
    End Function



    Private Async Function PostJson(url As String, json As String) As Task

        Using content As New Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
            Dim resp = Await http.PostAsync(url, content)
            resp.EnsureSuccessStatusCode()
        End Using
    End Function



    Private Async Function UploadFile(url As String, filePath As String, Optional fieldName As String = "file", Optional fields As Dictionary(Of String, String) = Nothing) As Task

        Using form As New Net.Http.MultipartFormDataContent()
            If fields IsNot Nothing Then
                For Each kv In fields
                    form.Add(New Net.Http.StringContent(kv.Value), kv.Key)
                Next
            End If

            Using fs As New IO.FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                form.Add(New Net.Http.StreamContent(fs), fieldName, IO.Path.GetFileName(filePath))
                Dim resp = Await http.PostAsync(url, form)
                resp.EnsureSuccessStatusCode()
            End Using
        End Using
    End Function




    Private Async Function FetchText(url As String) As Task(Of String)

        Return Await http.GetStringAsync(url)
    End Function




    Private Function GetLatestLogFile() As String

        If String.IsNullOrWhiteSpace(LOG_DIR) Then
            Return Nothing
        End If

        Dim logDirs As New List(Of String)()
        For Each part As String In LOG_DIR.Split(";"c)
            Dim dir As String = part.Trim()
            If dir <> "" Then
                logDirs.Add(dir)
            End If
        Next

        Dim allLogs As New List(Of String)()
        For Each dir As String In logDirs
            Try
                If Directory.Exists(dir) Then
                    Dim files() As String = Directory.GetFiles(dir, "logfile-*.log", SearchOption.TopDirectoryOnly)
                    allLogs.AddRange(files)
                End If
            Catch
            End Try
        Next
        If allLogs.Count = 0 Then
            Return Nothing
        End If
        Dim latestPath As String
        Dim latestStamp As DateTime = DateTime.MinValue

        For Each filePath As String In allLogs
            Try
                Dim stamp As DateTime = File.GetLastWriteTimeUtc(filePath)
                If (stamp > latestStamp) OrElse (stamp = latestStamp AndAlso String.CompareOrdinal(filePath, latestPath) > 0) Then
                    latestStamp = stamp
                    latestPath = filePath
                End If
            Catch

            End Try
        Next

        Return latestPath
    End Function




    Private Async Function FetchFailRules() As Task

        Dim url = "https://docs.google.com/spreadsheets/d/1kLmq1Fj2OaT7BMQEF1N1dZq7Z-SIbhAFaPLJtOhGczE/export?format=csv"
        Try
            Dim csvData = Await FetchText(url)
            Dim lines = csvData.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

            questFailureTriggers.Clear()
            questFailureReasons.Clear()
            skillFailureTriggers.Clear()
            skillFailureReasons.Clear()
            combatFailureTriggers.Clear()
            combatFailureReasons.Clear()

            For i = 1 To lines.Length - 1
                Dim parts = lines(i).Split(","c)
                If parts.Length < 3 Then Continue For

                Dim category = parts(0).Trim().ToLower()
                Dim pattern = parts(1).Trim()
                Dim entryType = parts(2).Trim().ToLower()
                Dim friendly = If(parts.Length > 3, parts(3).Trim(), "")

                Select Case category
                    Case "quest", "skill", "combat"
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


    Private Function ParseLogDate(line As String) As DateTime?

        Try
            Return DateTime.ParseExact(line.Substring(0, 19), "yyyy-MM-dd HH:mm:ss", Nothing)
        Catch
            Return Nothing
        End Try
    End Function


    Private Function SliceChatSegments(lines As IEnumerable(Of String)) As List(Of List(Of String))

        Dim segments As New List(Of List(Of String))()
        Dim current As New List(Of String)()
        For Each line In lines
            Dim upper = line.ToUpper()
            If upper.Contains("CHAT") AndAlso current.Count = 0 Then
                current.Add(line)
            ElseIf (upper.Contains("SLOWLY TYPING RESPONSE") OrElse upper.Contains("BAD RESPONSE")) AndAlso current.Count > 0 Then
                current.Add(line)
                segments.Add(New List(Of String)(current))
                current.Clear()
            ElseIf current.Count > 0 Then
                current.Add(line)
            End If
        Next

        Return segments
    End Function


    Private Function SliceQuests(lines As IEnumerable(Of String)) As List(Of List(Of String))

        Dim quests As New List(Of List(Of String))()
        For Each line In lines
            If line.Contains("Congratulations, you've completed a quest") Then
                Dim cleaned = Regex.Replace(line, "<col=.*?>(.*?)</col>", "$1")
                quests.Add(New List(Of String) From {cleaned})
            End If
        Next
        Return quests
    End Function



    Private Async Function SendSegments(segments As List(Of List(Of String)), filePath As String, embedTitle As String, embedColor As Integer) As Task

        Dim targetWebhook As String
        If embedTitle.Contains("Chat") Then
            targetWebhook = CHAT_CHANNEL
        ElseIf embedTitle.Contains("Quest") Then
            targetWebhook = QUEST_CHANNEL
        Else
            targetWebhook = WEBHOOK_URL
        End If


        If String.IsNullOrWhiteSpace(chatEmbed.Text) Then
            AppendLog("⚠ Chat/Quest embed textbox is empty, cannot send embed.")
            Return
        End If

        Dim filename As String = System.IO.Path.GetFileName(filePath)

        For segIdx = 0 To segments.Count - 1
            Dim joined = String.Join(Environment.NewLine, segments(segIdx))
            Dim segText = "```" & joined.Substring(0, Math.Min(1000, joined.Length)) & "```"

            Dim payload As String = chatEmbed.Text.Trim()
            payload = payload _
                .Replace("{mention}", JsonSafe(DISCORD_MENTION)) _
                .Replace("{type}", JsonSafe(embedTitle)) _
                .Replace("{segment}", JsonSafe(segText)) _
                .Replace("{filename}", JsonSafe(filename)) _
                .Replace("{folder}", JsonSafe(GetFolderName(filePath))) _
                .Replace("{time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) _
                .Replace("{index}", (segIdx + 1).ToString())

            If String.IsNullOrWhiteSpace(DISCORD_MENTION) Then
                payload = payload.Replace("<@>", "")
            End If

            Dim validationError As String = Nothing
            If Not IsJson(payload, validationError) Then
                AppendLog($"❌ Invalid JSON for Chat/Quest embed: {validationError}")
                Continue For
            End If

            Try
                Await PostJson(targetWebhook, payload)
                AppendLog($"✅ {embedTitle} embed sent (Segment {segIdx + 1}).")
            Catch ex As Exception
                AppendLog($"❌ Failed to send {embedTitle} embed: " & ex.Message)
            End Try
        Next
    End Function





    Private Async Function SnapAndSend(sourceLogPath As String) As Task

        Try
            Dim targetWebhook As String
            If String.IsNullOrWhiteSpace(CHAT_CHANNEL) OrElse Not CHAT_CHANNEL.StartsWith("http") Then
                AppendLog("Chat Webhook empty, defaulting to Discord Webhook.")
                targetWebhook = WEBHOOK_URL
            Else
                targetWebhook = CHAT_CHANNEL
            End If

            Dim hWnd = PickBotWindow(GetFolderName(sourceLogPath))
            If hWnd = IntPtr.Zero Then
                AppendLog("DreamBot window not found.")
                Return
            End If

            ShowWindow(hWnd, SW_RESTORE)
            SetForegroundWindow(hWnd)

            Dim rect As RECT
            If Not GetWindowRect(hWnd, rect) Then
                AppendLog("Failed to get window rect.")
                Return
            End If

            Dim width As Integer = rect.Right - rect.Left
            Dim height As Integer = rect.Bottom - rect.Top

            Using bmp As New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, New Size(width, height), CopyPixelOperation.SourceCopy)
                End Using

                Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                Dim filePath As String = $"dreambot_screenshot_{timestamp}.png"
                bmp.Save(filePath, Imaging.ImageFormat.Png)

                Await UploadFile(targetWebhook, filePath)

                AppendLog("📸 Screenshot sent.")

                If autoCleanup AndAlso File.Exists(filePath) Then
                    File.Delete(filePath)
                    AppendLog("Deleted screenshot: " & filePath)
                End If
            End Using

        Catch ex As Exception
            AppendLog("Error screenshot: " & ex.Message)
        End Try
    End Function


    Private Sub ToggleSecretMode(sender As Object, e As EventArgs) Handles screenshotmode.CheckedChanged

        Dim mask As Boolean = screenshotmode.Checked

        chatID.Password = False
        questID.Password = False
        errorID.Password = False
        txtWebhook.Password = False
        txtMention.Password = False
        txtLogDir.Password = False
        If screenshotmode.Checked Then
            chatID.Password = True
            questID.Password = True
            errorID.Password = True
            txtWebhook.Password = True
            txtMention.Password = True
            txtLogDir.Password = True
        End If
    End Sub


    Private Sub CommonButtons(sender As Object, e As EventArgs) Handles clearBtn.Click, forcecheckBtn.Click, wikiBtn.Click, p2pdiscordBtn.Click, dbdiscordBtn.Click, dbforumBtn.Click, p2psalesBtn.Click, p2psetupBtn.Click, p2psurvivalBtn.Click, p2pgearBtn.Click

        If sender Is clearBtn Then
            txtLog.Clear()
            Exit Sub
        End If


        If sender Is forcecheckBtn Then
            If monitoring Then
                AppendLog("Forcing check heartbeat..")
                HeartbeatTick(Nothing)
            Else
                AppendLog("⚠ Monitorin is not running, cannot force check.")
            End If
            Exit Sub
        End If


        Dim url As String
        Dim failMsg As String = ""

        Select Case True
            Case sender Is wikiBtn
                url = "https://aeglen.wiki.gg"
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
                url = "https://aeglen.wiki.gg/wiki/Getting_Started"
                failMsg = "Failed to open P2P setup page: "
            Case sender Is p2psurvivalBtn
                url = "https://docs.google.com/spreadsheets/d/1G03zGPeqEc3jOrmGUeSccNbuoVpFCzMjC4R-Sy0lVo8/edit?usp=sharing"
                failMsg = "Failed to open P2P survival page: "
            Case sender Is p2pgearBtn
                url = "https://aeglen.wiki.gg/wiki/Supported_Gear"
                failMsg = "Failed to open P2P supported gear page: "
        End Select

        If Not String.IsNullOrWhiteSpace(url) Then
            Try
                Process.Start(New ProcessStartInfo(url) With {.UseShellExecute = True})
            Catch ex As Exception
                'just log whatever went wrong if something did
                AppendLog(failMsg & ex.Message)
            End Try
        End If
    End Sub



    Private Async Sub SendTestWebhooks(sender As Object, e As EventArgs) Handles testBtn.Click
        Dim webhookMap As New Dictionary(Of String, String) From {
        {"Default Webhook", txtWebhook.Text.Trim}, {"Quest Webhook", questID.Text.Trim}, {"Error Webhook", errorID.Text.Trim}, {"Chat Webhook", chatID.Text.Trim}
    }

        For Each entry In webhookMap
            Dim name = entry.Key
            Dim url = entry.Value

            If String.IsNullOrWhiteSpace(url) OrElse Not url.StartsWith("http") Then
                AppendLog($"⚠ {name} is empty or invalid, skipping.")
                Continue For
            End If

            Dim payload =
            "{" &
            $"""content"": ""<@{DISCORD_MENTION}> – Test Embed""," &
            """embeds"": [{" &
            $"""title"": ""Test Webhook Embed""," &
            $"""description"": ""This is a test embed sent at {Date.Now}.""," &
            $"""color"": {COLOR_DEFAULT}" &
            "}]}"
            Try
                Await PostJson(url, payload)
                AppendLog($"✅ Test embed sent to {name}.")
            Catch ex As Exception
                AppendLog($"❌ Failed to send test embed to {name}: {ex.Message}")
            End Try
        Next

    End Sub



    Private Async Sub TryEmbeds(sender As Object, e As EventArgs) Handles testEmbeds.Click
        Dim resolvedMention As String = txtMention.Text.Trim()
        If String.IsNullOrWhiteSpace(resolvedMention) Then
            resolvedMention = My.Settings.MentionID
        End If
        DISCORD_MENTION = resolvedMention

        Dim webhookMap As New Dictionary(Of String, String) From {
        {"Default Webhook", txtWebhook.Text.Trim()}, {"Chat Webhook", chatID.Text.Trim()}, {"Quest Webhook", questID.Text.Trim()}, {"Error Webhook", errorID.Text.Trim()}
    }
        If Not String.IsNullOrWhiteSpace(errorEmbed.Text.Trim()) Then
            For Each entry In webhookMap
                Dim name As String = entry.Key
                Dim url As String = entry.Value

                If String.IsNullOrWhiteSpace(url) OrElse Not url.StartsWith("http") Then
                    AppendLog($"⚠ {name} is empty or invalid, skipping Error embed test.")
                    Continue For
                End If

                Dim payload As String = errorEmbed.Text.Trim()

                payload = payload _
                .Replace("{mention}", resolvedMention) _
                .Replace("{type}", "Error") _
                .Replace("{reason}", "This is a test Failure reason") _
                .Replace("{filename}", "test_log.log") _
                .Replace("{time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

                If String.IsNullOrWhiteSpace(resolvedMention) Then
                    payload = payload.Replace("<@>", "")
                End If

                Dim validationError As String
                If Not IsJson(payload, validationError) Then
                    AppendLog($"❌ Invalid JSON for {name}: {validationError}")
                    Continue For
                End If


                Try
                    Await PostJson(url, payload)
                    AppendLog($"✅ Test Error embed sent to {name}.")
                Catch ex As Exception
                    AppendLog($"❌ Failed to send Test Error embed to {name}: {ex.Message}")
                End Try
            Next
        Else
            AppendLog("⚠ Error Embed textbox is empty, skipping test.")
        End If

        If Not String.IsNullOrWhiteSpace(chatEmbed.Text.Trim()) Then
            For Each entry In webhookMap
                Dim name As String = entry.Key
                Dim url As String = entry.Value
                If String.IsNullOrWhiteSpace(url) OrElse Not url.StartsWith("http") Then
                    AppendLog($"⚠ {name} is empty or invalid, skipping Chat/Quest embed test.")
                    Continue For
                End If
                Dim payload As String = chatEmbed.Text.Trim()
                Dim safeSegment As String = "```This is a test segment```".Replace(vbCr, "\n").Replace(vbLf, "\n")
                payload = payload _
                .Replace("{mention}", resolvedMention) _
                .Replace("{type}", "Chat") _
                .Replace("{segment}", safeSegment) _
                .Replace("{filename}", "test_log.log") _
                .Replace("{time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) _
                .Replace("{index}", "1")
                If String.IsNullOrWhiteSpace(resolvedMention) Then
                    payload = payload.Replace("<@>", "")
                End If
                Dim validationError As String
                If Not IsJson(payload, validationError) Then
                    AppendLog($"❌ Invalid JSON for {name}: {validationError}")
                    Continue For
                End If
                Try
                    Await PostJson(url, payload)
                    AppendLog($"✅ Test Chat/Quest embed sent to {name}.")
                Catch ex As Exception
                    AppendLog($"❌ Failed to send Test Chat/Quest embed to {name}: {ex.Message}")
                End Try
            Next
        Else
            AppendLog("⚠ Chat/Quest Embed textbox is empty, skipping test.")
        End If
    End Sub



    Private Sub ResetChatTemplate(sender As Object, e As EventArgs) Handles resetChat.Click
        Dim tmplLines As String() = {
        "{",
        "  ""content"": ""<@{mention}> – Detected {type}."", ""embeds"": [{",
        "    ""title"": ""{type}"", ""description"": ""Captured segment for file {filename}"", ""color"": 7506394, ""fields"": [",
        "      {",
        "        ""name"": ""Segment {index}"", ""value"": ""{segment}"", ""inline"": false",
        "      }",
        "    ]",
        "  }]",
        "}"
    }
        chatEmbed.Text = String.Join(vbCrLf, tmplLines)
    End Sub



    Private Sub ResetErrorTemplate(sender As Object, e As EventArgs) Handles ResetError.Click
        Dim tmplLines As String() = {
        "{",
        "  ""content"": ""<@{mention}> – {type} Failure Detected."", ""embeds"": [{",
        "    ""title"": ""{type} Failure Detected"", ""description"": ""Failure detected while reading file {filename} on account {folder}"", ""color"": 16711680, ""fields"": [",
        "      {",
        "        ""name"": ""Trigger"", ""value"": ""```{trigger}```"", ""inline"": false",
        "      },",
        "      {",
        "        ""name"": ""Reason"", ""value"": ""{reason}"", ""inline"": false",
        "      }",
        "    ]",
        "  }]",
        "}"
    }
        errorEmbed.Text = String.Join(vbCrLf, tmplLines)
    End Sub


End Class
