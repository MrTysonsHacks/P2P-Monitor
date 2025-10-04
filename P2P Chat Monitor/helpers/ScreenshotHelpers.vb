Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Text

Public Class ScreenshotHelpers

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function PrintWindow(hWnd As IntPtr, hdcBlt As IntPtr, nFlags As UInteger) As Boolean
    End Function

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmGetWindowAttribute(hWnd As IntPtr, dwAttribute As Integer, ByRef pvAttribute As RECT, cbAttribute As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowRect(hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function IsIconic(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ShowWindow(hWnd As IntPtr, nCmdShow As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function EnumWindows(lpEnumFunc As EnumWindowsProc, lParam As IntPtr) As Boolean
    End Function

    Public Delegate Function EnumWindowsProc(hWnd As IntPtr, lParam As IntPtr) As Boolean

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function GetWindowTextLength(hWnd As IntPtr) As Integer
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function GetWindowText(hWnd As IntPtr, lpString As StringBuilder, nMaxCount As Integer) As Integer
    End Function
    <DllImport("user32.dll")>
    Private Shared Function GetClientRect(hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ClientToScreen(hWnd As IntPtr, ByRef lpPoint As POINT) As Boolean
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Private Structure POINT
        Public X As Integer
        Public Y As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    Private Const PW_RENDERFULLCONTENT As UInteger = &H2UI
    Private Const DWMWA_EXTENDED_FRAME_BOUNDS As Integer = 9

    Private Const SW_RESTORE As Integer = 9
    Private Const SW_MINIMIZE As Integer = 6

    Public Shared Function PickBotWindow(folderName As String) As IntPtr
        If String.IsNullOrWhiteSpace(folderName) Then Return IntPtr.Zero
        Dim result As IntPtr = IntPtr.Zero
        Dim needle As String = folderName.Trim()
        EnumWindows(Function(h, p)
                        Dim len = GetWindowTextLength(h)
                        If len > 0 Then
                            Dim sb As New StringBuilder(len + 1)
                            GetWindowText(h, sb, sb.Capacity)
                            Dim title = sb.ToString()
                            If title.IndexOf("DreamBot", StringComparison.OrdinalIgnoreCase) >= 0 AndAlso
                               title.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0 Then
                                result = h
                                Return False
                            End If
                        End If
                        Return True
                    End Function, IntPtr.Zero)
        Return result
    End Function

    Public Shared Function FindByTitleChunk(titleContains As String) As IntPtr
        Dim result As IntPtr = IntPtr.Zero
        EnumWindows(Function(h, p)
                        Dim len = GetWindowTextLength(h)
                        If len > 0 Then
                            Dim sb As New StringBuilder(len + 1)
                            GetWindowText(h, sb, sb.Capacity)
                            If sb.ToString().IndexOf(titleContains, StringComparison.OrdinalIgnoreCase) >= 0 Then
                                result = h
                                Return False
                            End If
                        End If
                        Return True
                    End Function, IntPtr.Zero)
        Return result
    End Function
    Private Shared Function TryPrintWindowBitmap(hWnd As IntPtr, Optional log As Action(Of String) = Nothing) As Bitmap
        Dim r As RECT
        Dim hr As Integer = DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, r, Runtime.InteropServices.Marshal.SizeOf(GetType(RECT)))
        If hr <> 0 Then
            If Not GetWindowRect(hWnd, r) Then
                If log IsNot Nothing Then log("⚠ DwmGetWindowAttribute/GetWindowRect failed.")
                Return Nothing
            End If
        End If

        Dim w As Integer = Math.Max(0, r.Right - r.Left)
        Dim h As Integer = Math.Max(0, r.Bottom - r.Top)
        If w = 0 OrElse h = 0 Then
            If log IsNot Nothing Then log($"⚠ Invalid bounds for PrintWindow: {w}x{h}")
            Return Nothing
        End If

        Dim bmp As New Bitmap(w, h, Imaging.PixelFormat.Format32bppArgb)
        Dim g As Graphics = Nothing
        Dim hdc As IntPtr = IntPtr.Zero
        Try
            g = Graphics.FromImage(bmp)
            hdc = g.GetHdc()
            If Not PrintWindow(hWnd, hdc, PW_RENDERFULLCONTENT) Then
                bmp.Dispose()
                bmp = Nothing
            End If
        Catch ex As Exception
            If log IsNot Nothing Then log("⚠ PrintWindow exception: " & ex.Message)
            If bmp IsNot Nothing Then bmp.Dispose()
            bmp = Nothing
        Finally
            If hdc <> IntPtr.Zero Then
                Try : g.ReleaseHdc(hdc) : Catch : End Try
            End If
            If g IsNot Nothing Then g.Dispose()
        End Try

        Return bmp
    End Function

    Private Shared Sub BlurRegion(ByRef bmp As Bitmap, region As Rectangle, Optional blurSize As Integer = 12)
        If bmp Is Nothing Then Exit Sub
        If region.Width <= 0 OrElse region.Height <= 0 Then Exit Sub

        region.Intersect(New Rectangle(0, 0, bmp.Width, bmp.Height))
        If region.Width = 0 OrElse region.Height = 0 Then Exit Sub

        Dim cropped As New Bitmap(region.Width, region.Height)
        Using g As Graphics = Graphics.FromImage(cropped)
            g.DrawImage(bmp, New Rectangle(0, 0, region.Width, region.Height), region, GraphicsUnit.Pixel)
        End Using

        Dim smallW As Integer = Math.Max(1, region.Width \ blurSize)
        Dim smallH As Integer = Math.Max(1, region.Height \ blurSize)

        Dim small As New Bitmap(smallW, smallH)
        Using g As Graphics = Graphics.FromImage(small)
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBilinear
            g.DrawImage(cropped, New Rectangle(0, 0, smallW, smallH))
        End Using

        Dim blurred As New Bitmap(region.Width, region.Height)
        Using g As Graphics = Graphics.FromImage(blurred)
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBilinear
            g.DrawImage(small, New Rectangle(0, 0, region.Width, region.Height))
        End Using

        Using g As Graphics = Graphics.FromImage(bmp)
            g.DrawImage(blurred, region.Location)
        End Using
    End Sub
    Public Shared Function SnapAndSend(path As String, folderName As String, folderDir As String, log As Action(Of String)) As String
        Dim hWnd As IntPtr = PickBotWindow(folderName)
        If hWnd = IntPtr.Zero Then
            log?.Invoke($"⚠ Window for '{folderName}' not found.")
            Return Nothing
        End If

        Dim bmp As Bitmap = TryPrintWindowBitmap(hWnd, log)
        If bmp Is Nothing Then
            log?.Invoke($"⚠ PrintWindow failed for {folderName}.")
            Return Nothing
        End If

        Dim targetW As Integer = Math.Min(765, bmp.Width)
        Dim targetH As Integer = Math.Min(503, bmp.Height)
        Dim offsetY As Integer = 10

        Dim startX As Integer = Math.Max(0, (bmp.Width - targetW) \ 2)
        Dim startY As Integer = Math.Max(0, ((bmp.Height - targetH) \ 2) + offsetY)

        Dim cropRect As New Rectangle(startX, startY, targetW, targetH)
        Dim cropped As New Bitmap(cropRect.Width, cropRect.Height, Imaging.PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(cropped)
            g.DrawImage(bmp, New Rectangle(0, 0, cropRect.Width, cropRect.Height), cropRect, GraphicsUnit.Pixel)
        End Using
        bmp.Dispose()
        bmp = cropped

        Try
            If My.Settings.BlurStats AndAlso bmp IsNot Nothing Then
                Dim sensitiveArea As New Rectangle(130, 335, 390, 152)
                BlurRegion(bmp, sensitiveArea, 15)
                Using g As Graphics = Graphics.FromImage(bmp)
                    Using font As New Font("Roboto", 25, FontStyle.Bold, GraphicsUnit.Pixel)
                        Using brush As New SolidBrush(Color.White), shadowBrush As New SolidBrush(Color.Black)
                            Dim text As String = "P2P Monitor By CaS5"
                            Dim textX As Integer = 200
                            Dim textY As Integer = 400
                            g.DrawString(text, font, shadowBrush, textX + 1, textY + 1)
                            g.DrawString(text, font, brush, textX, textY)
                        End Using
                    End Using
                End Using
            End If
        Catch ex As Exception
            log?.Invoke($"⚠ Blur error: {ex.Message}")
        End Try

        If Not IO.Directory.Exists(folderDir) Then IO.Directory.CreateDirectory(folderDir)
        Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        Dim filePath As String = IO.Path.Combine(folderDir, $"dreambot_screenshot_{timestamp}.png")
        bmp.Save(filePath, Imaging.ImageFormat.Png)
        bmp.Dispose()

        log?.Invoke($"📸 Saved screenshot for {folderName} to {filePath}")
        Return filePath
    End Function


    Public Shared Async Function CaptureBotSelfie(folderName As String, outDir As String, log As Action(Of String)) As Task(Of String)
        Dim hWnd As IntPtr = PickBotWindow(folderName)
        If hWnd = IntPtr.Zero Then
            log?.Invoke($"⚠ Window for '{folderName}' not found.")
            Return Nothing
        End If

        Dim bmp As Bitmap = TryPrintWindowBitmap(hWnd, log)
        If bmp Is Nothing Then
            log?.Invoke($"⚠ PrintWindow failed for {folderName}.")
            Return Nothing
        End If

        Dim targetW As Integer = Math.Min(765, bmp.Width)
        Dim targetH As Integer = Math.Min(503, bmp.Height)
        Dim offsetY As Integer = 10
        Dim startX As Integer = Math.Max(0, (bmp.Width - targetW) \ 2)
        Dim startY As Integer = Math.Max(0, ((bmp.Height - targetH) \ 2) + offsetY)

        Dim cropRect As New Rectangle(startX, startY, targetW, targetH)
        Dim cropped As New Bitmap(cropRect.Width, cropRect.Height, Imaging.PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(cropped)
            g.DrawImage(bmp, New Rectangle(0, 0, cropRect.Width, cropRect.Height), cropRect, GraphicsUnit.Pixel)
        End Using
        bmp.Dispose()
        bmp = cropped

        Try
            If My.Settings.BlurStats AndAlso bmp IsNot Nothing Then
                Dim sensitiveArea As New Rectangle(130, 335, 390, 152)
                BlurRegion(bmp, sensitiveArea, 15)
                Using g As Graphics = Graphics.FromImage(bmp)
                    Dim font As New Font("Roboto", 25, FontStyle.Bold, GraphicsUnit.Pixel)
                    Dim brush As New SolidBrush(Color.White)
                    Dim shadowBrush As New SolidBrush(Color.Black)

                    Dim text As String = "P2P Monitor By CaS5"
                    Dim x As Integer = 200
                    Dim y As Integer = 400
                    g.DrawString(text, font, shadowBrush, x + 2, y + 2)
                    g.DrawString(text, font, brush, x, y)
                End Using
            End If
        Catch ex As Exception
            log?.Invoke($"⚠ Selfie blur error: {ex.Message}")
        End Try

        If Not IO.Directory.Exists(outDir) Then IO.Directory.CreateDirectory(outDir)
        Dim stamp As String = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        Dim filePath As String = IO.Path.Combine(outDir, $"dreambot_selfie_{stamp}.png")
        bmp.Save(filePath, Imaging.ImageFormat.Png)
        bmp.Dispose()

        log?.Invoke($"📸 Saved selfie for {folderName} to {filePath}")
        Return filePath
    End Function
End Class
