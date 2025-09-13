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
    Private Function GetFolderName(filePath As String) As String
        Dim dirPath As String = Path.GetDirectoryName(filePath)
        If String.IsNullOrWhiteSpace(dirPath) Then
            Return "Test Account"
        End If
        Return New DirectoryInfo(dirPath).Name
    End Function
    Private Const SW_RESTORE As Integer = 9
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
        chatEmbed.Text = My.Settings.ChatEmbedSet
        errorEmbed.Text = My.Settings.ErrorEmbedSet
        questEmbed.Text = My.Settings.QuestEmbedSet

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
    Private watchers As New List(Of FileSystemWatcher)
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
        My.Settings.ChatEmbedSet = chatEmbed.Text
        My.Settings.ErrorEmbedSet = errorEmbed.Text
        My.Settings.QuestEmbedSet = questEmbed.Text
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
                LogHelper.JumpToEnd(f, lastOffsets, lastProcessedTimes)
            Next
        Next
        Dim latest = LogHelper.GetLatestLogFile(LOG_DIR)
        For Each folder In logDirs
            Dim watcher As New FileSystemWatcher(folder, "logfile-*.log")
            watcher.NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.Size
            watcher.IncludeSubdirectories = False
            AddHandler watcher.Changed, Async Sub(s, eArgs) Await LogHelper.OnLogChanged(s, eArgs,
                monitoring, lastOffsets, lastProcessedTimes,
                monitorChat, monitorQuests, takeScreenshots,
                questError.Checked, skillIssue.Checked, combatError.Checked,
                questFailureTriggers, questFailureReasons,
                skillFailureTriggers, skillFailureReasons,
                combatFailureTriggers, combatFailureReasons,
                AddressOf AppendLog, AddressOf SendSegments,
                AddressOf PostFailAlert, AddressOf GetFolderName)

            AddHandler watcher.Created, Async Sub(s, eArgs) Await LogHelper.OnLogChanged(s, eArgs,
                monitoring, lastOffsets, lastProcessedTimes,
                monitorChat, monitorQuests, takeScreenshots,
                questError.Checked, skillIssue.Checked, combatError.Checked,
                questFailureTriggers, questFailureReasons,
                skillFailureTriggers, skillFailureReasons,
                combatFailureTriggers, combatFailureReasons,
                AddressOf AppendLog, AddressOf SendSegments,
                AddressOf PostFailAlert, AddressOf GetFolderName)
            watcher.SynchronizingObject = Me
            watcher.EnableRaisingEvents = True

            watchers.Add(watcher)
        Next

        logTimer = New System.Threading.Timer(
            Sub(state) LogHelper.HeartbeatTick(state, AddressOf AppendLog, lastFile, checkInterval, LOG_DIR, lastOffsets, lastProcessedTimes),
            Nothing, checkInterval * 1000, checkInterval * 1000)
        AppendLog($"▶ Monitoring started in {logDirs.Count} folder(s).")
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

        monitoring = False
        AppendLog("⏹ Monitoring stopped.")
    End Sub
    Private Async Function PostFailAlert(trigger As String, reason As String, filePath As String, FailureType As String) As Task
        Dim targetWebhook As String = ERROR_CHANNEL
        Dim payload As String = errorEmbed.Text.Trim()
        If String.IsNullOrWhiteSpace(payload) Then
            AppendLog("⚠ Error embed textbox is empty, cannot send embed.")
            Return
        End If

        Dim filename As String = System.IO.Path.GetFileName(filePath)
        payload = DiscordHelpers.BuildErrorPayload(payload, DISCORD_MENTION, FailureType, trigger, reason, filename, GetFolderName(filePath), DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"))

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
    Private Async Function SendSegments(segments As List(Of List(Of String)),
                                   path As String,
                                   embedTitle As String,
                                   embedColor As Integer,
                                   Optional failureTrigger As String = "",
                                   Optional failureReason As String = "") As Task

        For segIdx = 0 To segments.Count - 1
            Dim seg = segments(segIdx)

            Dim screenshotPath As String = Nothing
            Dim screenshotRef As String = ""
            If takeScreenshots Then
                screenshotPath = ScreenshotHelpers.SnapAndSend(path, GetFolderName(path), LOG_DIR)
                If Not String.IsNullOrWhiteSpace(screenshotPath) AndAlso IO.File.Exists(screenshotPath) Then
                    screenshotRef = IO.Path.GetFileName(screenshotPath)
                    AppendLog("📸 Screenshot captured.")
                Else
                    AppendLog($"⚠ Screenshot path invalid or missing: {screenshotPath}")
                    screenshotPath = Nothing
                End If
            End If

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
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                segIdx + 1
            )

            ElseIf embedTitle.Contains("Quest") Then
                Dim questText As String = String.Join(Environment.NewLine, seg)

                payload = DiscordHelpers.BuildQuestPayload(
                questEmbed.Text,
                DISCORD_MENTION,
                questText,
                IO.Path.GetFileName(path),
                GetFolderName(path),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
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
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            )
            End If

            Dim targetWebhook As String = WEBHOOK_URL
            If embedTitle.Contains("Chat") Then
                targetWebhook = If(Not String.IsNullOrWhiteSpace(CHAT_CHANNEL), CHAT_CHANNEL, WEBHOOK_URL)
            ElseIf embedTitle.Contains("Quest") Then
                targetWebhook = If(Not String.IsNullOrWhiteSpace(QUEST_CHANNEL), QUEST_CHANNEL, WEBHOOK_URL)
            ElseIf embedTitle.Contains("Error") Then
                targetWebhook = If(Not String.IsNullOrWhiteSpace(ERROR_CHANNEL), ERROR_CHANNEL, WEBHOOK_URL)
            End If

            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                If Not String.IsNullOrWhiteSpace(screenshotPath) AndAlso IO.File.Exists(screenshotPath) Then
                    Await DiscordHelpers.UploadFile(targetWebhook, screenshotPath, payload, AddressOf AppendLog)
                    If autoCleanup AndAlso Not String.IsNullOrWhiteSpace(screenshotPath) AndAlso IO.File.Exists(screenshotPath) Then
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
                LogHelper.HeartbeatTick(Nothing, AddressOf AppendLog, lastFile, checkInterval, LOG_DIR, lastOffsets, lastProcessedTimes)
            Else
                AppendLog("⚠ Monitor is not running, cannot force check.")
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
                Await DiscordHelpers.PostJson(url, payload)
                AppendLog($"✅ Test embed sent to {name}.")
            Catch ex As Exception
                AppendLog($"❌ Failed to send test embed to {name}: {ex.Message}")
            End Try
        Next

    End Sub
    Private Async Sub testEmbeds_Click(sender As Object, e As EventArgs) Handles testEmbeds.Click
        Dim filePath As String = "test_log.log"
        Dim filename As String = IO.Path.GetFileName(filePath)
        Dim FailureType As String = "Test"
        Dim trigger As String = "Simulated error trigger"
        Dim reason As String = "Simulated error reason"
        Dim chatLine As String = "Simulated chat line"
        Dim respLine As String = "Simulated response line"
        Dim screenshotRef As String = "https://i.imgur.com/fRKkKy5.png"
        Dim errorWebhook As String = errorID.Text.Trim()
        Dim chatWebhook As String = chatID.Text.Trim()
        Dim questWebhook As String = questID.Text.Trim()

        If Not String.IsNullOrWhiteSpace(errorEmbed.Text.Trim()) Then
            Dim payload = DiscordHelpers.BuildErrorPayload(
        errorEmbed.Text.Trim(),
        DISCORD_MENTION,
        FailureType,
        trigger,
        reason,
        filename,
        GetFolderName(filePath),
        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    )

            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                Await DiscordHelpers.PostJson(errorWebhook, payload)
                AppendLog("✅ Test Error embed sent.")
            Else
                AppendLog($"⚠ Invalid JSON in Error embed: {err}")
            End If
        End If

        If Not String.IsNullOrWhiteSpace(chatEmbed.Text.Trim()) Then
            Dim payload = DiscordHelpers.BuildChatPayload(
        chatEmbed.Text.Trim(),
        DISCORD_MENTION,
        chatLine,
        respLine,
        screenshotRef,
        filename,
        GetFolderName(filePath),
        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        1
    )

            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                Await DiscordHelpers.PostJson(chatWebhook, payload)
                AppendLog("✅ Test Chat embed sent.")
            Else
                AppendLog($"⚠ Invalid JSON in Chat embed: {err}")
            End If
        End If

        If Not String.IsNullOrWhiteSpace(questEmbed.Text.Trim()) Then
            Dim payload = DiscordHelpers.BuildQuestPayload(
        questEmbed.Text.Trim(),
        DISCORD_MENTION,
        "Simulated quest text",
        filename,
        GetFolderName(filePath),
        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        1
    )

            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                Await DiscordHelpers.PostJson(questWebhook, payload)   ' 👈 use textbox value
                AppendLog("✅ Test Quest embed sent.")
            Else
                AppendLog($"⚠ Invalid JSON in Quest embed: {err}")
            End If
        End If
    End Sub

    Private Sub btnChatEditor_Click(sender As Object, e As EventArgs) Handles btnChatEditor.Click
        Using editor As New EmbedEditor(chatEmbed.Text, "Chat Embed Editor", DiscordHelpers.defaultChatTemplate, DarkModeEnabled.Checked)
            If editor.ShowDialog() = DialogResult.OK Then
                chatEmbed.Text = editor.ResultText
                My.Settings.ChatEmbedSet = chatEmbed.Text
                My.Settings.Save()
                AppendLog("✅ Chat embed template updated.")
            End If
        End Using
    End Sub
    Private Sub btnQuestEditor_Click(sender As Object, e As EventArgs) Handles btnQuestEditor.Click
        Using editor As New EmbedEditor(questEmbed.Text, "Quest Embed Editor", DiscordHelpers.defaultQuestTemplate, DarkModeEnabled.Checked)
            If editor.ShowDialog() = DialogResult.OK Then
                questEmbed.Text = editor.ResultText
                My.Settings.QuestEmbedSet = questEmbed.Text
                My.Settings.Save()
                AppendLog("✅ Quest embed template updated.")
            End If
        End Using
    End Sub
    Private Sub btnErrorEditor_Click(sender As Object, e As EventArgs) Handles btnErrorEditor.Click
        Using editor As New EmbedEditor(errorEmbed.Text, "Error Embed Editor", DiscordHelpers.defaultErrorTemplate, DarkModeEnabled.Checked)
            If editor.ShowDialog() = DialogResult.OK Then
                errorEmbed.Text = editor.ResultText
                My.Settings.ErrorEmbedSet = errorEmbed.Text
                My.Settings.Save()
                AppendLog("✅ Error embed template updated.")
            End If
        End Using
    End Sub
    Private Async Sub btnCheckUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckUpdates.Click
        Await UpdateHelper.CheckForUpdates(AddressOf AppendLog)
    End Sub
End Class