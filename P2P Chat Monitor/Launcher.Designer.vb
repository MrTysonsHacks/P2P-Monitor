' Launcher.Designer.vb
' Partial designer class for the launcher “main menu”
' Put this in the same folder as Launcher.vb

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Launcher
    Inherits MaterialSkin.Controls.MaterialForm

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Launcher))
        btnMonitor = New MaterialSkin.Controls.MaterialButton()
        btnScanner = New MaterialSkin.Controls.MaterialButton()
        picLogo = New PictureBox()
        saveChoice = New MaterialSkin.Controls.MaterialSwitch()
        CType(picLogo, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnMonitor
        ' 
        btnMonitor.AutoSize = False
        btnMonitor.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnMonitor.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnMonitor.Depth = 0
        btnMonitor.HighEmphasis = True
        btnMonitor.Icon = Nothing
        btnMonitor.Location = New Point(7, 130)
        btnMonitor.Margin = New Padding(4, 6, 4, 6)
        btnMonitor.MouseState = MaterialSkin.MouseState.HOVER
        btnMonitor.Name = "btnMonitor"
        btnMonitor.NoAccentTextColor = Color.Empty
        btnMonitor.Size = New Size(220, 48)
        btnMonitor.TabIndex = 1
        btnMonitor.Text = "P2P Realtime Monitor"
        btnMonitor.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnMonitor.UseAccentColor = False
        btnMonitor.UseVisualStyleBackColor = True
        ' 
        ' btnScanner
        ' 
        btnScanner.AutoSize = False
        btnScanner.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnScanner.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default
        btnScanner.Depth = 0
        btnScanner.HighEmphasis = True
        btnScanner.Icon = Nothing
        btnScanner.Location = New Point(7, 70)
        btnScanner.Margin = New Padding(4, 6, 4, 6)
        btnScanner.MouseState = MaterialSkin.MouseState.HOVER
        btnScanner.Name = "btnScanner"
        btnScanner.NoAccentTextColor = Color.Empty
        btnScanner.Size = New Size(220, 48)
        btnScanner.TabIndex = 2
        btnScanner.Text = "P2P Log Scanner"
        btnScanner.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained
        btnScanner.UseAccentColor = False
        btnScanner.UseVisualStyleBackColor = True
        ' 
        ' picLogo
        ' 
        picLogo.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        picLogo.BackColor = Color.Transparent
        picLogo.Image = CType(resources.GetObject("picLogo.Image"), Image)
        picLogo.Location = New Point(233, 67)
        picLogo.Name = "picLogo"
        picLogo.Size = New Size(197, 184)
        picLogo.SizeMode = PictureBoxSizeMode.Zoom
        picLogo.TabIndex = 3
        picLogo.TabStop = False
        ' 
        ' saveChoice
        ' 
        saveChoice.AutoSize = True
        saveChoice.Depth = 0
        saveChoice.Location = New Point(21, 201)
        saveChoice.Margin = New Padding(0)
        saveChoice.MouseLocation = New Point(-1, -1)
        saveChoice.MouseState = MaterialSkin.MouseState.HOVER
        saveChoice.Name = "saveChoice"
        saveChoice.Ripple = True
        saveChoice.Size = New Size(186, 37)
        saveChoice.TabIndex = 4
        saveChoice.Text = "Remember Choice"
        saveChoice.UseVisualStyleBackColor = True
        ' 
        ' Launcher
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(436, 257)
        Controls.Add(saveChoice)
        Controls.Add(picLogo)
        Controls.Add(btnScanner)
        Controls.Add(btnMonitor)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "Launcher"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Choose A Tool..."
        CType(picLogo, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents btnMonitor As MaterialSkin.Controls.MaterialButton
    Friend WithEvents btnScanner As MaterialSkin.Controls.MaterialButton
    Friend WithEvents picLogo As System.Windows.Forms.PictureBox
    Friend WithEvents saveChoice As MaterialSkin.Controls.MaterialSwitch
End Class
