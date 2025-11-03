Imports System.Windows.Forms

Public Module ErrorHandler
    Public Sub Report(ex As Exception, Optional context As String = Nothing)
        Try
            Dim msg As String =
                If(String.IsNullOrWhiteSpace(context), ex.ToString(), $"{context}:{Environment.NewLine}{ex}")
            Try
                Dim m = Application.OpenForms.Cast(Of Form)().FirstOrDefault(Function(f) f.Name = "main")
                If m IsNot Nothing Then
                    Dim mi = m.GetType().GetMethod("AppendLog", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public)
                    If mi IsNot Nothing Then mi.Invoke(m, New Object() {"❌ " & msg})
                End If
            Catch
            End Try

            MessageBox.Show(msg, "Unhandled Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch
        End Try
    End Sub
End Module
