Imports MaterialSkin
Imports MaterialSkin.Controls

Public Class AddAccountForm
    Inherits MaterialForm

    Public Property EnteredName As String
    Private txtName As MaterialTextBox
    Private btnSave As MaterialButton

    Public Sub New()
        Me.Text = "Add Account"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Size = New Size(400, 250)

        Dim mgr As MaterialSkinManager = MaterialSkinManager.Instance
        mgr.AddFormToManage(Me)
        mgr.Theme = If(My.Settings.DarkModeOn, MaterialSkinManager.Themes.DARK, MaterialSkinManager.Themes.LIGHT)
        mgr.ColorScheme = New ColorScheme(
        Primary.Grey800, Primary.Grey900, Primary.Grey500,
        Accent.LightBlue200, TextShade.WHITE
    )

        txtName = New MaterialTextBox() With {
        .Hint = "Enter username",
        .Location = New Point(20, 100),
        .Width = 340
    }

        btnSave = New MaterialButton() With {
        .Text = "Save",
        .Location = New Point(20, 170),
        .Width = 100
    }

        AddHandler btnSave.Click, AddressOf Save_Click

        Me.Controls.Add(txtName)
        Me.Controls.Add(btnSave)
    End Sub

    Private Sub Save_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtName.Text) Then
            MessageBox.Show("Please enter a username.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        EnteredName = txtName.Text.Trim()
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class

Public Module CLICreator
    Public Function PromptForAccountName(owner As Form) As String
        Using form As New AddAccountForm()
            If form.ShowDialog(owner) = DialogResult.OK Then
                Return form.EnteredName
            End If
        End Using
        Return Nothing
    End Function

    Public Function CreateAccountContextMenu(accountNames As MaterialComboBox) As ContextMenuStrip
        Dim cms As New ContextMenuStrip()
        cms.ShowImageMargin = False

        Dim mnuRemove As New ToolStripMenuItem("Remove selected")
        AddHandler mnuRemove.Click, Sub()
                                        Dim sel = TryCast(accountNames.SelectedItem, String)
                                        If Not String.IsNullOrWhiteSpace(sel) Then
                                            accountNames.Items.Remove(sel)
                                        End If
                                    End Sub
        cms.Items.Add(mnuRemove)

        Dim mnuSort As New ToolStripMenuItem("Sort A→Z")
        AddHandler mnuSort.Click, Sub()
                                      Dim sorted = accountNames.Items.Cast(Of String)().OrderBy(Function(x) x).ToArray()
                                      accountNames.Items.Clear()
                                      accountNames.Items.AddRange(sorted)
                                  End Sub
        cms.Items.Add(mnuSort)

        cms.Items.Add(New ToolStripSeparator())

        Dim mnuClearAll As New ToolStripMenuItem("Clear all")
        AddHandler mnuClearAll.Click, Sub()
                                          accountNames.Items.Clear()
                                      End Sub
        cms.Items.Add(mnuClearAll)

        Return cms
    End Function
    Public Sub GenerateBatchFile(dbPath As String,
                                ramValue As Integer,
                                covert As Boolean,
                                fresh As Boolean,
                                accounts As IEnumerable(Of String),
                                cliOutput As MaterialMultiLineTextBox,
                                launchP2P As Boolean)

        If String.IsNullOrWhiteSpace(dbPath) OrElse Not IO.File.Exists(dbPath) Then
            MessageBox.Show("Please select a valid .jar file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim lines As New List(Of String)
        lines.Add("@echo off")
        lines.Add("echo Starting applications...")
        lines.Add("")

        Dim jarQuoted As String = """" & dbPath & """"
        Dim ramArg As String = "-Xmx" & ramValue & "m"

        For Each acc As String In accounts
            If String.IsNullOrWhiteSpace(acc) Then Continue For

            Dim args As New List(Of String)
            args.Add("start """ & acc & """ java " & ramArg & " -jar " & jarQuoted)

            If covert Then args.Add("-covert")
            If fresh Then args.Add("-no-fresh")

            args.Add("-account """ & acc & """")
            If launchP2P Then
                args.Add("-script ""P2P Master AI""")
            End If

            lines.Add(String.Join(" ", args))
        Next

        Dim batchContent As String = String.Join(Environment.NewLine, lines)
        cliOutput.Text = batchContent
        Dim outFile As String = IO.Path.Combine(IO.Path.GetDirectoryName(dbPath), "LaunchBots.bat")
        IO.File.WriteAllText(outFile, batchContent)

        MessageBox.Show("Batch file created: " & outFile, "CLI Creator", MessageBoxButtons.OK, MessageBoxIcon.Information)
        main.AppendLog($"Batch file created: {outFile}")
    End Sub
End Module
