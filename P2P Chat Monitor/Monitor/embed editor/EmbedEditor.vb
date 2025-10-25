Imports MaterialSkin
Imports MaterialSkin.Controls
Imports System.Text
Public Class EmbedSelector
    Inherits MaterialForm

    Public Sub New()
        Dim skinManager = MaterialSkinManager.Instance
        skinManager.AddFormToManage(Me)
        skinManager.Theme = MaterialSkinManager.Themes.DARK

        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Select Embed to Edit"
        Me.Width = 225
        Me.Height = 325

        Dim pnl As New FlowLayoutPanel() With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .Padding = New Padding(20),
            .AutoScroll = True
        }

        Dim btnTask As New MaterialButton() With {.Text = "Edit Task Embed", .AutoSize = True}
        AddHandler btnTask.Click, Sub() OpenEditor("TaskEmbed", My.Settings.TaskEmbedSet)

        Dim btnChat As New MaterialButton() With {.Text = "Edit Chat Embed", .AutoSize = True}
        AddHandler btnChat.Click, Sub() OpenEditor("ChatEmbed", My.Settings.ChatEmbedSet)

        Dim btnQuest As New MaterialButton() With {.Text = "Edit Quest Embed", .AutoSize = True}
        AddHandler btnQuest.Click, Sub() OpenEditor("QuestEmbed", My.Settings.QuestEmbedSet)

        Dim btnFailure As New MaterialButton() With {.Text = "Edit Error Embed", .AutoSize = True}
        AddHandler btnFailure.Click, Sub() OpenEditor("ErrorEmbed", My.Settings.ErrorEmbedSet)

        pnl.Controls.AddRange(New Control() {btnTask, btnChat, btnQuest, btnFailure})
        Me.Controls.Add(pnl)
    End Sub

    Private Sub OpenEditor(title As String, currentText As String)
        Dim editor As New EmbedEditor(currentText, $"Edit {title}", currentText, True)
        If editor.ShowDialog(Me) = DialogResult.OK Then
            Select Case title
                Case "TaskEmbed" : My.Settings.TaskEmbedSet = editor.ResultText
                Case "ChatEmbed" : My.Settings.ChatEmbedSet = editor.ResultText
                Case "QuestEmbed" : My.Settings.QuestEmbedSet = editor.ResultText
                Case "FailureEmbed" : My.Settings.ErrorEmbedSet = editor.ResultText
            End Select
            My.Settings.Save()
        End If
    End Sub

    Private Sub InitializeComponent()

    End Sub
End Class

Public Class EmbedEditor
    Inherits MaterialForm

    Public Property ResultText As String
    Public Property DefaultTemplate As String

    Private txtEditor As MaterialMultiLineTextBox
    Private btnSave As MaterialButton
    Private btnReset As MaterialButton
    Private btnCancel As MaterialButton

    Public Sub New(initialText As String, title As String, defaultTemplate As String, darkMode As Boolean)
        Dim skinManager = MaterialSkinManager.Instance
        skinManager.AddFormToManage(Me)

        If darkMode Then
            skinManager.Theme = MaterialSkinManager.Themes.DARK
        Else
            skinManager.Theme = MaterialSkinManager.Themes.LIGHT
        End If

        Me.StartPosition = FormStartPosition.CenterParent
        Me.Sizable = True
        Me.Text = If(String.IsNullOrWhiteSpace(title), "Embed Editor", title)
        Me.Width = 780
        Me.Height = 560
        Me.DefaultTemplate = If(String.IsNullOrWhiteSpace(defaultTemplate), initialText, defaultTemplate)
        Me.ResultText = If(String.IsNullOrWhiteSpace(initialText), Me.DefaultTemplate, initialText)

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
