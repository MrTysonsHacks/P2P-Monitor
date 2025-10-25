Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Collections.Generic

Public Class LogHelper

    Private Shared ReadOnly LogLevels As HashSet(Of String) =
        New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {"INFO", "DEBUG", "WARN", "ERROR", "TRACE", "FATAL"}

    Private Shared ReadOnly SeenLogs As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

    Public Shared Sub ResetSeen()
        SeenLogs.Clear()
    End Sub

    Public Shared Function GetLatestPerFolder(LOG_DIR As String) As List(Of String)
        Dim result As New List(Of String)()
        If String.IsNullOrWhiteSpace(LOG_DIR) Then Return result

        For Each dir As String In LOG_DIR.Split(";"c)
            Dim folder = dir.Trim()
            If folder = "" OrElse Not Directory.Exists(folder) Then Continue For

            Dim latestPath As String = Nothing
            Dim latestStamp As DateTime = DateTime.MinValue

            Try
                For Each filePath In Directory.GetFiles(folder, "logfile-*.log", SearchOption.TopDirectoryOnly)
                    Try
                        Dim stamp As DateTime = File.GetLastWriteTimeUtc(filePath)
                        If (stamp > latestStamp) OrElse (stamp = latestStamp AndAlso (latestPath Is Nothing OrElse String.CompareOrdinal(filePath, latestPath) > 0)) Then
                            latestStamp = stamp
                            latestPath = filePath
                        End If
                    Catch
                    End Try
                Next
            Catch
            End Try

            If Not String.IsNullOrWhiteSpace(latestPath) Then
                result.Add(latestPath)
            End If
        Next

        Return result
    End Function

    Public Shared Sub AnnounceLatestOnce(paths As IEnumerable(Of String),
                                         logAction As Action(Of String),
                                         lastOffsets As Dictionary(Of String, Long),
                                         lastProcessedTimes As Dictionary(Of String, DateTime?))
        For Each p In paths
            If Not SeenLogs.Contains(p) Then
                logAction("📄 New log file detected: " & Path.GetFileName(p))
                SeenLogs.Add(p)
                JumpToEnd(p, lastOffsets, lastProcessedTimes)
            End If
        Next
    End Sub

    Public Shared Async Function OnLogChanged(sender As Object, e As FileSystemEventArgs,
                                         monitoring As Boolean,
                                         lastOffsets As Dictionary(Of String, Long),
                                         lastProcessedTimes As Dictionary(Of String, DateTime?),
                                         monitorChat As Boolean,
                                         monitorQuests As Boolean,
                                         takeScreenshots As Boolean, monitorTask As Boolean,
                                         questError As Boolean,
                                         skillIssue As Boolean,
                                         combatError As Boolean,
                                         questFailureTriggers As List(Of Regex),
                                         questFailureReasons As List(Of KeyValuePair(Of Regex, String)),
                                         skillFailureTriggers As List(Of Regex),
                                         skillFailureReasons As List(Of KeyValuePair(Of Regex, String)),
                                         combatFailureTriggers As List(Of Regex),
                                         combatFailureReasons As List(Of KeyValuePair(Of Regex, String)),
                                         AppendLog As Action(Of String),
                                         SendSegments As Func(Of List(Of List(Of String)), String, String, Integer, String, Task),
                                         PostFailAlert As Func(Of String, String, String, String, Task),
                                         GetFolderName As Func(Of String, String)) As Task(Of Boolean)

        If Not monitoring Then Return False
        Try
            Dim path As String = e.FullPath
            If Not File.Exists(path) Then Return False

            If Not lastOffsets.ContainsKey(path) Then
                JumpToEnd(path, lastOffsets, lastProcessedTimes)
                Return False
            End If

            If e.ChangeType = WatcherChangeTypes.Created Then
                JumpToEnd(path, lastOffsets, lastProcessedTimes)
                Return False
            End If

            Dim offset As Long = If(lastOffsets.ContainsKey(path), lastOffsets(path), 0L)
            Dim newLines As List(Of String) = TailReadNewLines(path, offset)
            lastOffsets(path) = offset
            If newLines.Count = 0 Then Return False

            Dim cutoff As DateTime? = If(lastProcessedTimes.ContainsKey(path), lastProcessedTimes(path), Nothing)
            Dim onlyNew As New List(Of String)
            If Not SeenLogs.Contains(path) Then SeenLogs.Add(path)

            For Each line In newLines
                Dim ts = ParseLogDate(line)
                If ts.HasValue AndAlso (cutoff Is Nothing OrElse ts > cutoff) Then
                    onlyNew.Add(line)
                End If
            Next
            If onlyNew.Count = 0 Then Return False

            Dim folderName As String = GetFolderName(path)

            For Each line In onlyNew
                If line.IndexOf("BREAK START", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    main.accountBreakStates(folderName) = True
                    AppendLog($"⏸ {folderName} is now on break. Selfies paused.")
                ElseIf line.IndexOf("Break over", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    main.accountBreakStates(folderName) = False
                    AppendLog($"▶ {folderName} break ended. Selfies resumed.")
                End If
            Next

            If monitorChat Then
                Dim chatSegments = SliceChatSegments(onlyNew)
                If chatSegments.Count > 0 Then
                    Dim screenshotPath As String = Nothing
                    If takeScreenshots Then
                        Dim logRoot As String = System.IO.Path.GetDirectoryName(path)
                        Dim perAccountDir As String = System.IO.Path.Combine(logRoot, folderName)
                        screenshotPath = Await ScreenshotHelpers.SnapAndSend(path, folderName, perAccountDir, AppendLog)
                        If Not String.IsNullOrWhiteSpace(screenshotPath) Then
                            AppendLog("📸 Screenshot captured.")
                        Else
                            AppendLog("⚠ DreamBot window not found or failed to capture screenshot.")
                        End If
                    End If

                    AppendLog($"📨 Found {chatSegments.Count} chat event(s)")
                    Await SendSegments(chatSegments, path, "P2P Chat Event", &H7289DA, screenshotPath)
                End If
            End If

            If monitorQuests Then
                Dim questSegments = SliceQuests(onlyNew)
                If questSegments.Count > 0 Then
                    Dim screenshotPath As String = Nothing
                    If takeScreenshots Then
                        Dim logRoot As String = System.IO.Path.GetDirectoryName(path)
                        Dim perAccountDir As String = System.IO.Path.Combine(logRoot, folderName)
                        screenshotPath = Await ScreenshotHelpers.SnapAndSend(path, folderName, perAccountDir, AppendLog)
                        If Not String.IsNullOrWhiteSpace(screenshotPath) Then
                            AppendLog("📸 Screenshot captured.")
                        Else
                            AppendLog("⚠ DreamBot window not found or failed to capture screenshot.")
                        End If
                    End If

                    AppendLog($"🏆 Found {questSegments.Count} quest(s)")
                    Await SendSegments(questSegments, path, "Quest Event", &HFFD700, screenshotPath)
                End If
            End If

            If monitorTask Then
                Dim taskPairs = SliceTasks(onlyNew)
                If taskPairs.Count > 0 Then
                    Dim taskSegments As New List(Of List(Of String))()
                    For Each pair In taskPairs
                        taskSegments.Add(New List(Of String)(pair))
                    Next
                    Dim screenshotPath As String = ""
                    If takeScreenshots Then
                        Dim logRoot As String = System.IO.Path.GetDirectoryName(path)
                        Dim perAccountDir As String = System.IO.Path.Combine(logRoot, folderName)
                        screenshotPath = Await ScreenshotHelpers.SnapAndSend(path, folderName, perAccountDir, AppendLog)
                        If Not String.IsNullOrWhiteSpace(screenshotPath) Then
                            AppendLog("📸 Screenshot captured.")
                        Else
                            AppendLog("⚠ DreamBot window not found or failed to capture screenshot.")
                        End If
                    End If

                    AppendLog($"📝 Found {taskSegments.Count} task(s)")
                    Await SendSegments(taskSegments, path, "Task Event", &H57F287, screenshotPath)
                End If
            End If

            If questError Then
                Dim questFailures = ScanFailures(onlyNew, questFailureTriggers, questFailureReasons, "Quest")
                For Each failure In questFailures
                    Dim reasonOut = failure.Reason
                    If Not String.IsNullOrWhiteSpace(failure.Extra) Then reasonOut &= $" ({failure.Extra})"
                    AppendLog($"❌ Quest Failure detected: {failure.Trigger} / {reasonOut}")
                    Await PostFailAlert(failure.Trigger, reasonOut, path, "Quest")
                Next
            End If

            If skillIssue Then
                Dim skillFailures = ScanFailures(onlyNew, skillFailureTriggers, skillFailureReasons, "Skill")
                For Each failure In skillFailures
                    Dim reasonOut = failure.Reason
                    If Not String.IsNullOrWhiteSpace(failure.Extra) Then reasonOut &= $" ({failure.Extra})"
                    AppendLog($"❌ Skill Failure detected: {failure.Trigger} / {reasonOut}")
                    Await PostFailAlert(failure.Trigger, reasonOut, path, "Skill")
                Next
            End If

            If combatError Then
                Dim combatFailures = ScanFailures(onlyNew, combatFailureTriggers, combatFailureReasons, "Combat")
                For Each failure In combatFailures
                    Dim reasonOut = failure.Reason
                    If Not String.IsNullOrWhiteSpace(failure.Extra) Then reasonOut &= $" ({failure.Extra})"
                    AppendLog($"❌ Combat Failure detected: {failure.Trigger} / {reasonOut}")
                    Await PostFailAlert(failure.Trigger, reasonOut, path, "Combat")
                Next
            End If

            Dim maxTs = onlyNew.Select(Function(line) ParseLogDate(line)).
                            Where(Function(ts) ts.HasValue).
                            Select(Function(ts) ts.Value).
                            DefaultIfEmpty(DateTime.MinValue).
                            Max()
            If maxTs <> DateTime.MinValue Then lastProcessedTimes(path) = maxTs

        Catch ex As Exception
            AppendLog("Watcher error: " & ex.Message)
        End Try
        Return True
    End Function

    Public Shared Sub HeartbeatTick(state As Object, logAction As Action(Of String), ByRef lastFile As String,
                                    checkInterval As Integer, LOG_DIR As String,
                                    lastOffsets As Dictionary(Of String, Long),
                                    lastProcessedTimes As Dictionary(Of String, DateTime?))

        Dim latest = GetLatestLogFile(LOG_DIR)
        If String.IsNullOrWhiteSpace(latest) Then
            logAction("No log files found.")
            Return
        End If

        If Not SeenLogs.Contains(latest) Then
            logAction("📄 New log file detected: " & Path.GetFileName(latest))
            SeenLogs.Add(latest)
            lastFile = latest
            JumpToEnd(latest, lastOffsets, lastProcessedTimes)
        Else
            logAction($"No new entries. (Interval {checkInterval} seconds)")
        End If
    End Sub

    Public Shared Sub JumpToEnd(path As String, lastOffsets As Dictionary(Of String, Long), lastProcessedTimes As Dictionary(Of String, DateTime?))
        Try
            lastOffsets(path) = New FileInfo(path).Length
            lastProcessedTimes(path) = DateTime.Now
        Catch
        End Try
    End Sub

    Public Shared Function GetLatestLogFile(LOG_DIR As String) As String
        If String.IsNullOrWhiteSpace(LOG_DIR) Then Return Nothing

        Dim allLogs As New List(Of String)()
        For Each dir As String In LOG_DIR.Split(";"c)
            Dim trimmed = dir.Trim()
            If trimmed <> "" AndAlso Directory.Exists(trimmed) Then
                Try
                    allLogs.AddRange(Directory.GetFiles(trimmed, "logfile-*.log", SearchOption.TopDirectoryOnly))
                Catch
                End Try
            End If
        Next

        If allLogs.Count = 0 Then Return Nothing

        Dim latestPath As String = Nothing
        Dim latestStamp As DateTime = DateTime.MinValue

        For Each filePath As String In allLogs
            Try
                Dim stamp As DateTime = File.GetLastWriteTimeUtc(filePath)
                If (stamp > latestStamp) OrElse (stamp = latestStamp AndAlso (latestPath Is Nothing OrElse String.CompareOrdinal(filePath, latestPath) > 0)) Then
                    latestStamp = stamp
                    latestPath = filePath
                End If
            Catch
            End Try
        Next

        Return latestPath
    End Function

    Public Shared Function TailReadNewLines(path As String, ByRef offset As Long) As List(Of String)
        Dim result As New List(Of String)
        Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            fs.Seek(offset, SeekOrigin.Begin)
            Using sr As New StreamReader(fs)
                While Not sr.EndOfStream
                    result.Add(sr.ReadLine())
                End While
                offset = fs.Position
            End Using
        End Using
        Return result
    End Function

    Public Shared Function ParseLogDate(line As String) As DateTime?
        Try
            Return DateTime.ParseExact(line.Substring(0, 19), "yyyy-MM-dd HH:mm:ss", Nothing)
        Catch
            Return Nothing
        End Try
    End Function

    Public Shared Function SliceChatSegments(lines As IEnumerable(Of String)) As List(Of List(Of String))
        Dim segments As New List(Of List(Of String))()
        Dim current As New List(Of String)()
        For Each line In lines
            Dim upper = line.ToUpperInvariant()
            If upper.Contains("CHAT") AndAlso current.Count = 0 Then
                current.Add(line)
            ElseIf (upper.Contains("SLOWLY TYPING RESPONSE") OrElse upper.Contains("BAD RESPONSE")) AndAlso current.Count > 0 Then
                current.Add(line)
                segments.Add(New List(Of String)(current))
                current.Clear()
            ElseIf current.Count > 0 Then
                current.Add(line)
            End If
        Next
        Return segments
    End Function

    Public Shared Function SliceQuests(lines As IEnumerable(Of String)) As List(Of List(Of String))
        Dim quests As New List(Of List(Of String))()

        For Each line In lines
            If line.IndexOf("completed a quest", StringComparison.OrdinalIgnoreCase) >= 0 Then
                Dim noColor = Regex.Replace(line, "<col=.*?>(.*?)</col>", "$1", RegexOptions.IgnoreCase)

                Dim body = Regex.Replace(noColor,
                                     "^\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\s+\[[A-Z]+\]\s*(?:\[[^\]]*\]\s*)?>?\s*",
                                     "",
                                     RegexOptions.IgnoreCase)
                Dim questName As String = body
                Dim idx As Integer = questName.LastIndexOf(":"c)
                If idx >= 0 AndAlso idx < questName.Length - 1 Then
                    questName = questName.Substring(idx + 1)
                End If
                questName = questName.Trim()

                If questName.Length > 0 Then
                    quests.Add(New List(Of String) From {questName})
                End If
            End If
        Next

        Return quests
    End Function

    Public Shared Function ScanFailures(lines As List(Of String),
                                    triggers As List(Of Regex),
                                    reasons As List(Of KeyValuePair(Of Regex, String)),
                                    failureType As String) As List(Of (Trigger As String, Reason As String, Extra As String))

        Dim results As New List(Of (Trigger As String, Reason As String, Extra As String))

        For i = 0 To lines.Count - 1
            Dim currentLine = lines(i)
            If triggers.Any(Function(rx) rx.IsMatch(currentLine)) Then
                Dim matchedReason As String = Nothing

                Dim cleanedTrigger As String = Regex.Replace(currentLine,
                                                         "^\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\s+\[[A-Z]+\]\s*>?\s*",
                                                         "",
                                                         RegexOptions.IgnoreCase).Trim()

                Dim startIdx = Math.Max(0, i - 10)
                Dim endIdx = Math.Min(lines.Count - 1, i + 10)

                For j = startIdx To endIdx
                    For Each r In reasons
                        If r.Key.IsMatch(lines(j)) Then
                            matchedReason = r.Value
                            Exit For
                        End If
                    Next
                    If matchedReason IsNot Nothing Then Exit For
                Next

                If String.IsNullOrWhiteSpace(matchedReason) Then
                    matchedReason = "Unknown reason, please send logs to CaS5"
                End If

                Dim extra As String = ExtractExtraFromContext(lines, i)

                results.Add((cleanedTrigger, matchedReason, extra))
            End If
        Next
        Return results
    End Function

    Public Shared Function SliceTasks(lines As IEnumerable(Of String)) As List(Of List(Of String))
        Dim result As New List(Of List(Of String))()
        Dim arr As List(Of String) = lines.ToList()

        For i As Integer = 0 To arr.Count - 1
            Dim line As String = arr(i)

            If line.IndexOf("BREAK START", StringComparison.OrdinalIgnoreCase) >= 0 Then
                Dim taskName As String = "Break"
                Dim activity As String = ""

                Dim startIdx As Integer = Math.Max(0, i - 25)
                Dim endIdx As Integer = Math.Min(arr.Count - 1, i + 25)

                For j As Integer = startIdx To endIdx
                    If arr(j).IndexOf("Break length", StringComparison.OrdinalIgnoreCase) >= 0 Then
                        Dim parts = arr(j).Split(" "c)
                        Dim msStr As String = parts(parts.Length - 1)
                        Dim ms As Long
                        If Long.TryParse(msStr, ms) Then
                            Dim ts As TimeSpan = TimeSpan.FromMilliseconds(ms)

                            Dim sb As New System.Text.StringBuilder("Length: ")
                            If ts.Hours > 0 Then sb.Append($"{ts.Hours}h ")
                            If ts.Minutes > 0 OrElse ts.Hours > 0 Then sb.Append($"{ts.Minutes}m ")
                            sb.Append($"{ts.Seconds}s")

                            activity = sb.ToString().Trim()
                        End If
                        Exit For
                    End If
                Next

                result.Add(New List(Of String) From {taskName, activity})
                Continue For
            End If

            If line.IndexOf("NEW TASK", StringComparison.OrdinalIgnoreCase) >= 0 Then
                Dim maxJ As Integer = Math.Min(arr.Count - 1, i + 35)

                Dim normalTask As String = Nothing
                Dim actualTask As String = Nothing
                Dim genericActivity As String = Nothing
                Dim slayerArrow As String = Nothing

                Dim stripPrefix As Func(Of String, String) =
                Function(s As String) Regex.Replace(s, "^\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\s+\[[A-Z]+\]\s*>?\s*", "", RegexOptions.IgnoreCase)

                For j As Integer = i + 1 To maxJ
                    Dim inner As String = arr(j)
                    Dim body As String = stripPrefix(inner)
                    Dim bodyTrim As String = body.Trim()

                    If Regex.IsMatch(bodyTrim, "^Task is(?:\s+NOT)?\s+doable with this style$", RegexOptions.IgnoreCase) Then
                        Continue For
                    End If

                    If body.StartsWith("Actually task is", StringComparison.OrdinalIgnoreCase) Then
                        actualTask = inner
                    ElseIf Regex.IsMatch(body, "^(?<!Previous\s)Task is\b(?!\s+(?:NOT\s+)?doable with this style\b)", RegexOptions.IgnoreCase) Then
                        normalTask = inner
                    ElseIf body.StartsWith("Activity is", StringComparison.OrdinalIgnoreCase) Then
                        genericActivity = inner
                    ElseIf inner.IndexOf("Slayer ->", StringComparison.OrdinalIgnoreCase) >= 0 Then
                        slayerArrow = inner
                    End If
                Next

                Dim taskName As String = ""
                If Not String.IsNullOrEmpty(actualTask) Then
                    taskName = Regex.Replace(stripPrefix(actualTask), "^Actually task is\s*", "", RegexOptions.IgnoreCase).Trim()
                ElseIf Not String.IsNullOrEmpty(normalTask) Then
                    taskName = Regex.Replace(stripPrefix(normalTask), "^Task is\s*", "", RegexOptions.IgnoreCase).Trim()
                End If

                Dim activity As String = ""
                If Not String.IsNullOrEmpty(slayerArrow) Then
                    activity = Regex.Replace(slayerArrow, ".*Slayer\s*->\s*", "", RegexOptions.IgnoreCase).Trim()
                ElseIf Not String.IsNullOrEmpty(genericActivity) Then
                    activity = Regex.Replace(stripPrefix(genericActivity), "^Activity is\s*", "", RegexOptions.IgnoreCase).Trim()
                End If

                'really the best I can do without seeing more logs, except everyone keeps cropping their shit. Stop doing that. I'm talking to you.
                Dim bossingLine As Integer = -1
                Dim bossingCheckMax As Integer = Math.Min(arr.Count - 1, i + 25)
                For j As Integer = i + 1 To bossingCheckMax
                    If arr(j).IndexOf("Bossing Step 0", StringComparison.OrdinalIgnoreCase) >= 0 Then
                        taskName = "Bossing"
                        bossingLine = j
                        Exit For
                    End If
                Next

                If bossingLine <> -1 Then
                    Dim infernoStart As Integer = Math.Max(0, bossingLine - 25)
                    Dim infernoEnd As Integer = Math.Min(arr.Count - 1, bossingLine + 25)
                    For j As Integer = infernoStart To infernoEnd
                        If arr(j).IndexOf("Infernal Cape", StringComparison.OrdinalIgnoreCase) >= 0 Then
                            activity = "Inferno"
                            Exit For
                        End If
                    Next
                End If

                If String.IsNullOrWhiteSpace(taskName) AndAlso String.IsNullOrWhiteSpace(activity) Then
                    Continue For
                End If

                If Regex.IsMatch(taskName, "^(?:NOT\s+)?doable with this style$", RegexOptions.IgnoreCase) Then
                    taskName = ""
                End If

                taskName = If(taskName, "").Trim()
                activity = If(activity, "").Trim()

                result.Add(New List(Of String) From {taskName, activity})
            End If
        Next
        Return result
    End Function


    Private Shared Function ExtractExtraFromContext(lines As List(Of String), center As Integer) As String
        Dim startIdx = Math.Max(0, center - 10)
        Dim endIdx = Math.Min(lines.Count - 1, center + 10)

        For j = endIdx To startIdx Step -1
            Dim m = Regex.Match(lines(j), "Resource check failed\s*\[(.*?)\]", RegexOptions.IgnoreCase)
            If m.Success Then Return m.Groups(1).Value
        Next

        For j = endIdx To startIdx Step -1
            Dim ms = Regex.Matches(lines(j), "\[(.*?)\]")
            For k = ms.Count - 1 To 0 Step -1
                Dim content = ms(k).Groups(1).Value.Trim()
                If Not LogLevels.Contains(content) AndAlso content.Length > 1 AndAlso content.IndexOfAny(New Char() {","c, " "c}) >= 0 Then
                    Return content
                End If
            Next
        Next

        Return ""
    End Function
End Class
