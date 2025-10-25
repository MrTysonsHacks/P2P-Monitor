Imports System.IO
Imports System.Text
Imports MaterialSkin
Imports MaterialSkin.Controls

Public Class CleanLogForm
    Inherits MaterialForm

    Private ReadOnly _appendLog As Action(Of String)
    Private ReadOnly _folders As List(Of String)

    Private ReadOnly cmbFolders As MaterialComboBox
    Private ReadOnly lblLatest As MaterialLabel
    Private ReadOnly btnDoClean As MaterialButton
    Private ReadOnly btnCancel As MaterialButton

    Public Property OutputPath As String

    Public Sub New(logDirSemicolonString As String, appendLog As Action(Of String))
        Me.Text = "Clean Log"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Sizable = True
        Me.Width = 680
        Me.Height = 340

        Dim mgr = MaterialSkinManager.Instance
        mgr.AddFormToManage(Me)

        _appendLog = appendLog
        _folders = ParseFolders(logDirSemicolonString)

        Dim lblPick As New MaterialLabel()
        lblPick.Text = "Choose a monitored folder:"
        lblPick.AutoSize = True
        lblPick.Left = 24
        lblPick.Top = 88
        Me.Controls.Add(lblPick)

        cmbFolders = New MaterialComboBox() With {
        .DropDownStyle = ComboBoxStyle.DropDownList,
        .MaxDropDownItems = 10,
        .Hint = "Monitored folders"
    }
        cmbFolders.Left = 24
        cmbFolders.Top = lblPick.Bottom + 10
        cmbFolders.Width = Me.ClientSize.Width - 48
        cmbFolders.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        cmbFolders.Items.AddRange(_folders.Cast(Of Object).ToArray())
        Me.Controls.Add(cmbFolders)

        lblLatest = New MaterialLabel()
        lblLatest.Left = 24
        lblLatest.Top = cmbFolders.Bottom + 30
        lblLatest.AutoSize = True
        lblLatest.MaximumSize = New Size(Me.ClientSize.Width - 48, 0)
        lblLatest.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblLatest.Text = "Latest logfile: —"
        Me.Controls.Add(lblLatest)

        btnDoClean = New MaterialButton() With {
        .Text = "CLEAN",
        .HighEmphasis = True,
        .Width = 120
    }
        btnCancel = New MaterialButton() With {
        .Text = "CANCEL",
        .HighEmphasis = True,
        .Width = 120
    }

        btnDoClean.Left = Me.ClientSize.Width - btnDoClean.Width - 24
        btnDoClean.Top = Me.ClientSize.Height - btnDoClean.Height - 24
        btnDoClean.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right

        btnCancel.Left = btnDoClean.Left - btnCancel.Width - 8
        btnCancel.Top = btnDoClean.Top
        btnCancel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right

        Me.Controls.Add(btnDoClean)
        Me.Controls.Add(btnCancel)

        AddHandler cmbFolders.SelectedIndexChanged, AddressOf OnFolderChanged
        AddHandler btnDoClean.Click, AddressOf OnCleanClicked
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        AddHandler Me.Resize, Sub()
                                  Dim innerW = Me.ClientSize.Width - 48
                                  cmbFolders.Width = innerW
                                  lblLatest.MaximumSize = New Size(innerW, 0)
                                  btnDoClean.Left = Me.ClientSize.Width - btnDoClean.Width - 24
                                  btnCancel.Left = btnDoClean.Left - btnCancel.Width - 8
                                  btnDoClean.Top = Me.ClientSize.Height - btnDoClean.Height - 24
                                  btnCancel.Top = btnDoClean.Top
                              End Sub

        If cmbFolders.Items.Count > 0 Then
            cmbFolders.SelectedIndex = 0
        End If

        Me.AcceptButton = btnDoClean
        Me.CancelButton = btnCancel
    End Sub

    Private Shared Function ParseFolders(raw As String) As List(Of String)
        Dim list = New List(Of String)()
        If String.IsNullOrWhiteSpace(raw) Then Return list

        For Each s In raw.Split(";"c)
            Dim p = s.Trim()
            If p.Length > 0 AndAlso Directory.Exists(p) Then
                If Not list.Contains(p, StringComparer.OrdinalIgnoreCase) Then
                    list.Add(p)
                End If
            End If
        Next
        Return list
    End Function

    Private Sub OnFolderChanged(sender As Object, e As EventArgs)
        Try
            Dim folder = TryCast(cmbFolders.SelectedItem, String)
            If String.IsNullOrWhiteSpace(folder) Then
                lblLatest.Text = "Latest logfile: —"
                Return
            End If
            Dim latest = RedactionHelper.GetLatestLogInFolder(folder)
            If String.IsNullOrWhiteSpace(latest) Then
                lblLatest.Text = "Latest logfile: (none found)"
            Else
                lblLatest.Text = "Latest logfile: " & Path.GetFileName(latest)
            End If
        Catch
            lblLatest.Text = "Latest logfile: —"
        End Try
    End Sub

    Private Sub InitializeComponent()

    End Sub

    Private Sub OnCleanClicked(sender As Object, e As EventArgs)
        Try
            Dim folder = TryCast(cmbFolders.SelectedItem, String)
            If String.IsNullOrWhiteSpace(folder) Then
                MessageBox.Show(Me, "Please select a folder.", "Clean Log", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim latest = RedactionHelper.GetLatestLogInFolder(folder)
            If String.IsNullOrWhiteSpace(latest) OrElse Not File.Exists(latest) Then
                MessageBox.Show(Me, "No logfile found in the selected folder.", "Clean Log", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim tempCopy As String = Nothing
            Try
                tempCopy = RedactionHelper.SnapshotToTemp(latest)

                Dim originalText = File.ReadAllText(tempCopy, Encoding.UTF8)
                Dim cleaned = RedactionHelper.ApplyRedactions(originalText)

                Dim outPath = RedactionHelper.GetCleanedOutputPathFor(latest)
                File.WriteAllText(outPath, cleaned, Encoding.UTF8)

                Me.OutputPath = outPath
                Me.DialogResult = DialogResult.OK

            Finally
                If Not String.IsNullOrEmpty(tempCopy) Then
                    Try : File.Delete(tempCopy) : Catch : End Try
                End If
            End Try

        Catch ex As Exception
            _appendLog?.Invoke($"Clean log failed: {ex.Message}")
            MessageBox.Show(Me, "Clean failed: " & ex.Message, "Clean Log", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
