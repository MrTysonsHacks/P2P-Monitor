Imports MaterialSkin
Imports MaterialSkin.Controls

Public Class Launcher
    Inherits MaterialForm

    Private _autoLaunched As Boolean = False

    Public Sub New()
        InitializeComponent()
        Dim mgr = MaterialSkinManager.Instance
        mgr.AddFormToManage(Me)
        mgr.Theme = MaterialSkinManager.Themes.DARK
        mgr.ColorScheme = New ColorScheme(Primary.Grey800, Primary.Grey900, Primary.Grey500, Accent.LightBlue200, TextShade.WHITE)

        Me.Sizable = False
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        saveChoice.Checked = My.Settings.RememberLastTool
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)

        If _autoLaunched Then Return
        _autoLaunched = True

        If My.Settings.RememberLastTool Then
            Dim last = (If(My.Settings.LastToolChoice, String.Empty)).Trim().ToLowerInvariant()
            If last = "monitor" Then
                btnMonitor_Click(Me, EventArgs.Empty)
                Return
            ElseIf last = "scanner" Then
                btnScanner_Click(Me, EventArgs.Empty)
                Return
            End If
        End If
    End Sub

    Private Sub saveChoice_CheckedChanged(sender As Object, e As EventArgs) Handles saveChoice.CheckedChanged
        My.Settings.RememberLastTool = saveChoice.Checked
        If Not saveChoice.Checked Then
            My.Settings.LastToolChoice = String.Empty
        End If
        My.Settings.Save()
    End Sub

    Private Sub btnMonitor_Click(sender As Object, e As EventArgs) Handles btnMonitor.Click
        If saveChoice.Checked Then
            My.Settings.RememberLastTool = True
            My.Settings.LastToolChoice = "monitor"
            My.Settings.Save()
        End If

        Dim f As New main()
        AddHandler f.FormClosed, Sub()
                                     If Application.OpenForms.Count <= 1 AndAlso Not Me.IsDisposed Then
                                         Me.Show()
                                     End If
                                 End Sub
        f.Show()
        Me.Hide()
    End Sub

    Private Sub btnScanner_Click(sender As Object, e As EventArgs) Handles btnScanner.Click
        If saveChoice.Checked Then
            My.Settings.RememberLastTool = True
            My.Settings.LastToolChoice = "scanner"
            My.Settings.Save()
        End If

        Dim f As New LogScanner()
        AddHandler f.FormClosed, Sub()
                                     If Application.OpenForms.Count <= 1 AndAlso Not Me.IsDisposed Then
                                         Me.Show()
                                     End If
                                 End Sub
        f.Show()
        Me.Hide()
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        If Application.OpenForms.Count <= 1 Then
            Application.Exit()
        End If
        MyBase.OnFormClosing(e)
    End Sub
End Class
