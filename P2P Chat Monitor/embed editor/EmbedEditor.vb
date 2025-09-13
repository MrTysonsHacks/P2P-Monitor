Imports MaterialSkin
Imports MaterialSkin.Controls

Public Class EmbedEditor
    Inherits MaterialForm

    Public Property ResultText As String
    Public Property DefaultTemplate As String

    Private txtEditor As MaterialMultiLineTextBox
    Private btnSave As MaterialButton
    Private btnReset As MaterialButton

    Public Sub New(initialText As String, title As String, defaultTemplate As String, darkMode As Boolean)
        ' Apply MaterialSkin styling
        Dim skinManager = MaterialSkinManager.Instance
        skinManager.AddFormToManage(Me)
        If darkMode Then
            skinManager.Theme = MaterialSkinManager.Themes.DARK
            skinManager.ColorScheme = New ColorScheme(Primary.Grey800, Primary.Grey900, Primary.Grey500, Accent.LightBlue200, TextShade.WHITE)
        Else
            skinManager.Theme = MaterialSkinManager.Themes.LIGHT
            skinManager.ColorScheme = New ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE)
        End If

        Me.Text = title
        Me.Size = New Size(600, 450)
        Me.DefaultTemplate = defaultTemplate

        ' Textbox
        txtEditor = New MaterialMultiLineTextBox() With {
            .Dock = DockStyle.Top,
            .Height = 350,
            .Text = initialText,
            .ScrollBars = ScrollBars.Both
        }

        btnSave = New MaterialButton() With {
            .Text = "Save",
            .Dock = DockStyle.Left
        }
        AddHandler btnSave.Click, AddressOf SaveAndClose

        ' Reset button
        btnReset = New MaterialButton() With {
            .Text = "Reset",
            .Dock = DockStyle.Right
        }
        AddHandler btnReset.Click, AddressOf ResetToDefault

        ' Panel to hold buttons
        Dim pnl As New FlowLayoutPanel() With {
            .Dock = DockStyle.Bottom,
            .Height = 40
        }
        pnl.Controls.AddRange(New Control() {btnSave, btnReset})

        Me.Controls.Add(txtEditor)
        Me.Controls.Add(pnl)
    End Sub

    Private Sub SaveAndClose(sender As Object, e As EventArgs)
        ResultText = txtEditor.Text
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub InitializeComponent()

    End Sub

    Private Sub ResetToDefault(sender As Object, e As EventArgs)
        txtEditor.Text = DefaultTemplate
    End Sub
End Class
