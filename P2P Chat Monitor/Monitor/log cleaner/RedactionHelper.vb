Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Public NotInheritable Class RedactionHelper
    Private Sub New()
    End Sub

    Private Shared ReadOnly Rules As List(Of Regex) = New List(Of Regex) From {
        New Regex("(?<=\[INFO\]\s*Up to\s*)\d+(?=\s*coins available)", RegexOptions.IgnoreCase Or RegexOptions.Compiled),
        New Regex("(?<=\[INFO\]\s*Built user home of\s*).+", RegexOptions.IgnoreCase Or RegexOptions.Compiled),
        New Regex("(?<=\[INFO\]\s*Built user home:\s*).+", RegexOptions.IgnoreCase Or RegexOptions.Compiled),
        New Regex("(?<=\[INFO\]\s*Successfully set QuickStart account:\s*).+", RegexOptions.IgnoreCase Or RegexOptions.Compiled),
        New Regex("(?<=\[INFO\]\s*If the script does not start, delete the folder at\s*).+", RegexOptions.IgnoreCase Or RegexOptions.Compiled),
        New Regex("(?<=\[INFO\]\s*Found properties file\s*).+", RegexOptions.IgnoreCase Or RegexOptions.Compiled),
        New Regex("(?<=\[INFO\]\s*LOADING PROFILE\s*).+", RegexOptions.IgnoreCase Or RegexOptions.Compiled),
        New Regex("(?<=\[INFO\]\s*Loaded properties file\s*).+", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    }

    Public Shared Function ApplyRedactions(input As String) As String
        If String.IsNullOrEmpty(input) Then Return input
        Dim output As String = input
        For Each rx In Rules
            Try
                output = rx.Replace(output, "*REDACTED BY P2P MONITOR*")
            Catch
                ' Skip whatever rule fails instead of failing the entire thing
            End Try
        Next
        Return output
    End Function

    Public Shared Function GetLatestLogInFolder(folder As String) As String
        If String.IsNullOrWhiteSpace(folder) OrElse Not Directory.Exists(folder) Then Return Nothing
        Dim latestPath As String = Nothing
        Dim latestStamp As DateTime = DateTime.MinValue

        Try
            For Each filePath In Directory.GetFiles(folder, "logfile-*.log", SearchOption.TopDirectoryOnly)
                Try
                    Dim stamp As DateTime = File.GetLastWriteTimeUtc(filePath)
                    If (stamp > latestStamp) OrElse
                       (stamp = latestStamp AndAlso (latestPath Is Nothing OrElse String.CompareOrdinal(filePath, latestPath) > 0)) Then
                        latestStamp = stamp
                        latestPath = filePath
                    End If
                Catch
                End Try
            Next
        Catch
        End Try

        Return latestPath
    End Function

    Public Shared Function GetCleanedOutputPathFor(originalPath As String) As String
        Dim folder = Path.GetDirectoryName(originalPath)
        Dim baseName = Path.GetFileNameWithoutExtension(originalPath)
        Dim candidate = Path.Combine(folder, baseName & "-Cleaned.log")
        If File.Exists(candidate) Then
            Dim stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss")
            candidate = Path.Combine(folder, $"{baseName}-Cleaned-{stamp}.log")
        End If
        Return candidate
    End Function

    Public Shared Function SnapshotToTemp(originalPath As String,
                                          Optional retries As Integer = 5,
                                          Optional delayMs As Integer = 200) As String
        If String.IsNullOrWhiteSpace(originalPath) OrElse Not File.Exists(originalPath) Then
            Throw New FileNotFoundException("Log file not found.", originalPath)
        End If

        Dim tempPath = Path.Combine(Path.GetTempPath(), $"p2pmonitor_{Guid.NewGuid():N}.log")

        For attempt = 1 To Math.Max(1, retries)
            Try
                Using src As New FileStream(originalPath, FileMode.Open, FileAccess.Read,
                                            FileShare.ReadWrite Or FileShare.Delete)
                    Using dst As New FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                        src.CopyTo(dst)
                    End Using
                End Using
                Return tempPath
            Catch ex As IOException
                If attempt = retries Then Throw
                Threading.Thread.Sleep(delayMs)
            End Try
        Next

        Throw New IOException("Failed to snapshot the logfile to a temporary file.")
    End Function
End Class
