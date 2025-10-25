Imports System.Drawing
Imports MaterialSkin
Imports MaterialSkin.Controls

Public Class ThreadIds
    Inherits MaterialForm
    Public Property ChatsId As String
    Public Property TasksId As String
    Public Property QuestsId As String
    Public Property ErrorsId As String
    Public Property SelfiesId As String

    Private pnl As FlowLayoutPanel
    Private txtChats As MaterialTextBox
    Private txtTasks As MaterialTextBox
    Private txtQuests As MaterialTextBox
    Private txtErrors As MaterialTextBox
    Private txtSelfies As MaterialTextBox
    Private btnOK As MaterialButton
    Private btnCancel As MaterialButton

    Public Sub New(showChats As Boolean,
                   showTasks As Boolean,
                   showQuests As Boolean,
                   showErrors As Boolean,
                   showSelfies As Boolean,
                   Optional initialChats As String = "",
                   Optional initialTasks As String = "",
                   Optional initialQuests As String = "",
                   Optional initialErrors As String = "",
                   Optional initialSelfies As String = "")
        Me.ShowInTaskbar = True
        Try
            Dim launch = Application.OpenForms.OfType(Of Launcher)().FirstOrDefault()
            If launch IsNot Nothing AndAlso launch.Icon IsNot Nothing Then
                Me.Icon = launch.Icon
            Else
                Dim res As New System.ComponentModel.ComponentResourceManager(GetType(Launcher))
                Dim ico = TryCast(res.GetObject("$this.Icon"), Icon)
                If ico IsNot Nothing Then Me.Icon = ico
            End If
        Catch
        End Try
        Text = "Thread ID's"
        StartPosition = FormStartPosition.CenterParent
        Padding = New Padding(3, 64, 3, 3)
        MinimumSize = New Size(520, 420)
        MaximizeBox = False
        MinimizeBox = False
        Sizable = False

        Dim mgr = MaterialSkinManager.Instance
        mgr.EnforceBackcolorOnAllComponents = True
        Try : mgr.AddFormToManage(Me) : Catch : End Try

        pnl = New FlowLayoutPanel() With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(18, 12, 18, 72),
            .AutoScroll = True,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }
        Controls.Add(pnl)

        If showChats Then
            txtChats = MakeTextBox("Chats Thread ID", initialChats)
            pnl.Controls.Add(txtChats)
        End If
        If showTasks Then
            txtTasks = MakeTextBox("Tasks Thread ID", initialTasks)
            pnl.Controls.Add(txtTasks)
        End If
        If showQuests Then
            txtQuests = MakeTextBox("Quests Thread ID", initialQuests)
            pnl.Controls.Add(txtQuests)
        End If
        If showErrors Then
            txtErrors = MakeTextBox("Errors Thread ID", initialErrors)
            pnl.Controls.Add(txtErrors)
        End If
        If showSelfies Then
            txtSelfies = MakeTextBox("Selfies Thread ID", initialSelfies)
            pnl.Controls.Add(txtSelfies)
        End If

        btnOK = New MaterialButton() With {.Text = "Save", .AutoSize = True}
        btnCancel = New MaterialButton() With {.Text = "Cancel", .AutoSize = True}
        Controls.Add(btnOK)
        Controls.Add(btnCancel)

        AddHandler btnOK.Click, AddressOf OnOK
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel
        AddHandler Me.Resize, Sub() LayoutButtons()

        btnOK.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnCancel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnOK.BringToFront() : btnCancel.BringToFront()

        AcceptButton = btnOK
        CancelButton = btnCancel

        LayoutButtons()
    End Sub

    Private Function MakeTextBox(hint As String, initial As String) As MaterialTextBox
        Return New MaterialTextBox() With {
            .Hint = hint,
            .Size = New Size(440, 50),
            .Multiline = False,
            .Text = If(initial, "")
        }
    End Function

    Private Sub LayoutButtons()
        Dim pad = 24
        Dim y = ClientSize.Height - btnOK.Height - 16
        btnCancel.Location = New Point(ClientSize.Width - btnCancel.Width - pad, y)
        btnOK.Location = New Point(btnCancel.Left - btnOK.Width - 12, y)
    End Sub

    Private Sub OnOK(sender As Object, e As EventArgs)
        If txtChats IsNot Nothing Then ChatsId = txtChats.Text
        If txtTasks IsNot Nothing Then TasksId = txtTasks.Text
        If txtQuests IsNot Nothing Then QuestsId = txtQuests.Text
        If txtErrors IsNot Nothing Then ErrorsId = txtErrors.Text
        If txtSelfies IsNot Nothing Then SelfiesId = txtSelfies.Text

        Me.DialogResult = DialogResult.OK
        Close()
    End Sub
End Class
