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

    Public Shared Function SnapAndSend(sourceLogPath As String,
                                       folderName As String,
                                       logRoot As String,
                                       Optional log As Action(Of String) = Nothing) As String
        Try
            Dim baseRoot As String = logRoot
            If String.IsNullOrWhiteSpace(baseRoot) OrElse baseRoot.Contains(";"c) Then
                baseRoot = IO.Path.GetDirectoryName(sourceLogPath)
            End If

            Dim hWnd = PickBotWindow(folderName)
            If hWnd = IntPtr.Zero Then
                If log IsNot Nothing Then log($"⚠ No DreamBot window found for account '{folderName}'.")
                Return ""
            End If

            Dim bmp As Bitmap = TryPrintWindowBitmap(hWnd, log)
            Dim usedFallback As Boolean = False
            Dim wasMinimized As Boolean = False
            If bmp Is Nothing Then
                usedFallback = True
                wasMinimized = IsIconic(hWnd)
                ShowWindow(hWnd, SW_RESTORE)
                SetForegroundWindow(hWnd)
                System.Threading.Thread.Sleep(200)

                Dim rect As RECT
                If Not GetWindowRect(hWnd, rect) Then
                    If log IsNot Nothing Then log($"⚠ GetWindowRect failed for '{folderName}'.")
                    Return ""
                End If

                Dim width As Integer = rect.Right - rect.Left
                Dim height As Integer = rect.Bottom - rect.Top
                If width <= 0 OrElse height <= 0 Then
                    If log IsNot Nothing Then log($"⚠ Invalid window size for '{folderName}': {width}x{height}")
                    Return ""
                End If

                bmp = New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, New Size(width, height), CopyPixelOperation.SourceCopy)
                End Using
            End If

            Dim folderDir = IO.Path.Combine(baseRoot, folderName)
            If Not IO.Directory.Exists(folderDir) Then
                IO.Directory.CreateDirectory(folderDir)
            End If

            Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
            Dim filePath As String = IO.Path.Combine(folderDir, $"dreambot_screenshot_{timestamp}.png")
            bmp.Save(filePath, Imaging.ImageFormat.Png)
            bmp.Dispose()

            If usedFallback Then
                System.Threading.Thread.Sleep(150)
                If wasMinimized Then
                    ShowWindow(hWnd, SW_MINIMIZE)
                End If
            End If

            Return filePath

        Catch ex As Exception
            If log IsNot Nothing Then log($"⚠ Exception capturing screenshot for '{folderName}': {ex.Message}")
            Return ""
        End Try
    End Function
    Public Shared Function CaptureBotSelfie(folderName As String,
                                        accountFolderPath As String,
                                        Optional log As Action(Of String) = Nothing) As String
        Try
            If String.IsNullOrWhiteSpace(folderName) Then Return ""

            Dim hWnd = PickBotWindow(folderName)
            If hWnd = IntPtr.Zero Then
                If log IsNot Nothing Then log($"⚠ Selfie: no DreamBot window found for '{folderName}'.")
                Return ""
            End If

            Dim bmp As Bitmap = TryPrintWindowBitmap(hWnd, log)

            Dim usedFallback As Boolean = False
            Dim wasMinimized As Boolean = False
            If bmp Is Nothing Then
                usedFallback = True
                wasMinimized = IsIconic(hWnd)

                ShowWindow(hWnd, SW_RESTORE)
                SetForegroundWindow(hWnd)
                System.Threading.Thread.Sleep(200)

                Dim r As RECT
                If Not GetWindowRect(hWnd, r) Then
                    If log IsNot Nothing Then log($"⚠ Selfie: GetWindowRect failed for '{folderName}'.")
                    Return ""
                End If

                Dim w As Integer = r.Right - r.Left
                Dim h As Integer = r.Bottom - r.Top
                If w <= 0 OrElse h <= 0 Then
                    If log IsNot Nothing Then log($"⚠ Selfie: invalid window size for '{folderName}': {w}x{h}")
                    Return ""
                End If

                bmp = New Bitmap(w, h, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.CopyFromScreen(r.Left, r.Top, 0, 0, New Size(w, h), CopyPixelOperation.SourceCopy)
                End Using
            End If

            Dim outDir As String = accountFolderPath
            If Not IO.Directory.Exists(outDir) Then
                IO.Directory.CreateDirectory(outDir)
            End If

            Dim stamp As String = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
            Dim filePath As String = IO.Path.Combine(outDir, $"dreambot_selfie_{stamp}.png")
            bmp.Save(filePath, Imaging.ImageFormat.Png)
            bmp.Dispose()

            If usedFallback Then
                System.Threading.Thread.Sleep(150)
                If wasMinimized Then
                    ShowWindow(hWnd, SW_MINIMIZE)
                End If
            End If

            Return filePath

        Catch ex As Exception
            If log IsNot Nothing Then log($"⚠ Selfie: exception for '{folderName}': {ex.Message}")
            Return ""
        End Try
    End Function
End Class
