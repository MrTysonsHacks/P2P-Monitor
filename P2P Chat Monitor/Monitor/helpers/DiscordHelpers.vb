Imports System.IO
Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions
Imports System.Linq
Imports System.Collections.Concurrent

Public Class DiscordHelpers

    Private Shared ReadOnly http As New Net.Http.HttpClient()
    Private Shared ReadOnly ImageAttachmentRegex As New Regex("(?s),?\s*""image""\s*:\s*\{\s*""url""\s*:\s*""attachment://[^""]*""\s*\}\s*,?", RegexOptions.Compiled)
    Private Shared Function StripAttachmentImageBlock(json As String) As String
        Return ImageAttachmentRegex.Replace(json, "")
    End Function
    Private Shared Function RemoveImageWhenNoScreenshot(payload As String, screenshotRef As String) As String
        If Not String.IsNullOrWhiteSpace(screenshotRef) Then Return payload
        Try
            Dim root = JObject.Parse(payload)
            Dim embeds = TryCast(root("embeds"), JArray)
            If embeds IsNot Nothing Then
                For Each emb As JObject In embeds.OfType(Of JObject)()
                    emb.Remove("image")
                Next
            End If
            Return root.ToString(Newtonsoft.Json.Formatting.None)
        Catch
            Return StripAttachmentImageBlock(payload)
        End Try
    End Function

    Private Shared ReadOnly _nextAllowedSend As New ConcurrentDictionary(Of String, DateTime)(StringComparer.OrdinalIgnoreCase)

    Private Shared Async Function RespectCooldownAsync(target As String) As Task
        Dim now = DateTime.UtcNow
        Dim nextAt = _nextAllowedSend.GetOrAdd(target, now)
        If nextAt > now Then
            Dim delayMs = CInt(Math.Max(0, (nextAt - now).TotalMilliseconds))
            If delayMs > 0 Then Await Task.Delay(delayMs).ConfigureAwait(False)
        End If
    End Function

    Private Shared Sub BumpCooldown(target As String, seconds As Double)
        Dim until = DateTime.UtcNow.AddSeconds(Math.Max(0.0, seconds))
        _nextAllowedSend(target) = until
    End Sub

    Private Shared Function ParseRetryAfter(resp As HttpResponseMessage, body As String) As Double
        Try
            If Not String.IsNullOrWhiteSpace(body) Then
                Dim o = JObject.Parse(body)
                Dim ra = o("retry_after")
                If ra IsNot Nothing Then
                    Return CDbl(ra)
                End If
            End If
        Catch
        End Try
        Dim retryHeader As IEnumerable(Of String) = Nothing
        If resp.Headers.TryGetValues("Retry-After", retryHeader) Then
            Dim s = retryHeader.FirstOrDefault()
            Dim v As Double
            If Double.TryParse(s, v) Then Return v
        End If
        Return 1.0
    End Function

    Public Shared Function WithThreadId(baseWebhook As String, threadId As String) As String
        If String.IsNullOrWhiteSpace(baseWebhook) OrElse String.IsNullOrWhiteSpace(threadId) Then Return baseWebhook
        Dim sep = If(baseWebhook.Contains("?"), "&", "?")
        Return baseWebhook.Trim() & sep & "thread_id=" & threadId.Trim()
    End Function

    Public Shared Function JsonSafe(s As String) As String
        If s Is Nothing Then Return ""
        Dim t = s.Replace("\\", "\\\\").Replace("""", "\""")
        t = t.Replace(vbCrLf, "\n").Replace(vbCr, "\n").Replace(vbLf, "\n").Replace(vbTab, "\t")
        Return t
    End Function

    Public Shared Function IsJson(json As String, ByRef errorMessage As String) As Boolean
        Try
            JToken.Parse(json)
            Return True
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

    Public Shared Async Function PostJson(url As String, payload As String, Optional log As Action(Of String) = Nothing) As Task
        Try
            Dim content = New StringContent(payload, Encoding.UTF8, "application/json")
            Using resp = Await http.PostAsync(url, content)
                If Not resp.IsSuccessStatusCode Then
                    Dim body As String = ""
                    Try : body = Await resp.Content.ReadAsStringAsync() : Catch : End Try
                    If log IsNot Nothing Then
                        log($"❌ Discord POST {CInt(resp.StatusCode)} {resp.ReasonPhrase}. Body: {body}")
                    End If
                End If
            End Using
        Catch ex As Exception
            If log IsNot Nothing Then log($"❌ Discord PostJson error: {ex.Message}")
        End Try
    End Function

    Public Shared Async Function PostJsonOk(url As String,
                                       payload As String,
                                       Optional log As Action(Of String) = Nothing) As Task(Of Boolean)
        Try
            Await RespectCooldownAsync(url).ConfigureAwait(False)

            Dim content = New StringContent(payload, Encoding.UTF8, "application/json")
            Using resp = Await http.PostAsync(url, content).ConfigureAwait(False)
                If resp.IsSuccessStatusCode Then
                    _nextAllowedSend.TryRemove(url, Nothing)
                    Return True
                End If

                Dim body As String = ""
                Try : body = Await resp.Content.ReadAsStringAsync().ConfigureAwait(False) : Catch : End Try

                If CInt(resp.StatusCode) = 429 Then
                    Dim waitSec = ParseRetryAfter(resp, body)
                    If log IsNot Nothing Then log($"⏳ Discord 429; retrying in {waitSec:0.000}s")
                    BumpCooldown(url, waitSec + 0.05)
                    Await Task.Delay(CInt(waitSec * 1000)).ConfigureAwait(False)

                    Using resp2 = Await http.PostAsync(url, New StringContent(payload, Encoding.UTF8, "application/json")).ConfigureAwait(False)
                        If resp2.IsSuccessStatusCode Then
                            _nextAllowedSend.TryRemove(url, Nothing)
                            Return True
                        Else
                            Dim body2 As String = ""
                            Try : body2 = Await resp2.Content.ReadAsStringAsync().ConfigureAwait(False) : Catch : End Try
                            If log IsNot Nothing Then log($"❌ Discord POST {CInt(resp2.StatusCode)} {resp2.ReasonPhrase}. Body: {body2}")
                            Return False
                        End If
                    End Using
                Else
                    If log IsNot Nothing Then log($"❌ Discord POST {CInt(resp.StatusCode)} {resp.ReasonPhrase}. Body: {body}")
                    Return False
                End If
            End Using
        Catch ex As Exception
            If log IsNot Nothing Then log($"❌ Discord PostJson error: {ex.Message}")
            Return False
        End Try
    End Function


    Public Shared Async Function UploadFile(url As String,
                                        filePath As String,
                                        Optional payloadJson As String = Nothing,
                                        Optional log As Action(Of String) = Nothing) As Task(Of Boolean)
        Try
            Await RespectCooldownAsync(url).ConfigureAwait(False)

            Using form As New MultipartFormDataContent()
                If Not String.IsNullOrWhiteSpace(payloadJson) Then
                    form.Add(New StringContent(payloadJson, Encoding.UTF8, "application/json"), "payload_json")
                End If
                If Not String.IsNullOrEmpty(filePath) AndAlso File.Exists(filePath) Then
                    Dim fileName As String = Path.GetFileName(filePath)
                    Dim fileStream As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Dim fileContent As New StreamContent(fileStream)
                    fileContent.Headers.ContentType = New Net.Http.Headers.MediaTypeHeaderValue("image/png")
                    form.Add(fileContent, "files[0]", fileName)
                End If

                Using resp = Await http.PostAsync(url, form).ConfigureAwait(False)
                    If resp.IsSuccessStatusCode Then
                        _nextAllowedSend.TryRemove(url, Nothing)
                        Return True
                    End If

                    Dim body As String = ""
                    Try : body = Await resp.Content.ReadAsStringAsync().ConfigureAwait(False) : Catch : End Try

                    If CInt(resp.StatusCode) = 429 Then
                        Dim waitSec = ParseRetryAfter(resp, body)
                        If log IsNot Nothing Then log($"⏳ Discord 429 (upload); retrying in {waitSec:0.000}s")
                        BumpCooldown(url, waitSec + 0.05)
                        Await Task.Delay(CInt(waitSec * 1000)).ConfigureAwait(False)
                        Using resp2 = Await http.PostAsync(url, form).ConfigureAwait(False)
                            If resp2.IsSuccessStatusCode Then
                                _nextAllowedSend.TryRemove(url, Nothing)
                                Return True
                            Else
                                Dim body2 As String = ""
                                Try : body2 = Await resp2.Content.ReadAsStringAsync().ConfigureAwait(False) : Catch : End Try
                                If log IsNot Nothing Then log($"❌ Discord UPLOAD {CInt(resp2.StatusCode)} {resp2.ReasonPhrase}. Body: {body2}")
                                Return False
                            End If
                        End Using
                    Else
                        If log IsNot Nothing Then log($"❌ Discord UPLOAD {CInt(resp.StatusCode)} {resp.ReasonPhrase}. Body: {body}")
                        Return False
                    End If
                End Using
            End Using
        Catch ex As Exception
            If log IsNot Nothing Then log($"❌ Discord Upload error: {ex.Message}")
            Return False
        End Try
    End Function

    Public Shared Async Function FetchText(url As String) As Task(Of String)
        Return Await http.GetStringAsync(url)
    End Function

    Public Shared ReadOnly defaultChatTemplate As String =
    "{
  ""content"": """",
  ""tts"": false,
  ""embeds"": [
    {
      ""title"": ""P2P AI Chat Event"",
      ""description"": ""<@{mention}> - Detected P2P AI Chat Event\n\nLog: **{filename}**\nAccount: **{folder}**\n\n"",
      ""fields"": [
        {
          ""name"": ""Chat:"",
          ""value"": ""{chat}"",
          ""inline"": false
        },
        {
          ""name"": ""Response:"",
          ""value"": ""{response}""
        }
      ],
      ""footer"": {
        ""text"": ""P2P Monitor Detection System - {time}""
      },
      ""image"": {
        ""url"": ""attachment://{screenshot}""
      },
      ""color"": 6029136
    }
  ]
}"


    Public Shared ReadOnly defaultQuestTemplate As String =
    "{
  ""content"": """",
  ""tts"": false,
  ""embeds"": [
    {
      ""title"": ""P2P AI Quest Completion"",
      ""description"": ""<@{mention}> - Detected P2P AI Quest Completion\n\nLog: **{filename}**\nAccount: **{folder}**\n\n"",
      ""fields"": [
        {
          ""name"": ""Quest Completed:"",
          ""value"": ""{segment}"",
          ""inline"": false
        }
      ],
      ""footer"": {
        ""text"": ""P2P Monitor Detection System - {time}""
      },
      ""image"": {
        ""url"": ""attachment://{screenshot}""
      },
      ""color"": 6029136
    }
  ]
}"


    Public Shared ReadOnly defaultErrorTemplate As String =
    "{
  ""content"": """",
  ""tts"": false,
  ""embeds"": [
    {
      ""title"": ""P2P AI Error Detected"",
      ""description"": ""<@{mention}> - Detected P2P AI {type} Error\n\nLog: **{filename}**\nAccount: **{folder}**\n\n"",
      ""fields"": [
        {
          ""name"": ""Error Trigger:"",
          ""value"": ""{trigger}"",
          ""inline"": false
        },
        {
          ""name"": ""Error Reason:"",
          ""value"": ""{reason}""
        }
      ],
      ""footer"": {
        ""text"": ""P2P Monitor Detection System - {time}""
      },
      ""color"": 16711680
    }
  ]
}"
    Public Shared ReadOnly defaultTaskTemplate As String =
    "{
  ""content"": """",
  ""tts"": false,
  ""embeds"": [
    {
      ""title"": ""P2P AI Task Update"",
      ""description"": ""<@{mention}> - Detected P2P AI Task Update\n\nLog: **{filename}**\nAccount: **{folder}**\n\n"",
      ""fields"": [
        {
          ""name"": ""Task"",
          ""value"": ""{task}"",
          ""inline"": false
        },
        {
          ""name"": ""Activity:"",
          ""value"": ""{activity}""
        }
      ],
      ""footer"": {
        ""text"": ""P2P Monitor Detection System - {time}""
      },
      ""image"": {
        ""url"": ""attachment://{screenshot}""
      },
      ""color"": 16775424
    }
  ]
}"

    Public Shared ReadOnly defaultSelfieTemplate As String =
    "{""content"": ""📸 Periodic screenshot for account: {account}""}"


    Public Shared Function BuildErrorPayload(template As String, mention As String, failureType As String,
                                      trigger As String, reason As String, filename As String,
                                      folder As String, timestamp As DateTime) As String

        Dim time12h As String = timestamp.ToString("hh:mm:ss tt")
        Dim time24h As String = timestamp.ToString("HH:mm:ss")

        Return template _
        .Replace("{mention}", JsonSafe(mention)) _
        .Replace("{type}", JsonSafe(failureType)) _
        .Replace("{trigger}", JsonSafe(trigger)) _
        .Replace("{reason}", JsonSafe(reason)) _
        .Replace("{filename}", JsonSafe(filename)) _
        .Replace("{folder}", JsonSafe(folder)) _
        .Replace("{12h}", time12h) _
        .Replace("{24h}", time24h) _
        .Replace("{time}", time24h)
    End Function

    Public Shared Function BuildChatPayload(template As String,
                                        mention As String,
                                        chat As String,
                                        response As String,
                                        screenshotRef As String,
                                        filename As String,
                                        folder As String,
                                        timestamp As DateTime,
                                        index As Integer) As String

        Dim time12h As String = timestamp.ToString("hh:mm:ss tt")
        Dim time24h As String = timestamp.ToString("HH:mm:ss")

        Dim payload = template _
        .Replace("{mention}", JsonSafe(mention)) _
        .Replace("{chat}", JsonSafe(chat)) _
        .Replace("{response}", JsonSafe(response)) _
        .Replace("{screenshot}", JsonSafe(screenshotRef)) _
        .Replace("{filename}", JsonSafe(filename)) _
        .Replace("{folder}", JsonSafe(folder)) _
        .Replace("{12h}", time12h) _
        .Replace("{24h}", time24h) _
        .Replace("{time}", time24h) _
        .Replace("{index}", index.ToString())

        payload = RemoveImageWhenNoScreenshot(payload, screenshotRef)
        Return payload
    End Function


    Public Shared Function BuildQuestPayload(template As String,
                                         mention As String,
                                         segment As String,
                                         screenshotRef As String,
                                         filename As String,
                                         folder As String,
                                         timestamp As DateTime,
                                         index As Integer) As String

        Dim time12h As String = timestamp.ToString("hh:mm:ss tt")
        Dim time24h As String = timestamp.ToString("HH:mm:ss")

        Dim payload = template _
        .Replace("{mention}", JsonSafe(mention)) _
        .Replace("{segment}", JsonSafe(segment)) _
        .Replace("{screenshot}", screenshotRef) _
        .Replace("{filename}", JsonSafe(filename)) _
        .Replace("{folder}", JsonSafe(folder)) _
        .Replace("{12h}", time12h) _
        .Replace("{24h}", time24h) _
        .Replace("{time}", time24h) _
        .Replace("{index}", index.ToString())

        payload = RemoveImageWhenNoScreenshot(payload, screenshotRef)
        Return payload
    End Function
    Public Shared Function BuildTaskPayload(template As String,
                                        mention As String,
                                        task As String,
                                        activity As String,
                                        screenshotRef As String,
                                        filename As String,
                                        folder As String,
                                        timestamp As DateTime,
                                        index As Integer) As String

        Dim time12h As String = timestamp.ToString("hh:mm:ss tt")
        Dim time24h As String = timestamp.ToString("HH:mm:ss")

        Dim payload = template _
        .Replace("{mention}", JsonSafe(mention)) _
        .Replace("{task}", JsonSafe(task)) _
        .Replace("{activity}", JsonSafe(activity)) _
        .Replace("{screenshot}", screenshotRef) _
        .Replace("{filename}", JsonSafe(filename)) _
        .Replace("{folder}", JsonSafe(folder)) _
        .Replace("{12h}", time12h) _
        .Replace("{24h}", time24h) _
        .Replace("{time}", time24h) _
        .Replace("{index}", index.ToString())

        payload = RemoveImageWhenNoScreenshot(payload, screenshotRef)
        Return payload
    End Function
    Public Shared Function BuildSelfiePayload(template As String,
                                              mention As String,
                                              fileName As String,
                                              folder As String,
                                              accountName As String,
                                              timestamp As DateTime,
                                              Optional index As Integer = 1,
                                              Optional screenshotRef As String = "",
                                              Optional chat As String = "",
                                              Optional response As String = "",
                                              Optional task As String = "",
                                              Optional activity As String = "",
                                              Optional segment As String = "",
                                              Optional failureType As String = "",
                                              Optional trigger As String = "",
                                              Optional reason As String = "") As String
        If String.IsNullOrWhiteSpace(template) Then template = defaultSelfieTemplate
        Dim time12h As String = timestamp.ToString("hh:mm:ss tt")
        Dim time24h As String = timestamp.ToString("HH:mm:ss")
        Dim accountSafe As String = (If(accountName, "")).Replace("""", "'"c)

        Dim payload As String = template _
            .Replace("{mention}", JsonSafe(mention)) _
            .Replace("{account}", JsonSafe(accountSafe)) _
            .Replace("{screenshot}", JsonSafe(screenshotRef)) _
            .Replace("{filename}", JsonSafe(fileName)) _
            .Replace("{folder}", JsonSafe(folder)) _
            .Replace("{12h}", time12h) _
            .Replace("{24h}", time24h) _
            .Replace("{time}", time24h) _
            .Replace("{index}", index.ToString()) _
        .Replace("{chat}", JsonSafe(chat)) _
        .Replace("{response}", JsonSafe(response)) _
        .Replace("{task}", JsonSafe(task)) _
        .Replace("{activity}", JsonSafe(activity)) _
        .Replace("{segment}", JsonSafe(segment)) _
        .Replace("{type}", JsonSafe(failureType)) _
        .Replace("{trigger}", JsonSafe(trigger)) _
        .Replace("{reason}", JsonSafe(reason))

        payload = RemoveImageWhenNoScreenshot(payload, screenshotRef)
        Return payload
    End Function
End Class
