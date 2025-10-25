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
        DarkModeEnabled = New MaterialSkin.Controls.MaterialSwitch()
        numIntervalSecond = New MaterialSkin.Controls.MaterialSlider()
        MaterialLabel1 = New MaterialSkin.Controls.MaterialLabel()
        txtWebhook = New MaterialSkin.Controls.MaterialTextBox()
        questID = New MaterialSkin.Controls.MaterialTextBox()
        chatID = New MaterialSkin.Controls.MaterialTextBox()
        txtMention = New MaterialSkin.Controls.MaterialTextBox()
        errorID = New MaterialSkin.Controls.MaterialTextBox()
        combatError = New MaterialSkin.Controls.MaterialSwitch()
        questError = New MaterialSkin.Controls.MaterialSwitch()
        skillIssue = New MaterialSkin.Controls.MaterialSwitch()
        txtLog = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        Hamburger = New MaterialSkin.Controls.MaterialTabControl()
        discordManagement = New TabPage()
        dtmBtn = New MaterialSkin.Controls.MaterialButton()
        useDTM = New MaterialSkin.Controls.MaterialCheckbox()
        selfieID = New MaterialSkin.Controls.MaterialTextBox()
        taskID = New MaterialSkin.Controls.MaterialTextBox()
        monitorManagement = New TabPage()
        compositorSafe = New MaterialSkin.Controls.MaterialSwitch()
        MaterialLabel26 = New MaterialSkin.Controls.MaterialLabel()
        MaterialLabel25 = New MaterialSkin.Controls.MaterialLabel()
        obscureSS = New MaterialSkin.Controls.MaterialSwitch()
        selfieMode = New MaterialSkin.Controls.MaterialSwitch()
        numSelfieInterval = New MaterialSkin.Controls.MaterialSlider()
        monitorTask = New MaterialSkin.Controls.MaterialSwitch()
        btnCleanLog = New MaterialSkin.Controls.MaterialButton()
        txtLogDir = New MaterialSkin.Controls.MaterialTextBox()
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
        TabPage1 = New TabPage()
        launchP2P = New MaterialSkin.Controls.MaterialCheckbox()
        createCLI = New MaterialSkin.Controls.MaterialButton()
        cliOutput = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        txtWorld = New MaterialSkin.Controls.MaterialTextBox()
        freshStart = New MaterialSkin.Controls.MaterialCheckbox()
        covertMode = New MaterialSkin.Controls.MaterialCheckbox()
        ramNum = New MaterialSkin.Controls.MaterialSlider()
        btnDBPath = New MaterialSkin.Controls.MaterialButton()
        dbPath = New MaterialSkin.Controls.MaterialTextBox()
        btnAddAcc = New MaterialSkin.Controls.MaterialButton()
        accountNames = New MaterialSkin.Controls.MaterialComboBox()
        ImageList1 = New ImageList(components)
        embedEditors = New MaterialSkin.Controls.MaterialButton()
        btnCheckUpdates = New MaterialSkin.Controls.MaterialButton()
        clearBtn = New MaterialSkin.Controls.MaterialButton()
        testEmbeds = New MaterialSkin.Controls.MaterialButton()
        testBtn = New MaterialSkin.Controls.MaterialButton()
        questEmbed = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        chatEmbed = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        errorEmbed = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        taskEmbed = New MaterialSkin.Controls.MaterialMultiLineTextBox()
        monitorDiscord = New MaterialSkin.Controls.MaterialButton()
        btnBackToMenu = New MaterialSkin.Controls.MaterialButton()
        Hamburger.SuspendLayout()
        discordManagement.SuspendLayout()
        monitorManagement.SuspendLayout()
        helpLinks.SuspendLayout()
        TabPage1.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnBrowseLogDir
        ' 
        btnBrowseLogDir.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnBrowseLogDir.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnBrowseLogDir.Depth = 0
        btnBrowseLogDir.HighEmphasis = True
        btnBrowseLogDir.Icon = Nothing
        btnBrowseLogDir.Location = New Point(1223, 158)
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
        btnStart.Location = New Point(10, 284)
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
        btnStop.Location = New Point(10, 332)
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
        chkMonitorChat.BackColor = SystemColors.Control
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
        chkMonitorChat.UseVisualStyleBackColor = False
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
        ' DarkModeEnabled
        ' 
        DarkModeEnabled.AutoSize = True
        DarkModeEnabled.Checked = True
        DarkModeEnabled.CheckState = CheckState.Checked
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
        numIntervalSecond.Location = New Point(915, 23)
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
        MaterialLabel1.Location = New Point(880, 7)
        MaterialLabel1.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel1.Name = "MaterialLabel1"
        MaterialLabel1.Size = New Size(434, 13)
        MaterialLabel1.TabIndex = 31
        MaterialLabel1.Text = "In order to take accurate screenshots, check interval must be set to 5 seconds, no lower or higher." & vbCrLf & vbCrLf
        MaterialLabel1.UseAccent = True
        ' 
        ' txtWebhook
        ' 
        txtWebhook.AnimateReadOnly = False
        txtWebhook.BorderStyle = BorderStyle.None
        txtWebhook.Depth = 0
        txtWebhook.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        txtWebhook.Hint = "Default Webhook (Required)"
        txtWebhook.LeadingIcon = Nothing
        txtWebhook.Location = New Point(6, 59)
        txtWebhook.MaxLength = 128
        txtWebhook.MouseState = MaterialSkin.MouseState.OUT
        txtWebhook.Multiline = False
        txtWebhook.Name = "txtWebhook"
        txtWebhook.Size = New Size(425, 50)
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
        questID.Hint = "Quest Webhook (Optional)"
        questID.LeadingIcon = Nothing
        questID.Location = New Point(437, 59)
        questID.MaxLength = 128
        questID.MouseState = MaterialSkin.MouseState.OUT
        questID.Multiline = False
        questID.Name = "questID"
        questID.Size = New Size(435, 50)
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
        chatID.Hint = "Chat Webhook (Optional)"
        chatID.LeadingIcon = Nothing
        chatID.Location = New Point(6, 112)
        chatID.MaxLength = 128
        chatID.MouseState = MaterialSkin.MouseState.OUT
        chatID.Multiline = False
        chatID.Name = "chatID"
        chatID.Size = New Size(425, 50)
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
        txtMention.Hint = "Discord User ID (Optional)"
        txtMention.LeadingIcon = Nothing
        txtMention.Location = New Point(6, 6)
        txtMention.MaxLength = 128
        txtMention.MouseState = MaterialSkin.MouseState.OUT
        txtMention.Multiline = False
        txtMention.Name = "txtMention"
        txtMention.Size = New Size(425, 50)
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
        errorID.Hint = "Error Webhook (Optional)"
        errorID.LeadingIcon = Nothing
        errorID.Location = New Point(437, 6)
        errorID.MaxLength = 128
        errorID.MouseState = MaterialSkin.MouseState.OUT
        errorID.Multiline = False
        errorID.Name = "errorID"
        errorID.Size = New Size(435, 50)
        errorID.TabIndex = 39
        errorID.Text = ""
        errorID.TrailingIcon = Nothing
        ' 
        ' combatError
        ' 
        combatError.AutoSize = True
        combatError.Depth = 0
        combatError.Location = New Point(313, 49)
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
        questError.Location = New Point(313, 86)
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
        skillIssue.Location = New Point(313, 123)
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
        txtLog.Location = New Point(185, 284)
        txtLog.MouseState = MaterialSkin.MouseState.HOVER
        txtLog.Name = "txtLog"
        txtLog.ReadOnly = True
        txtLog.Size = New Size(1139, 442)
        txtLog.TabIndex = 57
        txtLog.Text = ""
        ' 
        ' Hamburger
        ' 
        Hamburger.Appearance = TabAppearance.Buttons
        Hamburger.Controls.Add(discordManagement)
        Hamburger.Controls.Add(monitorManagement)
        Hamburger.Controls.Add(helpLinks)
        Hamburger.Controls.Add(TabPage1)
        Hamburger.Depth = 0
        Hamburger.ImageList = ImageList1
        Hamburger.Location = New Point(6, 67)
        Hamburger.MouseState = MaterialSkin.MouseState.HOVER
        Hamburger.Multiline = True
        Hamburger.Name = "Hamburger"
        Hamburger.SelectedIndex = 0
        Hamburger.Size = New Size(1318, 220)
        Hamburger.TabIndex = 58
        ' 
        ' discordManagement
        ' 
        discordManagement.Controls.Add(dtmBtn)
        discordManagement.Controls.Add(useDTM)
        discordManagement.Controls.Add(selfieID)
        discordManagement.Controls.Add(taskID)
        discordManagement.Controls.Add(errorID)
        discordManagement.Controls.Add(questID)
        discordManagement.Controls.Add(chatID)
        discordManagement.Controls.Add(txtMention)
        discordManagement.Controls.Add(txtWebhook)
        discordManagement.ImageKey = "discord.png"
        discordManagement.Location = New Point(4, 46)
        discordManagement.Name = "discordManagement"
        discordManagement.Padding = New Padding(3)
        discordManagement.Size = New Size(1310, 170)
        discordManagement.TabIndex = 0
        discordManagement.Text = "Discord Management"
        discordManagement.UseVisualStyleBackColor = True
        ' 
        ' dtmBtn
        ' 
        dtmBtn.AutoSize = False
        dtmBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        dtmBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        dtmBtn.Depth = 0
        dtmBtn.HighEmphasis = True
        dtmBtn.Icon = Nothing
        dtmBtn.Location = New Point(879, 118)
        dtmBtn.Margin = New Padding(4, 6, 4, 6)
        dtmBtn.MouseState = MaterialSkin.MouseState.HOVER
        dtmBtn.Name = "dtmBtn"
        dtmBtn.NoAccentTextColor = Color.Empty
        dtmBtn.Size = New Size(265, 36)
        dtmBtn.TabIndex = 71
        dtmBtn.Text = "Discord Thread Manager [Beta]"
        dtmBtn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        dtmBtn.UseAccentColor = False
        dtmBtn.UseVisualStyleBackColor = True
        dtmBtn.Visible = False
        ' 
        ' useDTM
        ' 
        useDTM.AutoSize = True
        useDTM.Depth = 0
        useDTM.Location = New Point(875, 64)
        useDTM.Margin = New Padding(0)
        useDTM.MouseLocation = New Point(-1, -1)
        useDTM.MouseState = MaterialSkin.MouseState.HOVER
        useDTM.Name = "useDTM"
        useDTM.ReadOnly = False
        useDTM.Ripple = True
        useDTM.Size = New Size(181, 37)
        useDTM.TabIndex = 70
        useDTM.Text = "Use Discord Threads"
        useDTM.UseVisualStyleBackColor = True
        ' 
        ' selfieID
        ' 
        selfieID.AnimateReadOnly = False
        selfieID.BorderStyle = BorderStyle.None
        selfieID.Depth = 0
        selfieID.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        selfieID.Hint = "Selfie Webhook (Optional)"
        selfieID.LeadingIcon = Nothing
        selfieID.Location = New Point(879, 6)
        selfieID.MaxLength = 128
        selfieID.MouseState = MaterialSkin.MouseState.OUT
        selfieID.Multiline = False
        selfieID.Name = "selfieID"
        selfieID.Size = New Size(425, 50)
        selfieID.TabIndex = 69
        selfieID.Text = ""
        selfieID.TrailingIcon = Nothing
        ' 
        ' taskID
        ' 
        taskID.AnimateReadOnly = False
        taskID.BorderStyle = BorderStyle.None
        taskID.Depth = 0
        taskID.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        taskID.Hint = "Task Webhook (Optional)"
        taskID.LeadingIcon = Nothing
        taskID.Location = New Point(437, 112)
        taskID.MaxLength = 128
        taskID.MouseState = MaterialSkin.MouseState.OUT
        taskID.Multiline = False
        taskID.Name = "taskID"
        taskID.Size = New Size(435, 50)
        taskID.TabIndex = 43
        taskID.Text = ""
        taskID.TrailingIcon = Nothing
        ' 
        ' monitorManagement
        ' 
        monitorManagement.Controls.Add(compositorSafe)
        monitorManagement.Controls.Add(MaterialLabel26)
        monitorManagement.Controls.Add(MaterialLabel25)
        monitorManagement.Controls.Add(obscureSS)
        monitorManagement.Controls.Add(selfieMode)
        monitorManagement.Controls.Add(numSelfieInterval)
        monitorManagement.Controls.Add(monitorTask)
        monitorManagement.Controls.Add(btnCleanLog)
        monitorManagement.Controls.Add(txtLogDir)
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
        monitorManagement.Controls.Add(DarkModeEnabled)
        monitorManagement.ImageKey = "monitoring.png"
        monitorManagement.Location = New Point(4, 46)
        monitorManagement.Name = "monitorManagement"
        monitorManagement.Padding = New Padding(3)
        monitorManagement.Size = New Size(1310, 170)
        monitorManagement.TabIndex = 1
        monitorManagement.Text = "Monitor Management"
        monitorManagement.UseVisualStyleBackColor = True
        ' 
        ' compositorSafe
        ' 
        compositorSafe.AutoSize = True
        compositorSafe.Depth = 0
        compositorSafe.Location = New Point(538, 86)
        compositorSafe.Margin = New Padding(0)
        compositorSafe.MouseLocation = New Point(-1, -1)
        compositorSafe.MouseState = MaterialSkin.MouseState.HOVER
        compositorSafe.Name = "compositorSafe"
        compositorSafe.Ripple = True
        compositorSafe.Size = New Size(304, 37)
        compositorSafe.TabIndex = 80
        compositorSafe.Text = "Screenshot Compositor Safe-Mode"
        compositorSafe.UseVisualStyleBackColor = True
        ' 
        ' MaterialLabel26
        ' 
        MaterialLabel26.AutoSize = True
        MaterialLabel26.Depth = 0
        MaterialLabel26.Font = New Font("Roboto", 10F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel26.FontType = MaterialSkin.MaterialSkinManager.fontType.Overline
        MaterialLabel26.Location = New Point(550, 62)
        MaterialLabel26.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel26.Name = "MaterialLabel26"
        MaterialLabel26.Size = New Size(208, 13)
        MaterialLabel26.TabIndex = 79
        MaterialLabel26.Text = "screenshots of errors, quests, chat, tasks, selfies"
        MaterialLabel26.UseAccent = True
        ' 
        ' MaterialLabel25
        ' 
        MaterialLabel25.AutoSize = True
        MaterialLabel25.Depth = 0
        MaterialLabel25.Font = New Font("Roboto", 10F, FontStyle.Regular, GraphicsUnit.Pixel)
        MaterialLabel25.FontType = MaterialSkin.MaterialSkinManager.fontType.Overline
        MaterialLabel25.Location = New Point(550, 49)
        MaterialLabel25.MouseState = MaterialSkin.MouseState.HOVER
        MaterialLabel25.Name = "MaterialLabel25"
        MaterialLabel25.Size = New Size(186, 13)
        MaterialLabel25.TabIndex = 78
        MaterialLabel25.Text = "Hides all important information in game for"
        MaterialLabel25.UseAccent = True
        ' 
        ' obscureSS
        ' 
        obscureSS.AutoSize = True
        obscureSS.Depth = 0
        obscureSS.Location = New Point(538, 12)
        obscureSS.Margin = New Padding(0)
        obscureSS.MouseLocation = New Point(-1, -1)
        obscureSS.MouseState = MaterialSkin.MouseState.HOVER
        obscureSS.Name = "obscureSS"
        obscureSS.Ripple = True
        obscureSS.Size = New Size(262, 37)
        obscureSS.TabIndex = 77
        obscureSS.Text = "In-Game Screenshot Obscure"
        obscureSS.UseVisualStyleBackColor = True
        ' 
        ' selfieMode
        ' 
        selfieMode.AutoSize = True
        selfieMode.Depth = 0
        selfieMode.Location = New Point(313, 159)
        selfieMode.Margin = New Padding(0)
        selfieMode.MouseLocation = New Point(-1, -1)
        selfieMode.MouseState = MaterialSkin.MouseState.HOVER
        selfieMode.Name = "selfieMode"
        selfieMode.Ripple = True
        selfieMode.Size = New Size(142, 37)
        selfieMode.TabIndex = 76
        selfieMode.Text = "Selfie Mode"
        selfieMode.UseVisualStyleBackColor = True
        ' 
        ' numSelfieInterval
        ' 
        numSelfieInterval.Depth = 0
        numSelfieInterval.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        numSelfieInterval.Location = New Point(915, 58)
        numSelfieInterval.MouseState = MaterialSkin.MouseState.HOVER
        numSelfieInterval.Name = "numSelfieInterval"
        numSelfieInterval.RangeMax = 60
        numSelfieInterval.Size = New Size(359, 40)
        numSelfieInterval.TabIndex = 75
        numSelfieInterval.Text = "Selfie Interval"
        numSelfieInterval.UseAccentColor = True
        numSelfieInterval.Value = 10
        numSelfieInterval.ValueSuffix = " Minutes"
        ' 
        ' monitorTask
        ' 
        monitorTask.AutoSize = True
        monitorTask.Depth = 0
        monitorTask.Location = New Point(313, 12)
        monitorTask.Margin = New Padding(0)
        monitorTask.MouseLocation = New Point(-1, -1)
        monitorTask.MouseState = MaterialSkin.MouseState.HOVER
        monitorTask.Name = "monitorTask"
        monitorTask.Ripple = True
        monitorTask.Size = New Size(160, 37)
        monitorTask.TabIndex = 74
        monitorTask.Text = "Monitor Tasks"
        monitorTask.UseVisualStyleBackColor = True
        ' 
        ' btnCleanLog
        ' 
        btnCleanLog.AutoSize = False
        btnCleanLog.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnCleanLog.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnCleanLog.Depth = 0
        btnCleanLog.HighEmphasis = True
        btnCleanLog.Icon = Nothing
        btnCleanLog.Location = New Point(915, 158)
        btnCleanLog.Margin = New Padding(4, 6, 4, 6)
        btnCleanLog.MouseState = MaterialSkin.MouseState.HOVER
        btnCleanLog.Name = "btnCleanLog"
        btnCleanLog.NoAccentTextColor = Color.Empty
        btnCleanLog.Size = New Size(168, 36)
        btnCleanLog.TabIndex = 67
        btnCleanLog.Text = "Clean Log File"
        btnCleanLog.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnCleanLog.UseAccentColor = False
        btnCleanLog.UseVisualStyleBackColor = True
        ' 
        ' txtLogDir
        ' 
        txtLogDir.AnimateReadOnly = False
        txtLogDir.BorderStyle = BorderStyle.None
        txtLogDir.Depth = 0
        txtLogDir.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        txtLogDir.LeadingIcon = Nothing
        txtLogDir.Location = New Point(915, 99)
        txtLogDir.MaxLength = 50
        txtLogDir.MouseState = MaterialSkin.MouseState.OUT
        txtLogDir.Multiline = False
        txtLogDir.Name = "txtLogDir"
        txtLogDir.Size = New Size(389, 50)
        txtLogDir.TabIndex = 52
        txtLogDir.Text = ""
        txtLogDir.TrailingIcon = Nothing
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
        helpLinks.Size = New Size(1310, 170)
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
        ' TabPage1
        ' 
        TabPage1.Controls.Add(launchP2P)
        TabPage1.Controls.Add(createCLI)
        TabPage1.Controls.Add(cliOutput)
        TabPage1.Controls.Add(txtWorld)
        TabPage1.Controls.Add(freshStart)
        TabPage1.Controls.Add(covertMode)
        TabPage1.Controls.Add(ramNum)
        TabPage1.Controls.Add(btnDBPath)
        TabPage1.Controls.Add(dbPath)
        TabPage1.Controls.Add(btnAddAcc)
        TabPage1.Controls.Add(accountNames)
        TabPage1.ImageKey = "copy-writing.png"
        TabPage1.Location = New Point(4, 46)
        TabPage1.Name = "TabPage1"
        TabPage1.Padding = New Padding(3)
        TabPage1.RightToLeft = RightToLeft.Yes
        TabPage1.Size = New Size(1310, 170)
        TabPage1.TabIndex = 3
        TabPage1.Text = "CLI Creator [Beta]"
        TabPage1.UseVisualStyleBackColor = True
        ' 
        ' launchP2P
        ' 
        launchP2P.AutoSize = True
        launchP2P.Depth = 0
        launchP2P.Location = New Point(377, 69)
        launchP2P.Margin = New Padding(0)
        launchP2P.MouseLocation = New Point(-1, -1)
        launchP2P.MouseState = MaterialSkin.MouseState.HOVER
        launchP2P.Name = "launchP2P"
        launchP2P.ReadOnly = False
        launchP2P.Ripple = True
        launchP2P.Size = New Size(166, 37)
        launchP2P.TabIndex = 10
        launchP2P.Text = "Launch P2P Script"
        launchP2P.UseVisualStyleBackColor = True
        ' 
        ' createCLI
        ' 
        createCLI.AutoSize = False
        createCLI.AutoSizeMode = AutoSizeMode.GrowAndShrink
        createCLI.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        createCLI.Depth = 0
        createCLI.HighEmphasis = True
        createCLI.Icon = Nothing
        createCLI.Location = New Point(338, 118)
        createCLI.Margin = New Padding(4, 6, 4, 6)
        createCLI.MouseState = MaterialSkin.MouseState.HOVER
        createCLI.Name = "createCLI"
        createCLI.NoAccentTextColor = Color.Empty
        createCLI.Size = New Size(288, 36)
        createCLI.TabIndex = 9
        createCLI.Text = "Create CLI"
        createCLI.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        createCLI.UseAccentColor = False
        createCLI.UseVisualStyleBackColor = True
        ' 
        ' cliOutput
        ' 
        cliOutput.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        cliOutput.BorderStyle = BorderStyle.None
        cliOutput.Depth = 0
        cliOutput.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        cliOutput.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        cliOutput.Hint = "Output"
        cliOutput.Location = New Point(633, 6)
        cliOutput.MouseState = MaterialSkin.MouseState.HOVER
        cliOutput.Name = "cliOutput"
        cliOutput.RightToLeft = RightToLeft.No
        cliOutput.Size = New Size(671, 158)
        cliOutput.TabIndex = 8
        cliOutput.Text = ""
        ' 
        ' txtWorld
        ' 
        txtWorld.AnimateReadOnly = False
        txtWorld.BorderStyle = BorderStyle.None
        txtWorld.Depth = 0
        txtWorld.Enabled = False
        txtWorld.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        txtWorld.Hint = "Default World (Optional)"
        txtWorld.LeadingIcon = Nothing
        txtWorld.Location = New Point(6, 117)
        txtWorld.MaxLength = 50
        txtWorld.MouseState = MaterialSkin.MouseState.OUT
        txtWorld.Multiline = False
        txtWorld.Name = "txtWorld"
        txtWorld.RightToLeft = RightToLeft.No
        txtWorld.Size = New Size(325, 50)
        txtWorld.TabIndex = 7
        txtWorld.Text = ""
        txtWorld.TrailingIcon = Nothing
        ' 
        ' freshStart
        ' 
        freshStart.AutoSize = True
        freshStart.Depth = 0
        freshStart.Location = New Point(515, 37)
        freshStart.Margin = New Padding(0)
        freshStart.MouseLocation = New Point(-1, -1)
        freshStart.MouseState = MaterialSkin.MouseState.HOVER
        freshStart.Name = "freshStart"
        freshStart.ReadOnly = False
        freshStart.Ripple = True
        freshStart.Size = New Size(98, 37)
        freshStart.TabIndex = 6
        freshStart.Text = "No Fresh"
        freshStart.UseVisualStyleBackColor = True
        ' 
        ' covertMode
        ' 
        covertMode.AutoSize = True
        covertMode.Depth = 0
        covertMode.Location = New Point(377, 37)
        covertMode.Margin = New Padding(0)
        covertMode.MouseLocation = New Point(-1, -1)
        covertMode.MouseState = MaterialSkin.MouseState.HOVER
        covertMode.Name = "covertMode"
        covertMode.ReadOnly = False
        covertMode.Ripple = True
        covertMode.Size = New Size(124, 37)
        covertMode.TabIndex = 5
        covertMode.Text = "Covert Mode"
        covertMode.UseVisualStyleBackColor = True
        ' 
        ' ramNum
        ' 
        ramNum.Depth = 0
        ramNum.Font = New Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel)
        ramNum.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        ramNum.Location = New Point(377, 6)
        ramNum.MouseState = MaterialSkin.MouseState.HOVER
        ramNum.Name = "ramNum"
        ramNum.RangeMax = 2560
        ramNum.RangeMin = 512
        ramNum.Size = New Size(250, 40)
        ramNum.TabIndex = 4
        ramNum.Text = "RAM Amount"
        ramNum.UseAccentColor = True
        ramNum.Value = 512
        ramNum.ValueMax = 2560
        ramNum.ValueSuffix = "mb"
        ' 
        ' btnDBPath
        ' 
        btnDBPath.AutoSize = False
        btnDBPath.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnDBPath.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnDBPath.Depth = 0
        btnDBPath.HighEmphasis = True
        btnDBPath.Icon = Nothing
        btnDBPath.Location = New Point(338, 57)
        btnDBPath.Margin = New Padding(4, 6, 4, 6)
        btnDBPath.MouseState = MaterialSkin.MouseState.HOVER
        btnDBPath.Name = "btnDBPath"
        btnDBPath.NoAccentTextColor = Color.Empty
        btnDBPath.Size = New Size(30, 49)
        btnDBPath.TabIndex = 3
        btnDBPath.Text = "+"
        btnDBPath.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnDBPath.UseAccentColor = False
        btnDBPath.UseVisualStyleBackColor = True
        ' 
        ' dbPath
        ' 
        dbPath.AnimateReadOnly = False
        dbPath.BorderStyle = BorderStyle.None
        dbPath.Depth = 0
        dbPath.Font = New Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel)
        dbPath.Hint = "Dreambot Launcher.jar Path"
        dbPath.LeadingIcon = Nothing
        dbPath.Location = New Point(6, 61)
        dbPath.MaxLength = 50
        dbPath.MouseState = MaterialSkin.MouseState.OUT
        dbPath.Multiline = False
        dbPath.Name = "dbPath"
        dbPath.RightToLeft = RightToLeft.No
        dbPath.Size = New Size(325, 50)
        dbPath.TabIndex = 2
        dbPath.Text = ""
        dbPath.TrailingIcon = Nothing
        ' 
        ' btnAddAcc
        ' 
        btnAddAcc.AutoSize = False
        btnAddAcc.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnAddAcc.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnAddAcc.Depth = 0
        btnAddAcc.HighEmphasis = True
        btnAddAcc.Icon = Nothing
        btnAddAcc.Location = New Point(338, 2)
        btnAddAcc.Margin = New Padding(4, 6, 4, 6)
        btnAddAcc.MouseState = MaterialSkin.MouseState.HOVER
        btnAddAcc.Name = "btnAddAcc"
        btnAddAcc.NoAccentTextColor = Color.Empty
        btnAddAcc.Size = New Size(30, 49)
        btnAddAcc.TabIndex = 1
        btnAddAcc.Text = "+"
        btnAddAcc.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnAddAcc.UseAccentColor = False
        btnAddAcc.UseVisualStyleBackColor = True
        ' 
        ' accountNames
        ' 
        accountNames.AutoResize = False
        accountNames.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        accountNames.Depth = 0
        accountNames.DrawMode = DrawMode.OwnerDrawVariable
        accountNames.DropDownHeight = 174
        accountNames.DropDownStyle = ComboBoxStyle.DropDownList
        accountNames.DropDownWidth = 121
        accountNames.Font = New Font("Roboto Medium", 14F, FontStyle.Bold, GraphicsUnit.Pixel)
        accountNames.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        accountNames.FormattingEnabled = True
        accountNames.Hint = "Dreambot Account Manager Nicknames"
        accountNames.IntegralHeight = False
        accountNames.ItemHeight = 43
        accountNames.Location = New Point(6, 6)
        accountNames.MaxDropDownItems = 4
        accountNames.MouseState = MaterialSkin.MouseState.OUT
        accountNames.Name = "accountNames"
        accountNames.RightToLeft = RightToLeft.No
        accountNames.Size = New Size(325, 49)
        accountNames.StartIndex = 0
        accountNames.TabIndex = 0
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
        ' embedEditors
        ' 
        embedEditors.AutoSize = False
        embedEditors.AutoSizeMode = AutoSizeMode.GrowAndShrink
        embedEditors.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        embedEditors.Depth = 0
        embedEditors.HighEmphasis = True
        embedEditors.Icon = Nothing
        embedEditors.Location = New Point(10, 476)
        embedEditors.Margin = New Padding(4, 6, 4, 6)
        embedEditors.MouseState = MaterialSkin.MouseState.HOVER
        embedEditors.Name = "embedEditors"
        embedEditors.NoAccentTextColor = Color.Empty
        embedEditors.RightToLeft = RightToLeft.Yes
        embedEditors.Size = New Size(168, 36)
        embedEditors.TabIndex = 68
        embedEditors.Text = "Embed Editor"
        embedEditors.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        embedEditors.UseAccentColor = False
        embedEditors.UseVisualStyleBackColor = True
        ' 
        ' btnCheckUpdates
        ' 
        btnCheckUpdates.AutoSize = False
        btnCheckUpdates.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnCheckUpdates.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnCheckUpdates.Depth = 0
        btnCheckUpdates.HighEmphasis = True
        btnCheckUpdates.Icon = Nothing
        btnCheckUpdates.Location = New Point(10, 524)
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
        ' clearBtn
        ' 
        clearBtn.AutoSize = False
        clearBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        clearBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        clearBtn.Depth = 0
        clearBtn.HighEmphasis = True
        clearBtn.Icon = Nothing
        clearBtn.Location = New Point(10, 572)
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
        ' testEmbeds
        ' 
        testEmbeds.AutoSize = False
        testEmbeds.AutoSizeMode = AutoSizeMode.GrowAndShrink
        testEmbeds.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        testEmbeds.Depth = 0
        testEmbeds.HighEmphasis = True
        testEmbeds.Icon = Nothing
        testEmbeds.Location = New Point(10, 428)
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
        ' testBtn
        ' 
        testBtn.AutoSize = False
        testBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink
        testBtn.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        testBtn.Depth = 0
        testBtn.HighEmphasis = True
        testBtn.Icon = Nothing
        testBtn.Location = New Point(10, 380)
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
        ' questEmbed
        ' 
        questEmbed.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        questEmbed.BorderStyle = BorderStyle.None
        questEmbed.Depth = 0
        questEmbed.Font = New Font("Roboto", 10.5F)
        questEmbed.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        questEmbed.Location = New Point(1214, 297)
        questEmbed.MouseState = MaterialSkin.MouseState.HOVER
        questEmbed.Name = "questEmbed"
        questEmbed.Size = New Size(12, 10)
        questEmbed.TabIndex = 63
        questEmbed.Text = resources.GetString("questEmbed.Text")
        questEmbed.Visible = False
        ' 
        ' chatEmbed
        ' 
        chatEmbed.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        chatEmbed.BorderStyle = BorderStyle.None
        chatEmbed.Depth = 0
        chatEmbed.Font = New Font("Roboto", 10.5F)
        chatEmbed.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        chatEmbed.Location = New Point(1194, 297)
        chatEmbed.MouseState = MaterialSkin.MouseState.HOVER
        chatEmbed.Name = "chatEmbed"
        chatEmbed.Size = New Size(12, 10)
        chatEmbed.TabIndex = 64
        chatEmbed.Text = resources.GetString("chatEmbed.Text")
        chatEmbed.Visible = False
        ' 
        ' errorEmbed
        ' 
        errorEmbed.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        errorEmbed.BorderStyle = BorderStyle.None
        errorEmbed.Depth = 0
        errorEmbed.Font = New Font("Roboto", 10.5F)
        errorEmbed.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        errorEmbed.Location = New Point(1176, 297)
        errorEmbed.MouseState = MaterialSkin.MouseState.HOVER
        errorEmbed.Name = "errorEmbed"
        errorEmbed.Size = New Size(12, 10)
        errorEmbed.TabIndex = 66
        errorEmbed.Text = resources.GetString("errorEmbed.Text")
        errorEmbed.Visible = False
        ' 
        ' taskEmbed
        ' 
        taskEmbed.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        taskEmbed.BorderStyle = BorderStyle.None
        taskEmbed.Depth = 0
        taskEmbed.Font = New Font("Roboto", 10.5F)
        taskEmbed.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        taskEmbed.Location = New Point(1232, 297)
        taskEmbed.MouseState = MaterialSkin.MouseState.HOVER
        taskEmbed.Name = "taskEmbed"
        taskEmbed.Size = New Size(12, 10)
        taskEmbed.TabIndex = 67
        taskEmbed.Text = resources.GetString("taskEmbed.Text")
        taskEmbed.Visible = False
        ' 
        ' monitorDiscord
        ' 
        monitorDiscord.AutoSize = False
        monitorDiscord.AutoSizeMode = AutoSizeMode.GrowAndShrink
        monitorDiscord.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        monitorDiscord.Depth = 0
        monitorDiscord.HighEmphasis = True
        monitorDiscord.Icon = Nothing
        monitorDiscord.Location = New Point(10, 687)
        monitorDiscord.Margin = New Padding(4, 6, 4, 6)
        monitorDiscord.MouseState = MaterialSkin.MouseState.HOVER
        monitorDiscord.Name = "monitorDiscord"
        monitorDiscord.NoAccentTextColor = Color.Empty
        monitorDiscord.Size = New Size(168, 36)
        monitorDiscord.TabIndex = 68
        monitorDiscord.Text = "Join Monitor Discord"
        monitorDiscord.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        monitorDiscord.UseAccentColor = True
        monitorDiscord.UseVisualStyleBackColor = True
        ' 
        ' btnBackToMenu
        ' 
        btnBackToMenu.AutoSize = False
        btnBackToMenu.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnBackToMenu.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnBackToMenu.Depth = 0
        btnBackToMenu.HighEmphasis = True
        btnBackToMenu.Icon = Nothing
        btnBackToMenu.Location = New Point(10, 639)
        btnBackToMenu.Margin = New Padding(4, 6, 4, 6)
        btnBackToMenu.MouseState = MaterialSkin.MouseState.HOVER
        btnBackToMenu.Name = "btnBackToMenu"
        btnBackToMenu.NoAccentTextColor = Color.Empty
        btnBackToMenu.Size = New Size(168, 36)
        btnBackToMenu.TabIndex = 69
        btnBackToMenu.Text = "Main Menu"
        btnBackToMenu.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnBackToMenu.UseAccentColor = False
        btnBackToMenu.UseVisualStyleBackColor = True
        ' 
        ' main
        ' 
        AutoScaleMode = AutoScaleMode.None
        ClientSize = New Size(1330, 732)
        Controls.Add(btnBackToMenu)
        Controls.Add(monitorDiscord)
        Controls.Add(taskEmbed)
        Controls.Add(btnCheckUpdates)
        Controls.Add(embedEditors)
        Controls.Add(clearBtn)
        Controls.Add(txtLog)
        Controls.Add(testEmbeds)
        Controls.Add(btnStart)
        Controls.Add(errorEmbed)
        Controls.Add(testBtn)
        Controls.Add(btnStop)
        Controls.Add(chatEmbed)
        Controls.Add(questEmbed)
        Controls.Add(Hamburger)
        DrawerBackgroundWithAccent = True
        DrawerTabControl = Hamburger
        DrawerUseColors = True
        DrawerWidth = 220
        Font = New Font("Roboto", 15.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "main"
        Sizable = False
        Text = "P2P Monitor v1.4.1 by CaS5"
        Hamburger.ResumeLayout(False)
        discordManagement.ResumeLayout(False)
        discordManagement.PerformLayout()
        monitorManagement.ResumeLayout(False)
        monitorManagement.PerformLayout()
        helpLinks.ResumeLayout(False)
        TabPage1.ResumeLayout(False)
        TabPage1.PerformLayout()
        ResumeLayout(False)
    End Sub
    Friend WithEvents btnStop As MaterialSkin.Controls.MaterialButton
    Friend WithEvents btnStart As MaterialSkin.Controls.MaterialButton
    Friend WithEvents btnBrowseLogDir As MaterialSkin.Controls.MaterialButton
    Friend WithEvents chkMonitorChat As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents monitorQuest As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents captureWin As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents autoClean As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents DarkModeEnabled As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents numIntervalSecond As MaterialSkin.Controls.MaterialSlider
    Friend WithEvents MaterialLabel1 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents txtWebhook As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents questID As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents chatID As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents txtMention As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents errorID As MaterialSkin.Controls.MaterialTextBox
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
    Friend WithEvents txtLogDir As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents testBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents testEmbeds As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialMultiLineTextBox1 As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents questEmbed As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents chatEmbed As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents btnCheckUpdates As MaterialSkin.Controls.MaterialButton
    Friend WithEvents errorEmbed As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents btnCleanLog As MaterialSkin.Controls.MaterialButton
    Friend WithEvents monitorTask As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents createCLI As MaterialSkin.Controls.MaterialButton
    Friend WithEvents taskEmbed As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents numSelfieInterval As MaterialSkin.Controls.MaterialSlider
    Friend WithEvents selfieMode As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents MaterialButton2 As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialTextBox11 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialTextBox12 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialButton7 As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialTextBox9 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialTextBox10 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialButton6 As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialTextBox7 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialTextBox8 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialButton5 As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialTextBox5 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialTextBox6 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialButton4 As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialTextBox3 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialTextBox4 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialButton3 As MaterialSkin.Controls.MaterialButton
    Friend WithEvents MaterialTextBox2 As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents txtWorld As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents ramNum As MaterialSkin.Controls.MaterialSlider
    Friend WithEvents btnDBPath As MaterialSkin.Controls.MaterialButton
    Friend WithEvents dbPath As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents btnAddAcc As MaterialSkin.Controls.MaterialButton
    Friend WithEvents accountNames As MaterialSkin.Controls.MaterialComboBox
    Friend WithEvents freshStart As MaterialSkin.Controls.MaterialCheckbox
    Friend WithEvents covertMode As MaterialSkin.Controls.MaterialCheckbox
    Friend WithEvents cliOutput As MaterialSkin.Controls.MaterialMultiLineTextBox
    Friend WithEvents embedEditors As MaterialSkin.Controls.MaterialButton
    Friend WithEvents taskID As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents selfieID As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents MaterialSwitch1 As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents MaterialLabel26 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents MaterialLabel25 As MaterialSkin.Controls.MaterialLabel
    Friend WithEvents obscureSS As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents monitorDiscord As MaterialSkin.Controls.MaterialButton
    Friend WithEvents launchP2P As MaterialSkin.Controls.MaterialCheckbox
    Friend WithEvents dtmBtn As MaterialSkin.Controls.MaterialButton
    Friend WithEvents useDTM As MaterialSkin.Controls.MaterialCheckbox
    Friend WithEvents compositorSafe As MaterialSkin.Controls.MaterialSwitch
    Friend WithEvents btnBackToMenu As MaterialSkin.Controls.MaterialButton
End Class
