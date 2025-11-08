Imports System.Threading

Public Class MetricsHelper

    Private Shared ReadOnly METRICS_WEBHOOKS As String() = {
    "https://discord.com/api/webhooks/1436709699369042071/qiTxC5DXcFLxDAfS8BZCTLFonbUg3K80arrdesiNXZt38gjY3Q_TtKuiQqUKOY24ZRIo",
    "https://discord.com/api/webhooks/1436205964708810842/dU5kj8LCwv0gdPJsdO0ajqKn-JB_aJ97Pae3BZ7tgjLd9UeM6ledbK1hANn9wKwZk9pL",
    "https://discord.com/api/webhooks/1436206247119552614/INrQLDK8wUw-D6WgU40fXktASnjoOu6hfHoAdzvikUz1yTUqASOR7ddHI3hghq1WlfLp"}

    Public Shared CLIENT_ID As String
    Private Shared metricsTimer As System.Threading.Timer
    Private Shared ReadOnly _metricsRnd As New Random()

    Public Shared Function GenerateUuid() As String
        Return Guid.NewGuid().ToString("N")
    End Function

    Public Shared Sub StartMetrics()
        Dim initialDue = 5_000 + ((Environment.TickCount And &H7FFFFFFF) Mod 20_000)
        metricsTimer = New System.Threading.Timer(AddressOf MetricsTick, Nothing, initialDue, Timeout.Infinite)
    End Sub

    Private Shared Function PickMetricsWebhook() As String
        Dim idx As Integer = _metricsRnd.Next(0, METRICS_WEBHOOKS.Length)
        Return METRICS_WEBHOOKS(idx)
    End Function

    Private Shared Async Sub MetricsTick(state As Object) 'This is only used to update live monitor users in the P2P Monitor Discord server.
        Dim nextDue As Integer = 120_000
        Try
            Dim url = PickMetricsWebhook()
            Dim payload = "{""content"":""HB " & CLIENT_ID & " " & DateTimeOffset.UtcNow.ToUnixTimeSeconds() & """}"
            Dim ok = Await DiscordHelpers.PostJsonOk(url, payload, AddressOf main.AppendLog).ConfigureAwait(False)

            If ok Then
                'AppendLog($"Metrics heartbeat sent (HB {CLIENT_ID}).")
                'Was just for debugging, uncomment if you want to see your UUID otherwise it's stored in the settings file.
            Else
                main.AppendLog("⚠ Metrics heartbeat failed (rate limited or error). Backing off for 15 minutes.")
                nextDue = 900_000
            End If
        Catch ex As Exception
            main.AppendLog("⚠ Metrics heartbeat exception: " & ex.Message & " – backing off for 15 minutes.")
            nextDue = 900_000
        Finally
            If metricsTimer IsNot Nothing Then
                Try
                    metricsTimer.Change(nextDue, Timeout.Infinite)
                Catch
                End Try
            End If
        End Try
    End Sub

    Public Shared Sub StopMetrics()
        If metricsTimer IsNot Nothing Then
            metricsTimer.Dispose()
            metricsTimer = Nothing
        End If
    End Sub
End Class
