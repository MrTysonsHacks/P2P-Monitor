<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DiscordThreadManager
    Inherits MaterialSkin.Controls.MaterialForm

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub
    Private Class DTMOpts
        Public Chats As Boolean = True
        Public Tasks As Boolean = True
        Public Quests As Boolean = True
        Public Errors As Boolean = True
        Public Selfies As Boolean = True
    End Class

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DiscordThreadManager))
        canvas = New Panel()
        managedAccount1 = New MaterialSkin.Controls.MaterialCard()
        accounts = New MaterialSkin.Controls.MaterialComboBox()
        deleteDTM = New MaterialSkin.Controls.MaterialFloatingActionButton()
        txtWebhook = New MaterialSkin.Controls.MaterialTextBox()
        btnThreadIDs = New MaterialSkin.Controls.MaterialButton()
        btnMonitorOptions = New MaterialSkin.Controls.MaterialButton()
        add1 = New MaterialSkin.Controls.MaterialButton()
        canvas.SuspendLayout()
        managedAccount1.SuspendLayout()
        SuspendLayout()
        ' 
        ' canvas
        ' 
        canvas.Controls.Add(managedAccount1)
        canvas.Controls.Add(add1)
        canvas.Dock = DockStyle.Fill
        canvas.Font = New Font("Roboto", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        canvas.Location = New Point(3, 64)
        canvas.Name = "canvas"
        canvas.Size = New Size(857, 598)
        canvas.TabIndex = 0
        ' 
        ' managedAccount1
        ' 
        managedAccount1.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        managedAccount1.Controls.Add(accounts)
        managedAccount1.Controls.Add(deleteDTM)
        managedAccount1.Controls.Add(txtWebhook)
        managedAccount1.Controls.Add(btnThreadIDs)
        managedAccount1.Controls.Add(btnMonitorOptions)
        managedAccount1.Depth = 0
        managedAccount1.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        managedAccount1.Location = New Point(4, 52)
        managedAccount1.Margin = New Padding(14)
        managedAccount1.MouseState = MaterialSkin.MouseState.HOVER
        managedAccount1.Name = "managedAccount1"
        managedAccount1.Padding = New Padding(14)
        managedAccount1.Size = New Size(848, 82)
        managedAccount1.TabIndex = 10
        ' 
        ' accounts
        ' 
        accounts.AutoResize = False
        accounts.BackColor = Color.FromArgb(CByte(255), CByte(255), CByte(255))
        accounts.Depth = 0
        accounts.DrawMode = DrawMode.OwnerDrawVariable
        accounts.DropDownHeight = 174
        accounts.DropDownStyle = ComboBoxStyle.DropDownList
        accounts.DropDownWidth = 121
        accounts.Font = New Font("Roboto Medium", 14.0F, FontStyle.Bold, GraphicsUnit.Pixel)
        accounts.ForeColor = Color.FromArgb(CByte(222), CByte(0), CByte(0), CByte(0))
        accounts.FormattingEnabled = True
        accounts.Hint = "Account"
        accounts.IntegralHeight = False
        accounts.ItemHeight = 43
        accounts.Location = New Point(17, 17)
        accounts.MaxDropDownItems = 4
        accounts.MouseState = MaterialSkin.MouseState.OUT
        accounts.Name = "accounts"
        accounts.Size = New Size(146, 49)
        accounts.StartIndex = 0
        accounts.TabIndex = 1
        ' 
        ' deleteDTM
        ' 
        deleteDTM.AutoSizeMode = AutoSizeMode.GrowAndShrink
        deleteDTM.Depth = 0
        deleteDTM.Icon = My.Resources.Resources.trash_can
        deleteDTM.Location = New Point(801, 20)
        deleteDTM.Mini = True
        deleteDTM.MouseState = MaterialSkin.MouseState.HOVER
        deleteDTM.Name = "deleteDTM"
        deleteDTM.Size = New Size(40, 40)
        deleteDTM.TabIndex = 9
        deleteDTM.Text = "Test"
        deleteDTM.UseVisualStyleBackColor = False
        ' 
        ' txtWebhook
        ' 
        txtWebhook.AnimateReadOnly = False
        txtWebhook.BorderStyle = BorderStyle.None
        txtWebhook.Depth = 0
        txtWebhook.Font = New Font("Roboto", 16.0F, FontStyle.Regular, GraphicsUnit.Pixel)
        txtWebhook.Hint = "Thread Webhook"
        txtWebhook.LeadingIcon = Nothing
        txtWebhook.LeaveOnEnterKey = True
        txtWebhook.Location = New Point(169, 17)
        txtWebhook.MaxLength = 200
        txtWebhook.MouseState = MaterialSkin.MouseState.OUT
        txtWebhook.Multiline = False
        txtWebhook.Name = "txtWebhook"
        txtWebhook.Size = New Size(350, 50)
        txtWebhook.TabIndex = 6
        txtWebhook.Text = ""
        txtWebhook.TrailingIcon = Nothing
        ' 
        ' btnThreadIDs
        ' 
        btnThreadIDs.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnThreadIDs.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnThreadIDs.Depth = 0
        btnThreadIDs.HighEmphasis = True
        btnThreadIDs.Icon = Nothing
        btnThreadIDs.Location = New Point(687, 22)
        btnThreadIDs.Margin = New Padding(4, 6, 4, 6)
        btnThreadIDs.MouseState = MaterialSkin.MouseState.HOVER
        btnThreadIDs.Name = "btnThreadIDs"
        btnThreadIDs.NoAccentTextColor = Color.Empty
        btnThreadIDs.Size = New Size(107, 36)
        btnThreadIDs.TabIndex = 8
        btnThreadIDs.Text = "Thread ID's"
        btnThreadIDs.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnThreadIDs.UseAccentColor = False
        btnThreadIDs.UseVisualStyleBackColor = True
        ' 
        ' btnMonitorOptions
        ' 
        btnMonitorOptions.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnMonitorOptions.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnMonitorOptions.Depth = 0
        btnMonitorOptions.HighEmphasis = True
        btnMonitorOptions.Icon = Nothing
        btnMonitorOptions.Location = New Point(526, 22)
        btnMonitorOptions.Margin = New Padding(4, 6, 4, 6)
        btnMonitorOptions.MouseState = MaterialSkin.MouseState.HOVER
        btnMonitorOptions.Name = "btnMonitorOptions"
        btnMonitorOptions.NoAccentTextColor = Color.Empty
        btnMonitorOptions.Size = New Size(153, 36)
        btnMonitorOptions.TabIndex = 7
        btnMonitorOptions.Text = "Monitor Options"
        btnMonitorOptions.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnMonitorOptions.UseAccentColor = False
        btnMonitorOptions.UseVisualStyleBackColor = True
        ' 
        ' add1
        ' 
        add1.AutoSize = False
        add1.AutoSizeMode = AutoSizeMode.GrowAndShrink
        add1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        add1.Depth = 0
        add1.HighEmphasis = True
        add1.Icon = Nothing
        add1.Location = New Point(4, 6)
        add1.Margin = New Padding(4, 6, 4, 6)
        add1.MouseState = MaterialSkin.MouseState.HOVER
        add1.Name = "add1"
        add1.NoAccentTextColor = Color.Empty
        add1.Size = New Size(177, 36)
        add1.TabIndex = 0
        add1.Text = "Add Managed Account"
        add1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        add1.UseAccentColor = False
        add1.UseVisualStyleBackColor = True
        ' 
        ' DiscordThreadManager
        ' 
        AutoScaleDimensions = New SizeF(96.0F, 96.0F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(863, 665)
        Controls.Add(canvas)
        Font = New Font("Roboto", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        MinimumSize = New Size(800, 650)
        Name = "DiscordThreadManager"
        Sizable = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Discord Thread Manager"
        canvas.ResumeLayout(False)
        managedAccount1.ResumeLayout(False)
        managedAccount1.PerformLayout()
        ResumeLayout(False)

    End Sub

    Friend WithEvents canvas As System.Windows.Forms.Panel
    Friend WithEvents txtWebhook As MaterialSkin.Controls.MaterialTextBox
    Friend WithEvents accounts As MaterialSkin.Controls.MaterialComboBox
    Friend WithEvents add1 As MaterialSkin.Controls.MaterialButton
    Friend WithEvents deleteDTM As MaterialSkin.Controls.MaterialFloatingActionButton
    Friend WithEvents btnThreadIDs As MaterialSkin.Controls.MaterialButton
    Friend WithEvents btnMonitorOptions As MaterialSkin.Controls.MaterialButton
    Friend WithEvents managedAccount1 As MaterialSkin.Controls.MaterialCard
End Class
