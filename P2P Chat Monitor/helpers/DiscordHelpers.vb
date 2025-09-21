Imports System.IO
Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json.Linq

Public Class DiscordHelpers

    Private Shared ReadOnly http As New Net.Http.HttpClient()

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
            Using client As New HttpClient()
                Dim content = New StringContent(payload, Encoding.UTF8, "application/json")
                Await client.PostAsync(url, content)
            End Using
        Catch ex As Exception
            If log IsNot Nothing Then log($"❌ Discord PostJson error: {ex.Message}")
        End Try
    End Function

    Public Shared Async Function UploadFile(url As String,
                                     filePath As String,
                                     Optional payloadJson As String = Nothing,
                                     Optional log As Action(Of String) = Nothing) As Task
        Try
            Using client As New HttpClient()
                Using form As New MultipartFormDataContent()

                    If Not String.IsNullOrWhiteSpace(payloadJson) Then
                        Dim jsonContent = New StringContent(payloadJson, Encoding.UTF8, "application/json")
                        form.Add(jsonContent, "payload_json")
                    End If

                    If Not String.IsNullOrEmpty(filePath) AndAlso File.Exists(filePath) Then
                        Dim fileName As String = Path.GetFileName(filePath)
                        Dim fileStream As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        Dim fileContent As New StreamContent(fileStream)
                        fileContent.Headers.ContentType = New Net.Http.Headers.MediaTypeHeaderValue("image/png")

                        form.Add(fileContent, "files[0]", fileName)
                    End If

                    Await client.PostAsync(url, form)
                End Using
            End Using
        Catch ex As Exception
            If log IsNot Nothing Then log($"❌ Discord Upload error: {ex.Message}")
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
        ""text"": ""P2P Monitor Detection system - {time}""
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

    Public Shared Function BuildErrorPayload(template As String, mention As String, failureType As String,
                                      trigger As String, reason As String, filename As String,
                                      folder As String, timestamp As String) As String
        Return template _
            .Replace("{mention}", JsonSafe(mention)) _
            .Replace("{type}", JsonSafe(failureType)) _
            .Replace("{trigger}", JsonSafe(trigger)) _
            .Replace("{reason}", JsonSafe(reason)) _
            .Replace("{filename}", JsonSafe(filename)) _
            .Replace("{folder}", JsonSafe(folder)) _
            .Replace("{time}", timestamp)
    End Function

    Public Shared Function BuildChatPayload(template As String,
                                        mention As String,
                                        chat As String,
                                        response As String,
                                        screenshotRef As String,
                                        filename As String,
                                        folder As String,
                                        timestamp As String,
                                        index As Integer) As String
        Dim payload = template _
        .Replace("{mention}", mention) _
        .Replace("{chat}", chat) _
        .Replace("{response}", response) _
        .Replace("{screenshot}", screenshotRef) _
        .Replace("{filename}", filename) _
        .Replace("{folder}", folder) _
        .Replace("{time}", timestamp) _
        .Replace("{index}", index.ToString())

        If String.IsNullOrWhiteSpace(screenshotRef) Then
            payload = payload.Replace("""image"": {""url"": ""attachment://""},", "")
            payload = payload.Replace("""image"": {""url"": ""attachment://""}", "")
        End If

        Return payload
    End Function


    Public Shared Function BuildQuestPayload(template As String,
                                             mention As String,
                                             segment As String,
                                             screenshotRef As String,
                                             filename As String,
                                             folder As String,
                                             timestamp As String,
                                             index As Integer) As String
        Dim payload = template _
            .Replace("{mention}", JsonSafe(mention)) _
            .Replace("{segment}", JsonSafe(segment)) _
            .Replace("{screenshot}", screenshotRef) _
            .Replace("{filename}", JsonSafe(filename)) _
            .Replace("{folder}", JsonSafe(folder)) _
            .Replace("{time}", timestamp) _
            .Replace("{index}", index.ToString())

        If String.IsNullOrWhiteSpace(screenshotRef) Then
            payload = payload.Replace("""image"": {""url"": ""attachment://""},", "")
            payload = payload.Replace("""image"": {""url"": ""attachment://""}", "")
        End If

        Return payload
    End Function
End Class
