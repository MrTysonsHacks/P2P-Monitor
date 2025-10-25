Imports MaterialSkin
Imports MaterialSkin.Controls

Public Class DTMOptions
    Inherits MaterialForm

    Public Property Chats As Boolean
    Public Property Tasks As Boolean
    Public Property Quests As Boolean
    Public Property Errors As Boolean
    Public Property Selfies As Boolean

    Private swChats As MaterialSwitch
    Private swTasks As MaterialSwitch
    Private swQuests As MaterialSwitch
    Private swErrors As MaterialSwitch
    Private swSelfies As MaterialSwitch
    Private btnOK As MaterialButton
    Private btnCancel As MaterialButton

    Public Sub New(initialChats As Boolean,
                   initialTasks As Boolean,
                   initialQuests As Boolean,
                   initialErrors As Boolean,
                   initialSelfies As Boolean)
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
        Text = "Monitor Options"
        StartPosition = FormStartPosition.CenterParent
        Padding = New Padding(3, 64, 3, 3)
        MinimumSize = New Size(420, 280)
        MaximizeBox = False
        MinimizeBox = False
        Sizable = False

        Dim mgr = MaterialSkinManager.Instance
        mgr.EnforceBackcolorOnAllComponents = True
        Try : mgr.AddFormToManage(Me) : Catch : End Try

        swChats = New MaterialSwitch() With {.Text = "Chats", .Location = New Point(24, 80), .AutoSize = True}
        swTasks = New MaterialSwitch() With {.Text = "Tasks", .Location = New Point(24, 120), .AutoSize = True}
        swQuests = New MaterialSwitch() With {.Text = "Quests", .Location = New Point(24, 160), .AutoSize = True}
        swErrors = New MaterialSwitch() With {.Text = "Errors", .Location = New Point(200, 80), .AutoSize = True}
        swSelfies = New MaterialSwitch() With {.Text = "Selfies", .Location = New Point(200, 120), .AutoSize = True}

        btnOK = New MaterialButton() With {.Text = "Save", .AutoSize = True}
        btnCancel = New MaterialButton() With {.Text = "Cancel", .AutoSize = True}
        Controls.AddRange({swChats, swTasks, swQuests, swErrors, swSelfies, btnOK, btnCancel})
        LayoutButtons()

        AddHandler Me.Resize, Sub() LayoutButtons()

        swChats.Checked = initialChats
        swTasks.Checked = initialTasks
        swQuests.Checked = initialQuests
        swErrors.Checked = initialErrors
        swSelfies.Checked = initialSelfies

        AddHandler btnOK.Click, AddressOf OnOK
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        AcceptButton = btnOK
        CancelButton = btnCancel
        BackColor = Me.BackColor
    End Sub

    Private Sub LayoutButtons()
        Dim pad = 24
        Dim y = ClientSize.Height - 56
        btnCancel.Location = New Point(ClientSize.Width - btnCancel.Width - pad, y)
        btnOK.Location = New Point(btnCancel.Left - btnOK.Width - 12, y)
    End Sub

    Private Sub OnOK(sender As Object, e As EventArgs)
        Chats = swChats.Checked
        Tasks = swTasks.Checked
        Quests = swQuests.Checked
        Errors = swErrors.Checked
        Selfies = swSelfies.Checked
        Me.DialogResult = DialogResult.OK
        Close()
    End Sub
End Class
