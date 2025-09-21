<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class main
    Inherits MaterialSkin.Controls.MaterialForm

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(main))
        btnBrowseLogDir = New MaterialSkin.Controls.MaterialButton()
        btnStart = New MaterialSkin.Controls.MaterialButton()
        btnStop = New MaterialSkin.Controls.MaterialButton()
        chkMonitorChat = New MaterialSkin.Controls.MaterialSwitch()
        monitorQuest = New MaterialSkin.Controls.MaterialSwitch()
        captureWin = New MaterialSkin.Controls.MaterialSwitch()
        autoClean = New MaterialSkin.Controls.MaterialSwitch()
        monitorError = New MaterialSkin.Controls.MaterialSwitch()
        DarkModeEnabled = New MaterialSkin.Controls.MaterialSwitch()
        numIntervalSecond = New MaterialSkin.Controls.MaterialSlider()
        MaterialLabel1 = New MaterialSkin.Controls.MaterialLabel()
        MaterialLabel2 = New MaterialSkin.Controls.MaterialLabel()
        MaterialLabel3 = New MaterialSkin.Controls.MaterialLabel()
        txtWebhook = New MaterialSkin.Controls.MaterialTextBox()
        questID = New MaterialSkin.Controls.MaterialTextBox()
        chatID = New MaterialSkin.Controls.MaterialTextBox()
        txtMention = New MaterialSkin.Controls.MaterialTextBox()
        errorID = New MaterialSkin.Controls.MaterialTextBox()
        MaterialLabel4 = New MaterialSkin.Controls.MaterialLabel()
        MaterialLabel5 = New MaterialSkin.Controls.MaterialLabel()
        MaterialLabel6 = New MaterialSkin.Controls.MaterialLabel()
        combatError = New MaterialSkin.Controls.MaterialSwitch()
        questError = New MaterialSkin.Controls.MaterialSwitch()
        skillIssue = New MaterialSkin.Controls.MaterialSwitch()
        txtLog = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        Hamburger = New MaterialSkin.Controls.MaterialTabControl()
        discordManagement = New TabPage()
        btnErrorEditor = New MaterialSkin.Controls.MaterialButton()
        btnQuestEditor = New MaterialSkin.Controls.MaterialButton()
        btnChatEditor = New MaterialSkin.Controls.MaterialButton()
        monitorManagement = New TabPage()
        txtLogDir = New MaterialSkin.Controls.MaterialTextBox()
        MaterialLabel7 = New MaterialSkin.Controls.MaterialLabel()
        screenshotmode = New MaterialSkin.Controls.MaterialSwitch()
        helpLinks = New TabPage()
        MaterialMultiLineTextBox1 = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        p2pgearBtn = New MaterialSkin.Controls.MaterialButton()
        p2psurvivalBtn = New MaterialSkin.Controls.MaterialButton()
        p2psetupBtn = New MaterialSkin.Controls.MaterialButton()
        p2psalesBtn = New MaterialSkin.Controls.MaterialButton()
        dbforumBtn = New MaterialSkin.Controls.MaterialButton()
        dbdiscordBtn = New MaterialSkin.Controls.MaterialButton()
        p2pdiscordBtn = New MaterialSkin.Controls.MaterialButton()
        wikiBtn = New MaterialSkin.Controls.MaterialButton()
        ImageList1 = New ImageList(components)
        clearBtn = New MaterialSkin.Controls.MaterialButton()
        forcecheckBtn = New MaterialSkin.Controls.MaterialButton()
        testBtn = New MaterialSkin.Controls.MaterialButton()
        testEmbeds = New MaterialSkin.Controls.MaterialButton()
        questEmbed = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        chatEmbed = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        btnCheckUpdates = New MaterialSkin.Controls.MaterialButton()
        errorEmbed = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        Hamburger.SuspendLayout()
        discordManagement.SuspendLayout()
        monitorManagement.SuspendLayout()
        helpLinks.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnBrowseLogDir
        ' 
        btnBrowseLogDir.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnBrowseLogDir.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnBrowseLogDir.Depth = 0
        btnBrowseLogDir.HighEmphasis = True
        btnBrowseLogDir.Icon = Nothing
        btnBrowseLogDir.Location = New Point(1158, 125)
        btnBrowseLogDir.Margin = New Padding(4, 6, 4, 6)
        btnBrowseLogDir.MouseState = MaterialSkin.MouseState.HOVER
        btnBrowseLogDir.Name = "btnBrowseLogDir"
        btnBrowseLogDir.NoAccentTextColor = Color.Empty
        btnBrowseLogDir.Size = New Size(80, 36)
        btnBrowseLogDir.TabIndex = 15
        btnBrowseLogDir.Text = "Browse"
        btnBrowseLogDir.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnBrowseLogDir.UseAccentColor = False
        btnBrowseLogDir.UseVisualStyleBackColor = True
        ' 
        ' btnStart
        ' 
        btnStart.AutoSize = False
        btnStart.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnStart.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnStart.Depth = 0
        btnStart.HighEmphasis = True
        btnStart.Icon = Nothing
        btnStart.Location = New Point(66, 324)
        btnStart.Margin = New Padding(4, 6, 4, 6)
        btnStart.MouseState = MaterialSkin.MouseState.HOVER
        btnStart.Name = "btnStart"
        btnStart.NoAccentTextColor = Color.Empty
        btnStart.Size = New Size(168, 36)
        btnStart.TabIndex = 6
        btnStart.Text = "Start Monitoring"
        btnStart.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnStart.UseAccentColor = False
        btnStart.UseVisualStyleBackColor = True
        ' 
        ' btnStop
        ' 
        btnStop.AutoSize = False
        btnStop.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnStop.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnStop.Depth = 0
        btnStop.HighEmphasis = True
        btnStop.Icon = Nothing
        btnStop.Location = New Point(66, 372)
        btnStop.Margin = New Padding(4, 6, 4, 6)
        btnStop.MouseState = MaterialSkin.MouseState.HOVER
        btnStop.Name = "btnStop"
        btnStop.NoAccentTextColor = Color.Empty
        btnStop.Size = New Size(168, 36)
        btnStop.TabIndex = 8
        btnStop.Text = "Stop Monitoring"
        btnStop.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnStop.UseAccentColor = False
        btnStop.UseVisualStyleBackColor = True
        ' 
        ' chkMonitorChat
        ' 
        chkMonitorChat.AutoSize = True
        chkMonitorChat.Depth = 0
        chkMonitorChat.Location = New Point(5, 12)
        chkMonitorChat.Margin = New Padding(0)
        chkMonitorChat.MouseLocation = New Point(-1, -1)
        chkMonitorChat.MouseState = MaterialSkin.MouseState.HOVER
        chkMonitorChat.Name = "chkMonitorChat"
        chkMonitorChat.Ripple = True
        chkMonitorChat.Size = New Size(201, 37)
        chkMonitorChat.TabIndex = 24
        chkMonitorChat.Text = "Monitor Chat Events"
        chkMonitorChat.UseVisualStyleBackColor = True
        ' 
        ' monitorQuest
        ' 
        monitorQuest.AutoSize = True
        monitorQuest.Depth = 0
        monitorQuest.Location = New Point(5, 49)
        monitorQuest.Margin = New Padding(0)
        monitorQuest.MouseLocation = New Point(-1, -1)
        monitorQuest.MouseState = MaterialSkin.MouseState.HOVER
        monitorQuest.Name = "monitorQuest"
        monitorQuest.Ripple = True
        monitorQuest.Size = New Size(251, 37)
        monitorQuest.TabIndex = 25
        monitorQuest.Text = "Monitor Quest Completions"
        monitorQuest.UseVisualStyleBackColor = True
        ' 
        ' captureWin
        ' 
        captureWin.AutoSize = True
        captureWin.Depth = 0
        captureWin.Location = New Point(5, 86)
        captureWin.Margin = New Padding(0)
        captureWin.MouseLocation = New Point(-1, -1)
        captureWin.MouseState = MaterialSkin.MouseState.HOVER
        captureWin.Name = "captureWin"
        captureWin.Ripple = True
        captureWin.Size = New Size(299, 37)
        captureWin.TabIndex = 26
        captureWin.Text = "Take Screenshots (Chat & Quests)"
        captureWin.UseVisualStyleBackColor = True
        ' 
        ' autoClean
        ' 
        autoClean.AutoSize = True
        autoClean.Depth = 0
        autoClean.Location = New Point(5, 123)
        autoClean.Margin = New Padding(0)
        autoClean.MouseLocation = New Point(-1, -1)
        autoClean.MouseState = MaterialSkin.MouseState.HOVER
        autoClean.Name = "autoClean"
        autoClean.Ripple = True
        autoClean.Size = New Size(273, 37)
        autoClean.TabIndex = 27
        autoClean.Text = "Auto-Delete Local Screenshots"
        autoClean.UseVisualStyleBackColor = True
        ' 
        ' monitorError
        ' 
        monitorError.AutoSize = True
        monitorError.Depth = 0
        monitorError.Location = New Point(319, 12)
        monitorError.Margin = New Padding(0)
        monitorError.MouseLocation = New Point(-1, -1)
        monitorError.MouseState = MaterialSkin.MouseState.HOVER
        monitorError.Name = "monitorError"
        monitorError.Ripple = True
        monitorError.Size = New Size(215, 37)
        monitorError.TabIndex = 28
        monitorError.Text = "Monitor Error Reasons"
        monitorError.UseVisualStyleBackColor = True
        ' 
        ' DarkModeEnabled
        ' 
        DarkModeEnabled.AutoSize = True
        DarkModeEnabled.Depth = 0
        DarkModeEnabled.Location = New Point(5, 160)
        DarkModeEnabled.Margin = New Padding(0)
        DarkModeEnabled.MouseLocation = New Point(-1, -1)
        DarkModeEnabled.MouseState = MaterialSkin.MouseState.HOVER
        DarkModeEnabled.Name = "DarkModeEnabled"
        DarkModeEnabled.Ripple = True
        DarkModeEnabled.Size = New Size(305, 37)
        DarkModeEnabled.TabIndex = 29
        DarkModeEnabled.Text = "Dark Mode cause Choco is a pussy"
        DarkModeEnabled.UseVisualStyleBackColor = True
        ' 
        ' numIntervalSecond
        ' 
        numIntervalSecond.Depth = 0
        numIntervalSecond.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        numIntervalSecond.Location = New Point(849, 12)
        numIntervalSecond.MouseState = MaterialSkin.MouseState.HOVER
        numIntervalSecond.Name = "numIntervalSecond"
        numIntervalSecond.RangeMax = 60
        numIntervalSecond.RangeMin = 1
        numIntervalSecond.Size = New Size(359, 40)
        numIntervalSecond.TabIndex = 30
        numIntervalSecond.Text = "Check Interval"
        numIntervalSecond.UseAccentColor = True
        numIntervalSecond.Value = 5
        numIntervalSecond.ValueSuffix = " Seconds"
        ' 
        ' MaterialLabel1
        ' 
        MaterialLabel1.AutoSize = True
        MaterialLabel1.Depth = 0
        MaterialLabel1.Font = New Font("Roboto", 10F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel1.FontType = MaterialSkin.MaterialSkinManager.fontType.Overline
        MaterialLabel1.Location = New Point(844, 49)
        MaterialLabel1.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel1.Name = "MaterialLabel1"
        MaterialLabel1.Size = New Size(404, 13)
        MaterialLabel1.TabIndex = 31
        MaterialLabel1.Text = "In order to take accurate screenshots interval must be set to 5 seconds, no lower or higher." & vbCrLf & vbCrLf
        MaterialLabel1.UseAccent = True
        ' 
        ' MaterialLabel2
        ' 
        MaterialLabel2.AutoSize = True
        MaterialLabel2.Depth = 0
        MaterialLabel2.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel2.Location = New Point(17, 16)
        MaterialLabel2.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel2.Name = "MaterialLabel2"
        MaterialLabel2.Size = New Size(129, 19)
        MaterialLabel2.TabIndex = 33
        MaterialLabel2.Text = "Discord Webhook:"
        ' 
        ' MaterialLabel3
        ' 
        MaterialLabel3.AutoSize = True
        MaterialLabel3.Depth = 0
        MaterialLabel3.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel3.Location = New Point(38, 126)
        MaterialLabel3.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel3.Name = "MaterialLabel3"
        MaterialLabel3.Size = New Size(108, 19)
        MaterialLabel3.TabIndex = 34
        MaterialLabel3.Text = "Chat Webhook:"
        ' 
        ' txtWebhook
        ' 
        txtWebhook.AnimateReadOnly = False
        txtWebhook.BorderStyle = BorderStyle.None
        txtWebhook.Depth = 0
        txtWebhook.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        txtWebhook.LeadingIcon = Nothing
        txtWebhook.Location = New Point(151, 3)
        txtWebhook.MaxLength = 128
        txtWebhook.MouseState = MaterialSkin.MouseState.OUT
        txtWebhook.Multiline = False
        txtWebhook.Name = "txtWebhook"
        txtWebhook.Size = New Size(460, 50)
        txtWebhook.TabIndex = 35
        txtWebhook.Text = ""
        txtWebhook.TrailingIcon = Nothing
        ' 
        ' questID
        ' 
        questID.AnimateReadOnly = False
        questID.BorderStyle = BorderStyle.None
        questID.Depth = 0
        questID.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        questID.LeadingIcon = Nothing
        questID.Location = New Point(771, 59)
        questID.MaxLength = 128
        questID.MouseState = MaterialSkin.MouseState.OUT
        questID.Multiline = False
        questID.Name = "questID"
        questID.Size = New Size(533, 50)
        questID.TabIndex = 36
        questID.Text = ""
        questID.TrailingIcon = Nothing
        ' 
        ' chatID
        ' 
        chatID.AnimateReadOnly = False
        chatID.BorderStyle = BorderStyle.None
        chatID.Depth = 0
        chatID.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        chatID.LeadingIcon = Nothing
        chatID.Location = New Point(152, 115)
        chatID.MaxLength = 128
        chatID.MouseState = MaterialSkin.MouseState.OUT
        chatID.Multiline = False
        chatID.Name = "chatID"
        chatID.Size = New Size(459, 50)
        chatID.TabIndex = 37
        chatID.Text = ""
        chatID.TrailingIcon = Nothing
        ' 
        ' txtMention
        ' 
        txtMention.AnimateReadOnly = False
        txtMention.BorderStyle = BorderStyle.None
        txtMention.Depth = 0
        txtMention.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        txtMention.Hint = "Discord User ID"
        txtMention.LeadingIcon = Nothing
        txtMention.Location = New Point(152, 59)
        txtMention.MaxLength = 128
        txtMention.MouseState = MaterialSkin.MouseState.OUT
        txtMention.Multiline = False
        txtMention.Name = "txtMention"
        txtMention.Size = New Size(459, 50)
        txtMention.TabIndex = 38
        txtMention.Text = ""
        txtMention.TrailingIcon = Nothing
        ' 
        ' errorID
        ' 
        errorID.AnimateReadOnly = False
        errorID.BorderStyle = BorderStyle.None
        errorID.Depth = 0
        errorID.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        errorID.LeadingIcon = Nothing
        errorID.Location = New Point(771, 3)
        errorID.MaxLength = 128
        errorID.MouseState = MaterialSkin.MouseState.OUT
        errorID.Multiline = False
        errorID.Name = "errorID"
        errorID.Size = New Size(533, 50)
        errorID.TabIndex = 39
        errorID.Text = ""
        errorID.TrailingIcon = Nothing
        ' 
        ' MaterialLabel4
        ' 
        MaterialLabel4.AutoSize = True
        MaterialLabel4.Depth = 0
        MaterialLabel4.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel4.Location = New Point(6, 72)
        MaterialLabel4.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel4.Name = "MaterialLabel4"
        MaterialLabel4.Size = New Size(140, 19)
        MaterialLabel4.TabIndex = 40
        MaterialLabel4.Text = "Discord Mention ID:"
        ' 
        ' MaterialLabel5
        ' 
        MaterialLabel5.AutoSize = True
        MaterialLabel5.Depth = 0
        MaterialLabel5.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel5.Location = New Point(657, 16)
        MaterialLabel5.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel5.Name = "MaterialLabel5"
        MaterialLabel5.Size = New Size(108, 19)
        MaterialLabel5.TabIndex = 41
        MaterialLabel5.Text = "Error Webhook:"
        ' 
        ' MaterialLabel6
        ' 
        MaterialLabel6.AutoSize = True
        MaterialLabel6.Depth = 0
        MaterialLabel6.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel6.Location = New Point(649, 72)
        MaterialLabel6.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel6.Name = "MaterialLabel6"
        MaterialLabel6.Size = New Size(116, 19)
        MaterialLabel6.TabIndex = 42
        MaterialLabel6.Text = "Quest Webhook:"
        ' 
        ' combatError
        ' 
        combatError.AutoSize = True
        combatError.Depth = 0
        combatError.Location = New Point(319, 49)
        combatError.Margin = New Padding(0)
        combatError.MouseLocation = New Point(-1, -1)
        combatError.MouseState = MaterialSkin.MouseState.HOVER
        combatError.Name = "combatError"
        combatError.Ripple = True
        combatError.Size = New Size(210, 37)
        combatError.TabIndex = 44
        combatError.Text = "Slayer/Combat Errors"
        combatError.UseVisualStyleBackColor = True
        ' 
        ' questError
        ' 
        questError.AutoSize = True
        questError.Depth = 0
        questError.Location = New Point(319, 86)
        questError.Margin = New Padding(0)
        questError.MouseLocation = New Point(-1, -1)
        questError.MouseState = MaterialSkin.MouseState.HOVER
        questError.Name = "questError"
        questError.Ripple = True
        questError.Size = New Size(144, 37)
        questError.TabIndex = 45
        questError.Text = "Quest Errors"
        questError.UseVisualStyleBackColor = True
        ' 
        ' skillIssue
        ' 
        skillIssue.AutoSize = True
        skillIssue.Depth = 0
        skillIssue.Location = New Point(319, 123)
        skillIssue.Margin = New Padding(0)
        skillIssue.MouseLocation = New Point(-1, -1)
        skillIssue.MouseState = MaterialSkin.MouseState.HOVER
        skillIssue.Name = "skillIssue"
        skillIssue.Ripple = True
        skillIssue.Size = New Size(155, 37)
        skillIssue.TabIndex = 46
        skillIssue.Text = "Skilling Errors"
        skillIssue.UseVisualStyleBackColor = True
        ' 
        ' txtLog
        ' 
        txtLog.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        txtLog.BorderStyle = BorderStyle.None
        txtLog.Depth = 0
        txtLog.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        txtLog.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        txtLog.Location = New Point(241, 319)
        txtLog.MouseState = MaterialSkin.MouseState.HOVER
        txtLog.Name = "txtLog"
        txtLog.ReadOnly = True
        txtLog.Size = New Size(1083, 407)
        txtLog.TabIndex = 57
        txtLog.Text = ""
        ' 
        ' Hamburger
        ' 
        Hamburger.Appearance = TabAppearance.Buttons
        Hamburger.Controls.Add(discordManagement)
        Hamburger.Controls.Add(monitorManagement)
        Hamburger.Controls.Add(helpLinks)
        Hamburger.Depth = 0
        Hamburger.ImageList = ImageList1
        Hamburger.Location = New Point(62, 67)
        Hamburger.MouseState = MaterialSkin.MouseState.HOVER
        Hamburger.Multiline = True
        Hamburger.Name = "Hamburger"
        Hamburger.SelectedIndex = 0
        Hamburger.Size = New Size(1262, 257)
        Hamburger.TabIndex = 58
        ' 
        ' discordManagement
        ' 
        discordManagement.Controls.Add(btnErrorEditor)
        discordManagement.Controls.Add(btnQuestEditor)
        discordManagement.Controls.Add(btnChatEditor)
        discordManagement.Controls.Add(errorID)
        discordManagement.Controls.Add(questID)
        discordManagement.Controls.Add(MaterialLabel2)
        discordManagement.Controls.Add(MaterialLabel4)
        discordManagement.Controls.Add(MaterialLabel6)
        discordManagement.Controls.Add(MaterialLabel3)
        discordManagement.Controls.Add(chatID)
        discordManagement.Controls.Add(MaterialLabel5)
        discordManagement.Controls.Add(txtMention)
        discordManagement.Controls.Add(txtWebhook)
        discordManagement.ImageKey = "discord.png"
        discordManagement.Location = New Point(4, 46)
        discordManagement.Name = "discordManagement"
        discordManagement.Padding = New Padding(3)
        discordManagement.Size = New Size(1254, 207)
        discordManagement.TabIndex = 0
        discordManagement.Text = "Discord Management"
        discordManagement.UseVisualStyleBackColor = True
        ' 
        ' btnErrorEditor
        ' 
        btnErrorEditor.AutoSize = False
        btnErrorEditor.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnErrorEditor.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnErrorEditor.Depth = 0
        btnErrorEditor.HighEmphasis = True
        btnErrorEditor.Icon = Nothing
        btnErrorEditor.Location = New Point(1073, 129)
        btnErrorEditor.Margin = New Padding(4, 6, 4, 6)
        btnErrorEditor.MouseState = MaterialSkin.MouseState.HOVER
        btnErrorEditor.Name = "btnErrorEditor"
        btnErrorEditor.NoAccentTextColor = Color.Empty
        btnErrorEditor.Size = New Size(174, 36)
        btnErrorEditor.TabIndex = 45
        btnErrorEditor.Text = "Error Embed Editor"
        btnErrorEditor.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnErrorEditor.UseAccentColor = False
        btnErrorEditor.UseVisualStyleBackColor = True
        ' 
        ' btnQuestEditor
        ' 
        btnQuestEditor.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnQuestEditor.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnQuestEditor.Depth = 0
        btnQuestEditor.HighEmphasis = True
        btnQuestEditor.Icon = Nothing
        btnQuestEditor.Location = New Point(709, 129)
        btnQuestEditor.Margin = New Padding(4, 6, 4, 6)
        btnQuestEditor.MouseState = MaterialSkin.MouseState.HOVER
        btnQuestEditor.Name = "btnQuestEditor"
        btnQuestEditor.NoAccentTextColor = Color.Empty
        btnQuestEditor.Size = New Size(174, 36)
        btnQuestEditor.TabIndex = 44
        btnQuestEditor.Text = "Quest Embed Editor"
        btnQuestEditor.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnQuestEditor.UseAccentColor = False
        btnQuestEditor.UseVisualStyleBackColor = True
        ' 
        ' btnChatEditor
        ' 
        btnChatEditor.AutoSize = False
        btnChatEditor.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnChatEditor.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnChatEditor.Depth = 0
        btnChatEditor.HighEmphasis = True
        btnChatEditor.Icon = Nothing
        btnChatEditor.Location = New Point(891, 129)
        btnChatEditor.Margin = New Padding(4, 6, 4, 6)
        btnChatEditor.MouseState = MaterialSkin.MouseState.HOVER
        btnChatEditor.Name = "btnChatEditor"
        btnChatEditor.NoAccentTextColor = Color.Empty
        btnChatEditor.Size = New Size(174, 36)
        btnChatEditor.TabIndex = 43
        btnChatEditor.Text = "Chat Embed Editor"
        btnChatEditor.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnChatEditor.UseAccentColor = False
        btnChatEditor.UseVisualStyleBackColor = True
        ' 
        ' monitorManagement
        ' 
        monitorManagement.Controls.Add(txtLogDir)
        monitorManagement.Controls.Add(MaterialLabel7)
        monitorManagement.Controls.Add(screenshotmode)
        monitorManagement.Controls.Add(btnBrowseLogDir)
        monitorManagement.Controls.Add(chkMonitorChat)
        monitorManagement.Controls.Add(numIntervalSecond)
        monitorManagement.Controls.Add(monitorQuest)
        monitorManagement.Controls.Add(skillIssue)
        monitorManagement.Controls.Add(MaterialLabel1)
        monitorManagement.Controls.Add(captureWin)
        monitorManagement.Controls.Add(questError)
        monitorManagement.Controls.Add(autoClean)
        monitorManagement.Controls.Add(combatError)
        monitorManagement.Controls.Add(monitorError)
        monitorManagement.Controls.Add(DarkModeEnabled)
        monitorManagement.ImageKey = "monitoring.png"
        monitorManagement.Location = New Point(4, 46)
        monitorManagement.Name = "monitorManagement"
        monitorManagement.Padding = New Padding(3)
        monitorManagement.Size = New Size(1254, 207)
        monitorManagement.TabIndex = 1
        monitorManagement.Text = "Monitor Management"
        monitorManagement.UseVisualStyleBackColor = True
        ' 
        ' txtLogDir
        ' 
        txtLogDir.AnimateReadOnly = False
        txtLogDir.BorderStyle = BorderStyle.None
        txtLogDir.Depth = 0
        txtLogDir.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        txtLogDir.LeadingIcon = Nothing
        txtLogDir.Location = New Point(849, 73)
        txtLogDir.MaxLength = 50
        txtLogDir.MouseState = MaterialSkin.MouseState.OUT
        txtLogDir.Multiline = False
        txtLogDir.Name = "txtLogDir"
        txtLogDir.Size = New Size(389, 50)
        txtLogDir.TabIndex = 52
        txtLogDir.Text = ""
        txtLogDir.TrailingIcon = Nothing
        ' 
        ' MaterialLabel7
        ' 
        MaterialLabel7.AutoSize = True
        MaterialLabel7.Depth = 0
        MaterialLabel7.Font = New Font("Roboto", 10F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel7.FontType = MaterialSkin.MaterialSkinManager.fontType.Overline
        MaterialLabel7.Location = New Point(844, 194)
        MaterialLabel7.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel7.Name = "MaterialLabel7"
        MaterialLabel7.Size = New Size(252, 13)
        MaterialLabel7.TabIndex = 51
        MaterialLabel7.Text = "Hides all important information in monitor for screenshots"
        MaterialLabel7.UseAccent = True
        ' 
        ' screenshotmode
        ' 
        screenshotmode.AutoSize = True
        screenshotmode.Depth = 0
        screenshotmode.Location = New Point(849, 160)
        screenshotmode.Margin = New Padding(0)
        screenshotmode.MouseLocation = New Point(-1, -1)
        screenshotmode.MouseState = MaterialSkin.MouseState.HOVER
        screenshotmode.Name = "screenshotmode"
        screenshotmode.Ripple = True
        screenshotmode.Size = New Size(181, 37)
        screenshotmode.TabIndex = 50
        screenshotmode.Text = "Screenshot Mode"
        screenshotmode.UseVisualStyleBackColor = True
        ' 
        ' helpLinks
        ' 
        helpLinks.Controls.Add(MaterialMultiLineTextBox1)
        helpLinks.Controls.Add(p2pgearBtn)
        helpLinks.Controls.Add(p2psurvivalBtn)
        helpLinks.Controls.Add(p2psetupBtn)
        helpLinks.Controls.Add(p2psalesBtn)
        helpLinks.Controls.Add(dbforumBtn)
        helpLinks.Controls.Add(dbdiscordBtn)
        helpLinks.Controls.Add(p2pdiscordBtn)
        helpLinks.Controls.Add(wikiBtn)
        helpLinks.ImageKey = "information-button.png"
        helpLinks.Location = New Point(4, 46)
        helpLinks.Name = "helpLinks"
        helpLinks.Padding = New Padding(3)
        helpLinks.Size = New Size(1254, 207)
        helpLinks.TabIndex = 2
        helpLinks.Text = "Information"
        helpLinks.UseVisualStyleBackColor = True
        ' 
        ' MaterialMultiLineTextBox1
        ' 
        MaterialMultiLineTextBox1.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        MaterialMultiLineTextBox1.BorderStyle = BorderStyle.None
        MaterialMultiLineTextBox1.Depth = 0
        MaterialMultiLineTextBox1.Font = New Font("Roboto SemiBold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        MaterialMultiLineTextBox1.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        MaterialMultiLineTextBox1.Location = New Point(719, 6)
        MaterialMultiLineTextBox1.MouseState = MaterialSkin.MouseState.HOVER
        MaterialMultiLineTextBox1.Name = "MaterialMultiLineTextBox1"
        MaterialMultiLineTextBox1.ReadOnly = True
        MaterialMultiLineTextBox1.Size = New Size(529, 194)
        MaterialMultiLineTextBox1.TabIndex = 8
        MaterialMultiLineTextBox1.Text = resources.GetString("MaterialMultiLineTextBox1.Text")
        ' 
        ' p2pgearBtn
        ' 
        p2pgearBtn.AutoSize = False
        p2pgearBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        p2pgearBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense
        p2pgearBtn.Depth = 0
        p2pgearBtn.HighEmphasis = True
        p2pgearBtn.Icon = Nothing
        p2pgearBtn.Location = New Point(383, 158)
        p2pgearBtn.Margin = New Padding(4, 6, 4, 6)
        p2pgearBtn.MouseState = MaterialSkin.MouseState.HOVER
        p2pgearBtn.Name = "p2pgearBtn"
        p2pgearBtn.NoAccentTextColor = Color.Empty
        p2pgearBtn.Size = New Size(158, 32)
        p2pgearBtn.TabIndex = 7
        p2pgearBtn.Text = "P2P Supported Gear"
        p2pgearBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
        p2pgearBtn.UseAccentColor = True
        p2pgearBtn.UseVisualStyleBackColor = True
        ' 
        ' p2psurvivalBtn
        ' 
        p2psurvivalBtn.AutoSize = False
        p2psurvivalBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        p2psurvivalBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense
        p2psurvivalBtn.Depth = 0
        p2psurvivalBtn.HighEmphasis = True
        p2psurvivalBtn.Icon = Nothing
        p2psurvivalBtn.Location = New Point(383, 110)
        p2psurvivalBtn.Margin = New Padding(4, 6, 4, 6)
        p2psurvivalBtn.MouseState = MaterialSkin.MouseState.HOVER
        p2psurvivalBtn.Name = "p2psurvivalBtn"
        p2psurvivalBtn.NoAccentTextColor = Color.Empty
        p2psurvivalBtn.Size = New Size(158, 32)
        p2psurvivalBtn.TabIndex = 6
        p2psurvivalBtn.Text = "P2P Survival Guide"
        p2psurvivalBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
        p2psurvivalBtn.UseAccentColor = True
        p2psurvivalBtn.UseVisualStyleBackColor = True
        ' 
        ' p2psetupBtn
        ' 
        p2psetupBtn.AutoSize = False
        p2psetupBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        p2psetupBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense
        p2psetupBtn.Depth = 0
        p2psetupBtn.HighEmphasis = True
        p2psetupBtn.Icon = Nothing
        p2psetupBtn.Location = New Point(383, 62)
        p2psetupBtn.Margin = New Padding(4, 6, 4, 6)
        p2psetupBtn.MouseState = MaterialSkin.MouseState.HOVER
        p2psetupBtn.Name = "p2psetupBtn"
        p2psetupBtn.NoAccentTextColor = Color.Empty
        p2psetupBtn.Size = New Size(158, 32)
        p2psetupBtn.TabIndex = 5
        p2psetupBtn.Text = "P2P Setup Guide"
        p2psetupBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
        p2psetupBtn.UseAccentColor = True
        p2psetupBtn.UseVisualStyleBackColor = True
        ' 
        ' p2psalesBtn
        ' 
        p2psalesBtn.AutoSize = False
        p2psalesBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        p2psalesBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense
        p2psalesBtn.Depth = 0
        p2psalesBtn.HighEmphasis = True
        p2psalesBtn.Icon = Nothing
        p2psalesBtn.Location = New Point(383, 14)
        p2psalesBtn.Margin = New Padding(4, 6, 4, 6)
        p2psalesBtn.MouseState = MaterialSkin.MouseState.HOVER
        p2psalesBtn.Name = "p2psalesBtn"
        p2psalesBtn.NoAccentTextColor = Color.Empty
        p2psalesBtn.Size = New Size(158, 32)
        p2psalesBtn.TabIndex = 4
        p2psalesBtn.Text = "P2P Sales Page"
        p2psalesBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
        p2psalesBtn.UseAccentColor = True
        p2psalesBtn.UseVisualStyleBackColor = True
        ' 
        ' dbforumBtn
        ' 
        dbforumBtn.AutoSize = False
        dbforumBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        dbforumBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense
        dbforumBtn.Depth = 0
        dbforumBtn.HighEmphasis = True
        dbforumBtn.Icon = Nothing
        dbforumBtn.Location = New Point(217, 158)
        dbforumBtn.Margin = New Padding(4, 6, 4, 6)
        dbforumBtn.MouseState = MaterialSkin.MouseState.HOVER
        dbforumBtn.Name = "dbforumBtn"
        dbforumBtn.NoAccentTextColor = Color.Empty
        dbforumBtn.Size = New Size(158, 32)
        dbforumBtn.TabIndex = 3
        dbforumBtn.Text = "Dreambot Forum"
        dbforumBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
        dbforumBtn.UseAccentColor = True
        dbforumBtn.UseVisualStyleBackColor = True
        ' 
        ' dbdiscordBtn
        ' 
        dbdiscordBtn.AutoSize = False
        dbdiscordBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        dbdiscordBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense
        dbdiscordBtn.Depth = 0
        dbdiscordBtn.HighEmphasis = True
        dbdiscordBtn.Icon = Nothing
        dbdiscordBtn.Location = New Point(217, 110)
        dbdiscordBtn.Margin = New Padding(4, 6, 4, 6)
        dbdiscordBtn.MouseState = MaterialSkin.MouseState.HOVER
        dbdiscordBtn.Name = "dbdiscordBtn"
        dbdiscordBtn.NoAccentTextColor = Color.Empty
        dbdiscordBtn.Size = New Size(158, 32)
        dbdiscordBtn.TabIndex = 2
        dbdiscordBtn.Text = "Dreambot Discord"
        dbdiscordBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
        dbdiscordBtn.UseAccentColor = True
        dbdiscordBtn.UseVisualStyleBackColor = True
        ' 
        ' p2pdiscordBtn
        ' 
        p2pdiscordBtn.AutoSize = False
        p2pdiscordBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        p2pdiscordBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense
        p2pdiscordBtn.Depth = 0
        p2pdiscordBtn.HighEmphasis = True
        p2pdiscordBtn.Icon = Nothing
        p2pdiscordBtn.Location = New Point(217, 62)
        p2pdiscordBtn.Margin = New Padding(4, 6, 4, 6)
        p2pdiscordBtn.MouseState = MaterialSkin.MouseState.HOVER
        p2pdiscordBtn.Name = "p2pdiscordBtn"
        p2pdiscordBtn.NoAccentTextColor = Color.Empty
        p2pdiscordBtn.Size = New Size(158, 32)
        p2pdiscordBtn.TabIndex = 1
        p2pdiscordBtn.Text = "P2P Discord"
        p2pdiscordBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
        p2pdiscordBtn.UseAccentColor = True
        p2pdiscordBtn.UseVisualStyleBackColor = True
        ' 
        ' wikiBtn
        ' 
        wikiBtn.AutoSize = False
        wikiBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        wikiBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Dense
        wikiBtn.Depth = 0
        wikiBtn.HighEmphasis = True
        wikiBtn.Icon = Nothing
        wikiBtn.Location = New Point(217, 14)
        wikiBtn.Margin = New Padding(4, 6, 4, 6)
        wikiBtn.MouseState = MaterialSkin.MouseState.HOVER
        wikiBtn.Name = "wikiBtn"
        wikiBtn.NoAccentTextColor = Color.Empty
        wikiBtn.Size = New Size(158, 32)
        wikiBtn.TabIndex = 0
        wikiBtn.Text = "P2P Wiki"
        wikiBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined
        wikiBtn.UseAccentColor = True
        wikiBtn.UseVisualStyleBackColor = True
        ' 
        ' ImageList1
        ' 
        ImageList1.ColorDepth = ColorDepth.Depth32Bit
        ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), ImageListStreamer)
        ImageList1.TransparentColor = Color.Transparent
        ImageList1.Images.SetKeyName(0, "information-button.png")
        ImageList1.Images.SetKeyName(1, "copy-writing.png")
        ImageList1.Images.SetKeyName(2, "monitoring.png")
        ImageList1.Images.SetKeyName(3, "discord.png")
        ' 
        ' clearBtn
        ' 
        clearBtn.AutoSize = False
        clearBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        clearBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        clearBtn.Depth = 0
        clearBtn.HighEmphasis = True
        clearBtn.Icon = Nothing
        clearBtn.Location = New Point(66, 690)
        clearBtn.Margin = New Padding(4, 6, 4, 6)
        clearBtn.MouseState = MaterialSkin.MouseState.HOVER
        clearBtn.Name = "clearBtn"
        clearBtn.NoAccentTextColor = Color.Empty
        clearBtn.Size = New Size(168, 36)
        clearBtn.TabIndex = 59
        clearBtn.Text = "Clear Console"
        clearBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        clearBtn.UseAccentColor = False
        clearBtn.UseVisualStyleBackColor = True
        ' 
        ' forcecheckBtn
        ' 
        forcecheckBtn.AutoSize = False
        forcecheckBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        forcecheckBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        forcecheckBtn.Depth = 0
        forcecheckBtn.HighEmphasis = True
        forcecheckBtn.Icon = Nothing
        forcecheckBtn.Location = New Point(66, 420)
        forcecheckBtn.Margin = New Padding(4, 6, 4, 6)
        forcecheckBtn.MouseState = MaterialSkin.MouseState.HOVER
        forcecheckBtn.Name = "forcecheckBtn"
        forcecheckBtn.NoAccentTextColor = Color.Empty
        forcecheckBtn.Size = New Size(168, 36)
        forcecheckBtn.TabIndex = 60
        forcecheckBtn.Text = "Force Check"
        forcecheckBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        forcecheckBtn.UseAccentColor = False
        forcecheckBtn.UseVisualStyleBackColor = True
        ' 
        ' testBtn
        ' 
        testBtn.AutoSize = False
        testBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        testBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        testBtn.Depth = 0
        testBtn.HighEmphasis = True
        testBtn.Icon = Nothing
        testBtn.Location = New Point(66, 468)
        testBtn.Margin = New Padding(4, 6, 4, 6)
        testBtn.MouseState = MaterialSkin.MouseState.HOVER
        testBtn.Name = "testBtn"
        testBtn.NoAccentTextColor = Color.Empty
        testBtn.RightToLeft = RightToLeft.Yes
        testBtn.Size = New Size(168, 36)
        testBtn.TabIndex = 61
        testBtn.Text = "Check Webhook Validation"
        testBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        testBtn.UseAccentColor = False
        testBtn.UseVisualStyleBackColor = True
        ' 
        ' testEmbeds
        ' 
        testEmbeds.AutoSize = False
        testEmbeds.AutoSizeMode = AutoSizeMode.GrowAndShrink
        testEmbeds.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        testEmbeds.Depth = 0
        testEmbeds.HighEmphasis = True
        testEmbeds.Icon = Nothing
        testEmbeds.Location = New Point(66, 516)
        testEmbeds.Margin = New Padding(4, 6, 4, 6)
        testEmbeds.MouseState = MaterialSkin.MouseState.HOVER
        testEmbeds.Name = "testEmbeds"
        testEmbeds.NoAccentTextColor = Color.Empty
        testEmbeds.RightToLeft = RightToLeft.Yes
        testEmbeds.Size = New Size(168, 36)
        testEmbeds.TabIndex = 62
        testEmbeds.Text = "Send Test Embeds"
        testEmbeds.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        testEmbeds.UseAccentColor = False
        testEmbeds.UseVisualStyleBackColor = True
        ' 
        ' questEmbed
        ' 
        questEmbed.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        questEmbed.BorderStyle = BorderStyle.None
        questEmbed.Depth = 0
        questEmbed.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        questEmbed.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        questEmbed.Location = New Point(6, 185)
        questEmbed.MouseState = MaterialSkin.MouseState.HOVER
        questEmbed.Name = "questEmbed"
        questEmbed.Size = New Size(12, 10)
        questEmbed.TabIndex = 63
        questEmbed.Text = resources.GetString("questEmbed.Text")
        ' 
        ' chatEmbed
        ' 
        chatEmbed.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        chatEmbed.BorderStyle = BorderStyle.None
        chatEmbed.Depth = 0
        chatEmbed.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        chatEmbed.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        chatEmbed.Location = New Point(6, 212)
        chatEmbed.MouseState = MaterialSkin.MouseState.HOVER
        chatEmbed.Name = "chatEmbed"
        chatEmbed.Size = New Size(12, 10)
        chatEmbed.TabIndex = 64
        chatEmbed.Text = resources.GetString("chatEmbed.Text")
        ' 
        ' btnCheckUpdates
        ' 
        btnCheckUpdates.AutoSize = False
        btnCheckUpdates.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnCheckUpdates.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnCheckUpdates.Depth = 0
        btnCheckUpdates.HighEmphasis = True
        btnCheckUpdates.Icon = Nothing
        btnCheckUpdates.Location = New Point(66, 642)
        btnCheckUpdates.Margin = New Padding(4, 6, 4, 6)
        btnCheckUpdates.MouseState = MaterialSkin.MouseState.HOVER
        btnCheckUpdates.Name = "btnCheckUpdates"
        btnCheckUpdates.NoAccentTextColor = Color.Empty
        btnCheckUpdates.Size = New Size(168, 36)
        btnCheckUpdates.TabIndex = 65
        btnCheckUpdates.Text = "Check For Updates"
        btnCheckUpdates.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnCheckUpdates.UseAccentColor = False
        btnCheckUpdates.UseVisualStyleBackColor = True
        ' 
        ' errorEmbed
        ' 
        errorEmbed.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        errorEmbed.BorderStyle = BorderStyle.None
        errorEmbed.Depth = 0
        errorEmbed.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        errorEmbed.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        errorEmbed.Location = New Point(6, 239)
        errorEmbed.MouseState = MaterialSkin.MouseState.HOVER
        errorEmbed.Name = "errorEmbed"
        errorEmbed.Size = New Size(12, 10)
        errorEmbed.TabIndex = 66
        errorEmbed.Text = resources.GetString("errorEmbed.Text")
        ' 
        ' Form1
        ' 
        AutoScaleMode = AutoScaleMode.None
        ClientSize = New Size(1330, 732)
        Controls.Add(errorEmbed)
        Controls.Add(btnCheckUpdates)
        Controls.Add(chatEmbed)
        Controls.Add(questEmbed)
        Controls.Add(testEmbeds)
        Controls.Add(testBtn)
        Controls.Add(forcecheckBtn)
        Controls.Add(clearBtn)
        Controls.Add(Hamburger)
        Controls.Add(txtLog)
        Controls.Add(btnStart)
        Controls.Add(btnStop)
        DrawerBackgroundWithAccent = True
        DrawerShowIconsWhenHidden = True
        DrawerTabControl = Hamburger
        DrawerUseColors = True
        DrawerWidth = 220
        Font = New Font("Roboto", 15.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "Form1"
        Sizable = False
        Text = "P2P Monitor v1.2.3 by CaS5"
        Hamburger.ResumeLayout(False)
        discordManagement.ResumeLayout(False)
        discordManagement.PerformLayout()
        monitorManagement.ResumeLayout(False)
        monitorManagement.PerformLayout()
        helpLinks.ResumeLayout(False)
        ResumeLayout(False)
    End Sub
    Friend WithEvents btnStop As MaterialSkin.Controls.MaterialButton
    Friend WithEvents btnStart As MaterialSkin.Controls.MaterialButton
    Friend WithEvents btnBrowseLogDir As MaterialSkin.Controls.MaterialButton
    Friend WithEvents chkMonitorChat As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents monitorQuest As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents captureWin As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents autoClean As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents monitorError As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents DarkModeEnabled As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents numIntervalSecond As MaterialSkin.Controls.MaterialSlider
    Friend WithEvents MaterialLabel1 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents MaterialLabel2 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents MaterialLabel3 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents txtWebhook As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents questID As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents chatID As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents txtMention As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents errorID As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialLabel4 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents MaterialLabel5 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents MaterialLabel6 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents combatError As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents questError As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents skillIssue As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents MaterialExpansionPanel1 As MaterialSkin.Controls.MaterialExpansionPanel
    Friend WithEvents txtLog As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents Hamburger As MaterialSkin.Controls.MaterialTabControl
    Friend WithEvents discordManagement As TabPage
    Friend WithEvents monitorManagement As TabPage
    Friend WithEvents helpLinks As TabPage
    Friend WithEvents p2psalesBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents dbforumBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents dbdiscordBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents p2pdiscordBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents wikiBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents p2pgearBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents p2psurvivalBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents p2psetupBtn As MaterialSkin.Controls.MaterialButton
    Public WithEvents ImageList1 As ImageList
    Friend WithEvents clearBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents forcecheckBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialLabel7 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents screenshotmode As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents txtLogDir As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents testBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents testEmbeds As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialMultiLineTextBox1 As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents btnErrorEditor As MaterialSkin.Controls.MaterialButton
    Friend WithEvents btnQuestEditor As MaterialSkin.Controls.MaterialButton
    Friend WithEvents btnChatEditor As MaterialSkin.Controls.MaterialButton
    Friend WithEvents questEmbed As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents chatEmbed As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents btnCheckUpdates As MaterialSkin.Controls.MaterialButton
    Friend WithEvents errorEmbed As MaterialSkin.Controls.MaterialMultiLineTextBox
End Class
