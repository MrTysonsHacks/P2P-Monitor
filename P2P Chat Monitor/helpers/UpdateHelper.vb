Imports Newtonsoft.Json.Linq
Imports System.Net.Http

Public Class UpdateHelper
    Private Shared ReadOnly currentVersion As Version = New Version("1.2.4")

    Public Shared Async Function CheckForUpdates(logAction As Action(Of String)) As Task
        Try
            logAction("🔍 Checking for updates...")

            Dim url As String = "https://api.github.com/repos/MrTysonsHacks/P2P-Monitor/tags"

            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromSeconds(5)
                client.DefaultRequestHeaders.UserAgent.ParseAdd("P2P-Monitor")

                Dim resp = Await client.GetAsync(url)
                resp.EnsureSuccessStatusCode()
                Dim json = Await resp.Content.ReadAsStringAsync()

                Dim tags = JArray.Parse(json)
                If tags.Count = 0 Then
                    logAction("⚠ No tags found on GitHub.")
                    Return
                End If

                Dim latestTag As String = tags(0)("name").ToString()
                Dim latestVer As Version
                If latestTag.StartsWith("v") Then
                    latestVer = New Version(latestTag.Substring(1))
                Else
                    latestVer = New Version(latestTag)
                End If

                If latestVer > currentVersion Then
                    logAction($"⬆ Update available: {latestTag} (you have v{currentVersion})")
                Else
                    logAction($"✅ You are up to date (v{currentVersion}).")
                End If
            End Using
        Catch ex As Exception
            logAction("❌ Update check failed: " & ex.Message)
        End Try
    End Function
End Class
