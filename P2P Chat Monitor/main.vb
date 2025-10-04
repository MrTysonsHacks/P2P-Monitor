Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks
Imports MaterialSkin
Imports MaterialSkin.Controls
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports P2P_Chat_Monitor.My

Public Class main
    Private WithEvents cmbLogDir As MaterialSkin.Controls.MaterialComboBox
    Private isSyncingUI As Boolean = False
    Private cmsLogDir As ContextMenuStrip
    Private mnuRemoveSelected As ToolStripMenuItem
    Private mnuSort As ToolStripMenuItem
    Private mnuClearAll As ToolStripMenuItem
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
    Private botFailureTriggers As New List(Of Regex)
    Private botFailureReasons As New List(Of KeyValuePair(Of Regex, String))
    Private selfieTimer As System.Threading.Timer
    Private ReadOnly selfieLock As New Object()
    Public Shared accountBreakStates As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

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

    Private Function CreateThemedContextMenu() As ContextMenuStrip
        Dim menu As ContextMenuStrip = Nothing

        Try
            Dim candidateNames As String() = {
                "MaterialSkin.Controls.MaterialContextMenuStrip, MaterialSkin",
                "MaterialSkin.Controls.MaterialContextMenuStrip, MaterialSkin.2"
            }

            For Each qn In candidateNames
                Dim themedType As Type = Type.GetType(qn, throwOnError:=False)
                If themedType Is Nothing Then Continue For

                Dim ctor0 = themedType.GetConstructor(Type.EmptyTypes)
                If ctor0 IsNot Nothing Then
                    menu = CType(ctor0.Invoke(Nothing), ContextMenuStrip)
                    Exit For
                End If

                Dim ctor1 = themedType.GetConstructor(New Type() {GetType(System.ComponentModel.IContainer)})
                If ctor1 IsNot Nothing Then
                    Dim container As System.ComponentModel.IContainer = If(Me.components, New System.ComponentModel.Container())
                    menu = CType(ctor1.Invoke(New Object() {container}), ContextMenuStrip)
                    Exit For
                End If
            Next
        Catch
            menu = Nothing
        End Try

        If menu Is Nothing Then
            menu = New ContextMenuStrip()
        End If

        AddHandler menu.Opening, AddressOf CmsLogDir_Opening
        menu.ShowImageMargin = False
        menu.RenderMode = ToolStripRenderMode.ManagerRenderMode
        menu.Items.Clear()

        mnuRemoveSelected = New ToolStripMenuItem("Remove selected")
        AddHandler mnuRemoveSelected.Click, AddressOf MnuRemoveSelected_Click
        menu.Items.Add(mnuRemoveSelected)

        mnuSort = New ToolStripMenuItem("Sort A→Z")
        AddHandler mnuSort.Click, AddressOf MnuSort_Click
        menu.Items.Add(mnuSort)

        menu.Items.Add(New ToolStripSeparator())

        mnuClearAll = New ToolStripMenuItem("Clear all")
        AddHandler mnuClearAll.Click, AddressOf MnuClearAll_Click
        menu.Items.Add(mnuClearAll)

        Return menu
    End Function

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

    Private Sub ScreenshotMode_CheckedChangedHandler(ByVal s As Object, ByVal ev As EventArgs)
        If cmbLogDir IsNot Nothing Then
            cmbLogDir.Enabled = Not screenshotmode.Checked
        End If
    End Sub
    Private Function GetAllParts() As List(Of String)
        Return txtLogDir.Text.Split(";"c).
        Select(Function(p) p.Trim()).
        Where(Function(p) p.Length > 0).
        Distinct(StringComparer.OrdinalIgnoreCase).
        ToList()
    End Function

    Private Sub SetAllParts(parts As IEnumerable(Of String))
        Dim newRaw = String.Join(";", parts)
        If Not String.Equals(newRaw, txtLogDir.Text, StringComparison.Ordinal) Then
            txtLogDir.Text = newRaw
        End If
        SyncComboFromText()
    End Sub

    Private Sub CmbLogDir_SelectionChangeCommittedHandler(ByVal s As Object, ByVal ev As EventArgs)
        Dim sel = TryCast(cmbLogDir.SelectedItem, String)
        If String.IsNullOrWhiteSpace(sel) Then Return
        EnsureFolderInText(sel)
        SyncComboFromText()
    End Sub
    Private Sub MnuRemoveSelected_Click(ByVal s As Object, ByVal ev As EventArgs)
        Dim sel = TryCast(cmbLogDir.SelectedItem, String)
        If String.IsNullOrWhiteSpace(sel) Then Return
        Dim parts = GetAllParts().Where(Function(p) Not p.Equals(sel, StringComparison.OrdinalIgnoreCase)).ToList()
        SetAllParts(parts)
    End Sub

    Private Sub MnuRemoveMissing_Click(ByVal s As Object, ByVal ev As EventArgs)
        Dim parts = GetAllParts().Where(Function(p) IO.Directory.Exists(p)).ToList()
        SetAllParts(parts)
    End Sub

    Private Sub MnuSort_Click(ByVal s As Object, ByVal ev As EventArgs)
        Dim parts = GetAllParts().OrderBy(Function(p) p, StringComparer.OrdinalIgnoreCase).ToList()
        SetAllParts(parts)
    End Sub

    Private Sub MnuClearAll_Click(ByVal s As Object, ByVal ev As EventArgs)
        SetAllParts(Array.Empty(Of String))
    End Sub

    Private Sub CmsLogDir_Opening(ByVal s As Object, ByVal ev As System.ComponentModel.CancelEventArgs)
        If cmbLogDir Is Nothing OrElse cmsLogDir Is Nothing Then
            ev.Cancel = False
            Exit Sub
        End If

        Dim parts As List(Of String)
        Try
            parts = GetAllParts()
        Catch
            parts = New List(Of String)()
        End Try

        Dim hasAny As Boolean = (parts.Count > 0)
        Dim selText As String = TryCast(cmbLogDir.SelectedItem, String)
        Dim hasSel As Boolean = Not String.IsNullOrWhiteSpace(selText)

        mnuRemoveSelected.Enabled = hasSel
        mnuSort.Enabled = hasAny
        mnuClearAll.Enabled = hasAny

        Dim isMaterialMenu As Boolean = (cmsLogDir.GetType().FullName = "MaterialSkin.Controls.MaterialContextMenuStrip")
        If Not isMaterialMenu Then
            Try
                Dim mgr = MaterialSkin.MaterialSkinManager.Instance
                Dim isDark = (mgr IsNot Nothing AndAlso mgr.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK)

                cmsLogDir.BackColor = If(isDark, Color.FromArgb(48, 48, 48), Color.White)
                cmsLogDir.ForeColor = If(isDark, Color.White, Color.Black)

                For Each it As ToolStripItem In cmsLogDir.Items
                    If TypeOf it Is ToolStripMenuItem Then
                        it.ForeColor = cmsLogDir.ForeColor
                    End If
                Next
            Catch
            End Try
        End If
    End Sub

    Private Sub CmbLogDir_KeyDownHandler(ByVal s As Object, ByVal e As KeyEventArgs)
        If e.Control AndAlso e.KeyCode = Keys.Delete Then
            Dim sel = TryCast(cmbLogDir.SelectedItem, String)
            If Not String.IsNullOrWhiteSpace(sel) Then
                e.SuppressKeyPress = True
                MnuRemoveSelected_Click(Nothing, EventArgs.Empty)
            End If
        End If
    End Sub

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

            Case "bot"
                If isTrigger Then
                    botFailureTriggers.Add(regex)
                Else
                    botFailureReasons.Add(New KeyValuePair(Of Regex, String)(regex, friendlyText))
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
                SyncComboFromText()
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
        taskID.Text = My.Settings.TaskID
        selfieID.Text = My.Settings.SelfieID
        numIntervalSecond.Value = My.Settings.CheckInterval
        chatEmbed.Text = My.Settings.ChatEmbedSet
        errorEmbed.Text = My.Settings.ErrorEmbedSet
        questEmbed.Text = My.Settings.QuestEmbedSet
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

        cmsLogDir = CreateThemedContextMenu()
        cmbLogDir.ContextMenuStrip = cmsLogDir
        AddHandler cmbLogDir.KeyDown, AddressOf CmbLogDir_KeyDownHandler

        AddHandler txtLogDir.TextChanged, AddressOf TxtLogDir_TextChangedHandler
        AddHandler cmbLogDir.SelectionChangeCommitted, AddressOf CmbLogDir_SelectionChangeCommittedHandler
        AddHandler screenshotmode.CheckedChanged, AddressOf ScreenshotMode_CheckedChangedHandler
        accountNames.ContextMenuStrip = CLICreator.CreateAccountContextMenu(accountNames)

        SyncComboFromText()


        cmsLogDir = CreateThemedContextMenu()
        cmbLogDir.ContextMenuStrip = cmsLogDir
        AddHandler cmbLogDir.KeyDown, AddressOf CmbLogDir_KeyDownHandler

        mnuRemoveSelected = New ToolStripMenuItem("Remove selected")
        cmbLogDir.Left = txtLogDir.Left
        AddHandler cmbLogDir.SelectionChangeCommitted, AddressOf CmbLogDir_SelectionChangeCommittedHandler
        AddHandler screenshotmode.CheckedChanged, AddressOf ScreenshotMode_CheckedChangedHandler

        SyncComboFromText()

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
        Await FetchFailRules()
        Await UpdateHelper.CheckForUpdates(AddressOf AppendLog)
    End Sub
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        StopSelfieTimer()
        MyBase.OnFormClosing(e)
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
    Public Sub AppendLog(msg As String)
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
        My.Settings.TaskID = taskID.Text.Trim()
        My.Settings.SelfieID = selfieID.Text.Trim()
        My.Settings.LogFolderPath = txtLogDir.Text.Trim()
        My.Settings.CheckInterval = numIntervalSecond.Value
        My.Settings.BotSelfieInterval = numSelfieInterval.Value
        My.Settings.BotSelfie = selfieMode.Checked
        My.Settings.DarkModeOn = DarkModeEnabled.Checked
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
        My.Settings.Save()

        WEBHOOK_URL = My.Settings.WebhookURL
        DISCORD_MENTION = txtMention.Text.Trim()
        My.Settings.MentionID = DISCORD_MENTION
        LOG_DIR = My.Settings.LogFolderPath
        CHAT_CHANNEL = My.Settings.ChatID
        ERROR_CHANNEL = My.Settings.ErrorID
        QUEST_CHANNEL = My.Settings.QuestID
        TASK_CHANNEL = My.Settings.TaskID
        SELFIE_CHANNEL = My.Settings.SelfieID
        checkInterval = CInt(numIntervalSecond.Value)

        monitorChat = chkMonitorChat.Checked
        monitorQuests = monitorQuest.Checked
        monitorTasks = monitorTask.Checked
        takeSelfie = selfieMode.Checked
        takeScreenshots = captureWin.Checked
        autoCleanup = autoClean.Checked

        If String.IsNullOrWhiteSpace(WEBHOOK_URL) OrElse Not WEBHOOK_URL.StartsWith("http") Then
            AppendLog("⚠ Please enter a valid Discord Webhook URL.")
            Return
        End If

        If Not monitorChat AndAlso Not monitorQuests AndAlso Not questError.Checked AndAlso Not skillIssue.Checked AndAlso Not combatError.Checked AndAlso Not monitorTask.Checked AndAlso Not takeSelfie Then
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

        LogHelper.ResetSeen()
        Dim latestPer = LogHelper.GetLatestPerFolder(LOG_DIR)
        LogHelper.AnnounceLatestOnce(latestPer, AddressOf AppendLog, lastOffsets, lastProcessedTimes)

        For Each folder In logDirs
            For Each f In Directory.GetFiles(folder, "logfile-*.log", SearchOption.TopDirectoryOnly)
                LogHelper.JumpToEnd(f, lastOffsets, lastProcessedTimes)
            Next
        Next

        For Each folder In logDirs
            Dim watcher As New FileSystemWatcher(folder, "logfile-*.log")
            watcher.NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.Size
            watcher.IncludeSubdirectories = False

            AddHandler watcher.Changed, Async Sub(s, eArgs) Await LogHelper.OnLogChanged(s, eArgs,
            monitoring, lastOffsets, lastProcessedTimes,
            monitorChat, monitorQuests, takeScreenshots, monitorTasks,
            questError.Checked, skillIssue.Checked, combatError.Checked,
            questFailureTriggers, questFailureReasons,
            skillFailureTriggers, skillFailureReasons,
            combatFailureTriggers, combatFailureReasons,
            AddressOf AppendLog, AddressOf SendSegments,
            AddressOf PostFailAlert, AddressOf GetFolderName)

            AddHandler watcher.Created, Async Sub(s, eArgs) Await LogHelper.OnLogChanged(s, eArgs,
            monitoring, lastOffsets, lastProcessedTimes,
            monitorChat, monitorQuests, takeScreenshots, monitorTasks,
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

        If takeSelfie Then
            StartSelfieTimer()
        End If
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
        Dim targetWebhook As String = ERROR_CHANNEL
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
                                   Optional failureTrigger As String = "",
                                   Optional failureReason As String = "") As Task

        For segIdx = 0 To segments.Count - 1
            Dim seg = segments(segIdx)

            Dim screenshotPath As String = Nothing
            Dim screenshotRef As String = ""
            Dim outputRoot As String = IO.Path.GetDirectoryName(path)
            If takeScreenshots Then
                screenshotPath = ScreenshotHelpers.SnapAndSend(path, GetFolderName(path), outputRoot, AddressOf AppendLog)
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

            Dim targetWebhook As String = WEBHOOK_URL
            If embedTitle.Contains("Chat") Then
                targetWebhook = If(Not String.IsNullOrWhiteSpace(CHAT_CHANNEL), CHAT_CHANNEL, WEBHOOK_URL)
            ElseIf embedTitle.Contains("Quest") Then
                targetWebhook = If(Not String.IsNullOrWhiteSpace(QUEST_CHANNEL), QUEST_CHANNEL, WEBHOOK_URL)
            ElseIf embedTitle.Contains("Error") Then
                targetWebhook = If(Not String.IsNullOrWhiteSpace(ERROR_CHANNEL), ERROR_CHANNEL, WEBHOOK_URL)
            ElseIf embedTitle.Contains("Task") Then
                targetWebhook = If(Not String.IsNullOrWhiteSpace(TASK_CHANNEL), TASK_CHANNEL, WEBHOOK_URL)
            End If

            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                If Not String.IsNullOrWhiteSpace(screenshotPath) AndAlso IO.File.Exists(screenshotPath) Then
                    Await DiscordHelpers.UploadFile(targetWebhook, screenshotPath, payload, AddressOf AppendLog)
                    If autoCleanup AndAlso IO.File.Exists(screenshotPath) Then
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

    Private Function GetSelfieIntervalMinutes() As Integer
        Dim m As Integer = 0
        Try
            If Me.IsHandleCreated AndAlso Not Me.IsDisposed Then
                Me.Invoke(Sub()
                              If numSelfieInterval.Value > 0D Then
                                  m = CInt(numSelfieInterval.Value)
                              End If
                          End Sub)
            End If
        Catch
        End Try

        If m <= 0 Then m = My.Settings.BotSelfieInterval
        If m <= 0 Then m = 1
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

            Dim url As String =
            If(Not String.IsNullOrWhiteSpace(selfieUrl) AndAlso selfieUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase),
               selfieUrl,
               defaultUrl)

            If String.IsNullOrWhiteSpace(url) OrElse Not url.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
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

                    Dim payload As String = "{""content"": ""📸 Periodic screenshot for account: " & accountName.Replace("""", "'"c) & """}"

                    Await DiscordHelpers.UploadFile(url, shotPath, payload, AddressOf AppendLog)
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
        End Try
    End Sub

    Private Sub StopSelfieTimer()
        If selfieTimer IsNot Nothing Then
            selfieTimer.Dispose()
            selfieTimer = Nothing
            AppendLog("⏹ Selfie mode disabled.")
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

        Dim url As String = Nothing
        Dim failMsg As String = ""

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
            $"""color"": 6029136" &
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
        Dim taskLine As String = "Simulated Task"
        Dim activityLine As String = "Simulated Activity"
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
            DateTime.Now
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
            DateTime.Now,
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
            screenshotRef,
            filename,
            GetFolderName(filePath),
            DateTime.Now,
            1
        )

            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                Await DiscordHelpers.PostJson(questWebhook, payload)
                AppendLog("✅ Test Quest embed sent.")
            Else
                AppendLog($"⚠ Invalid JSON in Quest embed: {err}")
            End If
        End If


        If Not String.IsNullOrWhiteSpace(taskEmbed.Text.Trim()) Then
            Dim payload = DiscordHelpers.BuildTaskPayload(
            taskEmbed.Text.Trim(),
            DISCORD_MENTION,
            taskLine,
            activityLine,
            screenshotRef,
            filename,
            GetFolderName(filePath),
            DateTime.Now,
            1
        )

            Dim err As String = Nothing
            If DiscordHelpers.IsJson(payload, err) Then
                Await DiscordHelpers.PostJson(txtWebhook.Text.Trim(), payload)
                AppendLog("✅ Test Task embed sent.")
            Else
                AppendLog($"⚠ Invalid JSON in Task embed: {err}")
            End If
        End If
    End Sub
    Private Sub embedEditors_Click(sender As Object, e As EventArgs) Handles embedEditors.Click
        Dim selector As New EmbedSelector()
        selector.ShowDialog(Me)
    End Sub
    Private Async Sub btnCheckUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckUpdates.Click
        Await UpdateHelper.CheckForUpdates(AddressOf AppendLog)
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
        Dim newName As String = CLICreator.PromptForAccountName(Me)
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
        CLICreator.GenerateBatchFile(
            dbPath.Text,
            ramNum.Value,
            covertMode.Checked,
            freshStart.Checked,
            accountNames.Items.Cast(Of String)(),
            cliOutput
        )
    End Sub
End Class