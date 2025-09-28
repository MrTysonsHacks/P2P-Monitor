Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Collections.Concurrent

Public NotInheritable Class SingleFolderLogHelper

    Private Shared ReadOnly NickCache As New ConcurrentDictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    Private Shared ReadOnly NickPatterns As Regex() = {
        New Regex("(?i)\bAccount\s*Nickname\s*[:\-]\s*(.+)$"),
        New Regex("(?i)\bAccount\s*[:\-]\s*(.+)$"),
        New Regex("(?i)\bNickname\s*[:\-]\s*(.+)$")
    }

    Public Shared Function TryReadAccountNickname(path As String, Optional headerLineBudget As Integer = 120) As String
        If String.IsNullOrWhiteSpace(path) OrElse Not File.Exists(path) Then Return Nothing

        Dim cached As String = Nothing
        If NickCache.TryGetValue(path, cached) AndAlso Not String.IsNullOrWhiteSpace(cached) Then
            Return cached
        End If

        Try
            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using sr As New StreamReader(fs)
                    Dim linesChecked As Integer = 0
                    While Not sr.EndOfStream AndAlso linesChecked < headerLineBudget
                        Dim line = sr.ReadLine()
                        linesChecked += 1
                        If String.IsNullOrWhiteSpace(line) Then Continue While

                        For Each rx In NickPatterns
                            Dim m = rx.Match(line)
                            If m.Success Then
                                Dim rawName = m.Groups(1).Value.Trim()
                                rawName = Regex.Replace(rawName, "^\[[A-Z]+\]\s*", "")
                                If Not String.IsNullOrWhiteSpace(rawName) Then
                                    NickCache(path) = rawName
                                    Return rawName
                                End If
                            End If
                        Next
                    End While
                End Using
            End Using
        Catch
        End Try

        Return Nothing
    End Function

    Public Shared Function GetAccountNicknameCached(path As String) As String
        Return TryReadAccountNickname(path)
    End Function

    Public Shared Function GetRecentLogs(folder As String, within As TimeSpan) As List(Of String)
        Dim result As New List(Of String)
        If String.IsNullOrWhiteSpace(folder) OrElse Not Directory.Exists(folder) Then Return result

        Dim cutoffUtc = DateTime.UtcNow - within
        Try
            For Each f In Directory.GetFiles(folder, "logfile-*.log", SearchOption.TopDirectoryOnly)
                Try
                    Dim wt = File.GetLastWriteTimeUtc(f)
                    If wt >= cutoffUtc Then result.Add(f)
                Catch
                End Try
            Next
        Catch
        End Try
        Return result
    End Function

    Public Shared Sub WarmUpForRecent(folder As String,
                                      within As TimeSpan,
                                      log As Action(Of String),
                                      lastOffsets As Dictionary(Of String, Long),
                                      lastProcessedTimes As Dictionary(Of String, DateTime?))
        Dim recent = GetRecentLogs(folder, within)
        For Each p In recent
            TryReadAccountNickname(p)
            Try
                lastOffsets(p) = New FileInfo(p).Length
                lastProcessedTimes(p) = DateTime.Now
                log?.Invoke("📄 Recent log in single-folder mode: " & Path.GetFileName(p))
            Catch
            End Try
        Next
    End Sub

End Class
