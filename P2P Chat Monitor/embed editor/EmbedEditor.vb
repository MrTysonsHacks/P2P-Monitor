Imports MaterialSkin
Imports MaterialSkin.Controls
Imports System.Text

Public Class EmbedEditor
    Inherits MaterialForm

    Public Property ResultText As String
    Public Property DefaultTemplate As String

    Private txtEditor As MaterialMultiLineTextBox
    Private btnSave As MaterialButton
    Private btnReset As MaterialButton
    Private btnCancel As MaterialButton

    Public Sub New(initialText As String, title As String, defaultTemplate As String, darkMode As Boolean)
        ' Theme hookup
        Dim skinManager = MaterialSkinManager.Instance
        skinManager.AddFormToManage(Me)

        If darkMode Then
            skinManager.Theme = MaterialSkinManager.Themes.DARK
        Else
            skinManager.Theme = MaterialSkinManager.Themes.LIGHT
        End If

        ' Center this window over the owner
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Sizable = True
        Me.Text = If(String.IsNullOrWhiteSpace(title), "Embed Editor", title)
        Me.Width = 780
        Me.Height = 560

        defaultTemplate = defaultTemplate
        ResultText = initialText

        ' --- Controls ---
        txtEditor = New MaterialMultiLineTextBox() With {
            .Name = "txtEditor",
            .Text = initialText,
            .Dock = DockStyle.Fill
        }

        btnSave = New MaterialButton() With {
            .Text = "SAVE",
            .HighEmphasis = True,
            .AutoSize = False,
            .Width = 110
        }
        AddHandler btnSave.Click, AddressOf SaveAndClose

        btnReset = New MaterialButton() With {
            .Text = "RESET TO DEFAULT",
            .HighEmphasis = True,
            .AutoSize = False,
            .Width = 170
        }
        AddHandler btnReset.Click, AddressOf ResetToDefault

        btnCancel = New MaterialButton() With {
            .Text = "CANCEL",
            .HighEmphasis = True,
            .AutoSize = False,
            .Width = 110
        }
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        Dim pnl As New FlowLayoutPanel() With {
            .Dock = DockStyle.Bottom,
            .Height = 48,
            .FlowDirection = FlowDirection.RightToLeft,
            .Padding = New Padding(8, 8, 8, 8)
        }
        pnl.Controls.AddRange(New Control() {btnSave, btnCancel, btnReset})

        Me.Controls.Add(txtEditor)
        Me.Controls.Add(pnl)

        ' Enter/Esc behavior
        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel
    End Sub

    Private Sub SaveAndClose(sender As Object, e As EventArgs)
        ResultText = txtEditor.Text
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub ResetToDefault(sender As Object, e As EventArgs)
        txtEditor.Text = DefaultTemplate
    End Sub
End Class
