Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports System.Runtime.InteropServices.WindowsRuntime
Imports Windows.Graphics
Imports Windows.Graphics.Capture
Imports Windows.Graphics.DirectX
Imports Windows.Graphics.DirectX.Direct3D11
Imports Windows.Graphics.Imaging

Public Class ScreenshotHelpers
    <DllImport("combase.dll", ExactSpelling:=True)>
    Private Shared Function RoGetActivationFactory(hClassId As IntPtr,
                                               ByRef iid As Guid,
                                               ByRef factory As IntPtr) As Integer
    End Function

    <DllImport("combase.dll", ExactSpelling:=True, CharSet:=CharSet.Unicode)>
    Private Shared Function WindowsCreateString(source As String,
                                            length As Integer,
                                            ByRef hString As IntPtr) As Integer
    End Function

    <DllImport("combase.dll", ExactSpelling:=True)>
    Private Shared Function WindowsDeleteString(hString As IntPtr) As Integer
    End Function

    Private Shared Function GetActivationFactoryPtr(runtimeClass As String,
                                                ByRef iid As Guid,
                                                ByRef factoryPtr As IntPtr) As Integer
        Dim hstr As IntPtr = IntPtr.Zero
        Dim hr = WindowsCreateString(runtimeClass, runtimeClass.Length, hstr)
        If hr = 0 Then hr = RoGetActivationFactory(hstr, iid, factoryPtr)
        WindowsDeleteString(hstr)
        Return hr
    End Function

    <ComImport,
        System.Runtime.InteropServices.Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Private Interface IGraphicsCaptureItemInterop
        <PreserveSig>
        Function CreateForWindow(
        hwnd As IntPtr,
        ByRef iid As Guid,
        ByRef result As IntPtr
    ) As Integer

        <PreserveSig>
        Function CreateForMonitor(
        hMon As IntPtr,
        ByRef iid As Guid,
        ByRef result As IntPtr
    ) As Integer
    End Interface

    <DllImport("d3d11.dll", ExactSpelling:=True)>
    Private Shared Function CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice As IntPtr,
                                                             ByRef graphicsDevice As IntPtr) As Integer
    End Function
    Private Const D3D11_SDK_VERSION As UInteger = 7
    Private Const D3D11_CREATE_DEVICE_BGRA_SUPPORT As UInteger = &H20UI

    Private Enum D3D_DRIVER_TYPE
        HARDWARE = 1
        WARP = 5
    End Enum

    <DllImport("d3d11.dll", ExactSpelling:=True)>
    Private Shared Function D3D11CreateDevice(adapter As IntPtr,
                                          driverType As D3D_DRIVER_TYPE,
                                          software As IntPtr,
                                          flags As UInteger,
                                          pFeatureLevels As IntPtr,
                                          featureLevels As Integer,
                                          sdkVersion As UInteger,
                                          ByRef ppDevice As IntPtr,
                                          ByRef pFeatureLevel As Integer,
                                          ByRef ppImmediateContext As IntPtr) As Integer
    End Function
    Private Shared ReadOnly IID_IDXGIDevice As New Guid("54EC77FA-1377-44E6-8C32-88FD5F44C84C")
    Private Shared ReadOnly IID_IGraphicsCaptureItem As New Guid("79C3F95B-31F7-4EC2-A464-632EF5D30760")

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

    <DllImport("user32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Private Shared Function GetClassName(hWnd As IntPtr, lpClassName As StringBuilder, nMaxCount As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowThreadProcessId(hWnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Private Shared Function QueryFullProcessImageName(hProcess As IntPtr, dwFlags As Integer, lpExeName As StringBuilder, ByRef lpdwSize As Integer) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function OpenProcess(dwDesiredAccess As Integer, bInheritHandle As Boolean, dwProcessId As Integer) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function CloseHandle(hObject As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function IsWindowVisible(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetDpiForWindow(hWnd As IntPtr) As UInteger
    End Function

    <DllImport("user32.dll")>
    Private Shared Function MonitorFromWindow(hWnd As IntPtr, dwFlags As UInteger) As IntPtr
    End Function

    <DllImport("Shcore.dll")>
    Private Shared Function GetDpiForMonitor(hmonitor As IntPtr, dpiType As Integer, ByRef dpiX As UInteger, ByRef dpiY As UInteger) As Integer
    End Function

    Private Const MONITOR_DEFAULTTONEAREST As UInteger = 2
    Private Const MDT_EFFECTIVE_DPI As Integer = 0

    Private Shared Function GetWindowScale(hWnd As IntPtr) As Double
        Try
            Dim d As Integer = CInt(GetDpiForWindow(hWnd))
            If d > 0 Then Return d / 96.0
        Catch
        End Try

        Try
            Dim mon = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST)
            If mon <> IntPtr.Zero Then
                Dim dx As UInteger, dy As UInteger
                If GetDpiForMonitor(mon, MDT_EFFECTIVE_DPI, dx, dy) = 0 AndAlso dx > 0UI Then
                    Return dx / 96.0
                End If
            End If
        Catch
        End Try

        Return 1.0
    End Function

    Private Shared Function Scale(v As Integer, s As Double) As Integer
        Return Math.Max(1, CInt(Math.Round(v * s)))
    End Function


    Private Const PROCESS_QUERY_LIMITED_INFORMATION As Integer = &H1000

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

    Public Shared UseCompositorSafe As Boolean = False
    Private Shared ReadOnly _capSem As New Threading.SemaphoreSlim(1, 1)
    Private Shared ReadOnly _jitterRng As New Random()

    Private Shared ReadOnly BrowserExeNames As HashSet(Of String) =
    New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "chrome.exe", "msedge.exe", "firefox.exe", "opera.exe", "brave.exe", "iexplore.exe"
    }

    Private Shared Function GetWindowTextSafe(hWnd As IntPtr) As String
        Dim len = GetWindowTextLength(hWnd)
        If len <= 0 Then Return ""
        Dim sb As New StringBuilder(len + 1)
        GetWindowText(hWnd, sb, sb.Capacity)
        Return sb.ToString()
    End Function

    Private Shared Function GetWindowClass(hWnd As IntPtr) As String
        Dim sb As New StringBuilder(256)
        If GetClassName(hWnd, sb, sb.Capacity) > 0 Then Return sb.ToString()
        Return ""
    End Function

    Private Shared Function TryGetProcessExe(hWnd As IntPtr, ByRef exe As String) As Boolean
        exe = ""
        Dim pid As Integer = 0
        GetWindowThreadProcessId(hWnd, pid)
        If pid = 0 Then Return False
        Dim hProc = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, False, pid)
        If hProc <> IntPtr.Zero Then
            Try
                Dim sb As New StringBuilder(1024)
                Dim size As Integer = sb.Capacity
                If QueryFullProcessImageName(hProc, 0, sb, size) Then
                    exe = sb.ToString(0, size)
                    Return True
                End If
            Finally
                CloseHandle(hProc)
            End Try
        End If

        Try
            Dim p = Process.GetProcessById(pid)
            exe = p.MainModule.FileName
            Return Not String.IsNullOrEmpty(exe)
        Catch
            Return False
        End Try
    End Function

    Private Shared Function IsLikelyBrowser(hWnd As IntPtr) As Boolean
        Dim exe As String = ""
        If TryGetProcessExe(hWnd, exe) AndAlso exe.Length > 0 Then
            If BrowserExeNames.Contains(IO.Path.GetFileName(exe)) Then Return True
        End If

        Dim cls = GetWindowClass(hWnd)
        If cls.StartsWith("Chrome_WidgetWin", StringComparison.OrdinalIgnoreCase) _
       OrElse cls.Equals("MozillaWindowClass", StringComparison.OrdinalIgnoreCase) _
       OrElse cls.Equals("ApplicationFrameWindow", StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If
        Return False
    End Function

    Private Shared Function ScoreDreamBotCandidate(hWnd As IntPtr, title As String, nickname As String) As Integer
        Dim score = 0
        If title.IndexOf("DreamBot", StringComparison.OrdinalIgnoreCase) >= 0 Then score += 5
        If Not String.IsNullOrWhiteSpace(nickname) AndAlso title.IndexOf(nickname, StringComparison.OrdinalIgnoreCase) >= 0 Then score += 4
        If IsWindowVisible(hWnd) Then score += 2

        Dim rc As RECT
        If GetClientRect(hWnd, rc) Then
            Dim w = rc.Right - rc.Left
            Dim h = rc.Bottom - rc.Top
            If w >= 400 AndAlso h >= 300 Then score += 2
        End If

        Dim cls = GetWindowClass(hWnd)
        If cls.StartsWith("SunAwt", StringComparison.OrdinalIgnoreCase) _
       OrElse cls.IndexOf("LWJGL", StringComparison.OrdinalIgnoreCase) >= 0 Then
            score += 1
        End If
        Return score
    End Function

    Public Shared Function PickBotWindow(folderName As String) As IntPtr
        If String.IsNullOrWhiteSpace(folderName) Then Return IntPtr.Zero

        Dim best As IntPtr = IntPtr.Zero
        Dim bestScore As Integer = Integer.MinValue
        Dim needle As String = folderName.Trim()

        EnumWindows(Function(h, p)
                        Dim title = GetWindowTextSafe(h)
                        If String.IsNullOrWhiteSpace(title) Then Return True
                        'fix for hoody, must contain dreambot and account nickname token
                        If title.IndexOf("DreamBot", StringComparison.OrdinalIgnoreCase) < 0 Then Return True
                        If title.IndexOf(needle, StringComparison.OrdinalIgnoreCase) < 0 Then Return True

                        If IsLikelyBrowser(h) Then
                            Return True
                        End If

                        Dim score = ScoreDreamBotCandidate(h, title, needle)
                        If score > bestScore Then
                            bestScore = score
                            best = h
                        End If
                        Return True
                    End Function, IntPtr.Zero)

        Return best
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

    Private Shared Function GetStatsRegion(bmp As Bitmap, Optional dpiScale As Double = 1.0) As Rectangle
        Dim boxW As Integer = Scale(390, dpiScale)
        Dim boxH As Integer = Scale(152, dpiScale)
        Dim leftMargin As Integer = Scale(130, dpiScale)
        Dim bottomMargin As Integer = Scale(16, dpiScale)

        If bmp Is Nothing OrElse bmp.Width <= 0 OrElse bmp.Height <= 0 Then
            Return Rectangle.Empty
        End If

        Dim x As Integer = Math.Min(leftMargin, Math.Max(0, bmp.Width - boxW))
        Dim y As Integer = Math.Max(0, bmp.Height - bottomMargin - boxH)

        Dim w As Integer = Math.Min(boxW, bmp.Width)
        Dim h As Integer = Math.Min(boxH, bmp.Height)
        If x + w > bmp.Width Then x = Math.Max(0, bmp.Width - w)
        If y + h > bmp.Height Then y = Math.Max(0, bmp.Height - h)

        Return New Rectangle(x, y, w, h)
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

    Private Shared ReadOnly _rng As New Random()

    Private Shared Sub DrawCenteredWatermark(bmp As Bitmap, rect As Rectangle)
        If bmp Is Nothing OrElse rect.Width <= 0 OrElse rect.Height <= 0 Then Exit Sub

        Dim text As String = "P2P Monitor By CaS5"
        Dim roll As Integer
        SyncLock _rng
            roll = _rng.Next(1000)
        End SyncLock
        If roll = 0 Then text = "Choco Is A Pussy"

        Using g As Graphics = Graphics.FromImage(bmp)
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            Using font As New Font("Roboto", 25, FontStyle.Bold, GraphicsUnit.Pixel)
                Dim sf As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }
                Dim shadowRect As New Rectangle(rect.X + 1, rect.Y + 1, rect.Width, rect.Height)
                Using shadow As New SolidBrush(Color.Black)
                    g.DrawString(text, font, shadow, shadowRect, sf)
                End Using
                Using fore As New SolidBrush(Color.White)
                    g.DrawString(text, font, fore, rect, sf)
                End Using
            End Using
        End Using
    End Sub

    Private Shared Function TryReadDreamBotSettings(ByRef outW As Integer, ByRef outH As Integer, ByRef devMode As Boolean, Optional log As Action(Of String) = Nothing) As Boolean
        outW = 0 : outH = 0 : devMode = False
        Try
            Dim jsonPath As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "DreamBot", "BotData", "settings.json")
            If Not IO.File.Exists(jsonPath) Then
                main.AppendLog($"ℹ settings.json not found at {jsonPath}")
                Return False
            End If

            Dim text As String = IO.File.ReadAllText(jsonPath)
            Dim root As JObject = JObject.Parse(text)

            Dim devTok As JToken = root.SelectToken("$..developerMode")
            If devTok IsNot Nothing Then
                Select Case devTok.Type
                    Case JTokenType.Boolean
                        devMode = CBool(devTok)
                    Case JTokenType.Integer
                        devMode = (CInt(devTok) <> 0)
                    Case JTokenType.String
                        Dim s = devTok.ToString().Trim().ToLowerInvariant()
                        devMode = (s = "true")
                End Select
            End If

            Dim sizeTok As JToken = root.SelectToken("$..lastCanvasSize")
            If sizeTok IsNot Nothing AndAlso sizeTok.Type = JTokenType.String Then
                Dim parts = sizeTok.ToString().Split(":"c)
                Dim w As Integer, h As Integer
                If parts.Length = 2 AndAlso Integer.TryParse(parts(0).Trim(), w) AndAlso Integer.TryParse(parts(1).Trim(), h) Then
                    outW = Math.Max(1, w)
                    outH = Math.Max(1, h)
                End If
            End If
            Return True
        Catch ex As Exception
            main.AppendLog($"⚠ settings.json parse error: {ex.Message}")
            Return False
        End Try
    End Function
    Private Shared Async Function GetBitmapForWindowAsync(hWnd As IntPtr, log As Action(Of String)) As Task(Of Bitmap)
        Await _capSem.WaitAsync().ConfigureAwait(False)
        Try
            If UseCompositorSafe Then
                Dim bmp As Bitmap = Nothing
                Try
                    bmp = TryWgcBitmap(hWnd, log)
                Catch ex As Exception
                    main.AppendLog($"ℹ WGC capture failed, reverting to legacy: {ex.Message}")
                End Try
                If bmp IsNot Nothing Then Return bmp
                Return Await Task.Run(Function() TryPrintWindowBitmap(hWnd, log))
            Else
                Return Await Task.Run(Function() TryPrintWindowBitmap(hWnd, log))
            End If
        Finally
            _capSem.Release()
        End Try
    End Function

    Private Shared Function TryWgcBitmap(hWnd As IntPtr, log As Action(Of String)) As Bitmap
        If hWnd = IntPtr.Zero Then Return Nothing

        If IsIconic(hWnd) Then
            main.AppendLog("ℹ Skipping WGC capture (window minimized).")
            Return Nothing
        End If

        Try
            If Not GraphicsCaptureSession.IsSupported Then
                main.AppendLog("ℹ WGC not supported on this OS.")
                Return Nothing
            End If
        Catch
            main.AppendLog("ℹ WGC types not available (add Microsoft.Windows.SDK.*).")
            Return Nothing
        End Try

        Dim bounds As RECT
        If DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, bounds, Marshal.SizeOf(GetType(RECT))) <> 0 Then
            Dim r As RECT
            If Not GetWindowRect(hWnd, r) Then Return Nothing
            bounds = r
        End If
        Dim fallbackW As Integer = Math.Max(1, bounds.Right - bounds.Left)
        Dim fallbackH As Integer = Math.Max(1, bounds.Bottom - bounds.Top)
        Dim d3dDevicePtr As IntPtr = IntPtr.Zero
        Dim d3dCtxPtr As IntPtr = IntPtr.Zero
        Dim fl As Integer = 0
        Dim hr As Integer = D3D11CreateDevice(IntPtr.Zero, D3D_DRIVER_TYPE.HARDWARE, IntPtr.Zero,
                                          D3D11_CREATE_DEVICE_BGRA_SUPPORT, IntPtr.Zero, 0,
                                          D3D11_SDK_VERSION, d3dDevicePtr, fl, d3dCtxPtr)
        If hr <> 0 OrElse d3dDevicePtr = IntPtr.Zero Then
            hr = D3D11CreateDevice(IntPtr.Zero, D3D_DRIVER_TYPE.WARP, IntPtr.Zero,
                               D3D11_CREATE_DEVICE_BGRA_SUPPORT, IntPtr.Zero, 0,
                               D3D11_SDK_VERSION, d3dDevicePtr, fl, d3dCtxPtr)
            If hr <> 0 OrElse d3dDevicePtr = IntPtr.Zero Then
                main.AppendLog($"⚠ D3D11CreateDevice failed (hr=0x{hr:X8}).")
                Return Nothing
            End If
        End If

        Dim dxgiDevicePtr As IntPtr = IntPtr.Zero

        Marshal.QueryInterface(d3dDevicePtr, IID_IDXGIDevice, dxgiDevicePtr)
        If dxgiDevicePtr = IntPtr.Zero Then
            main.AppendLog("⚠ QueryInterface(IDXGIDevice) failed.")
            If d3dCtxPtr <> IntPtr.Zero Then Marshal.Release(d3dCtxPtr)
            Marshal.Release(d3dDevicePtr)
            Return Nothing
        End If

        Dim winrtDevicePtr As IntPtr = IntPtr.Zero
        hr = CreateDirect3D11DeviceFromDXGIDevice(dxgiDevicePtr, winrtDevicePtr)
        If hr <> 0 OrElse winrtDevicePtr = IntPtr.Zero Then
            main.AppendLog($"⚠ CreateDirect3D11DeviceFromDXGIDevice failed (hr=0x{hr:X8}).")
            Marshal.Release(dxgiDevicePtr)
            If d3dCtxPtr <> IntPtr.Zero Then Marshal.Release(d3dCtxPtr)
            Marshal.Release(d3dDevicePtr)
            Return Nothing
        End If

        Dim winrtDevice As Windows.Graphics.DirectX.Direct3D11.IDirect3DDevice = Nothing

        Try
            winrtDevice = Global.WinRT.MarshalInterface(Of Windows.Graphics.DirectX.Direct3D11.IDirect3DDevice).FromAbi(winrtDevicePtr)
        Catch ex As Exception
            main.AppendLog("⚠ Failed to project ABI to IDirect3DDevice: " & ex.Message)
            Marshal.Release(winrtDevicePtr)
            Marshal.Release(dxgiDevicePtr)
            If d3dCtxPtr <> IntPtr.Zero Then Marshal.Release(d3dCtxPtr)
            Marshal.Release(d3dDevicePtr)
            Return Nothing
        Finally
            Marshal.Release(winrtDevicePtr) : winrtDevicePtr = IntPtr.Zero
        End Try

        Dim factoryPtr As IntPtr = IntPtr.Zero

        hr = GetActivationFactoryPtr("Windows.Graphics.Capture.GraphicsCaptureItem",
                                 GetType(IGraphicsCaptureItemInterop).GUID,
                                 factoryPtr)
        If hr <> 0 OrElse factoryPtr = IntPtr.Zero Then
            main.AppendLog($"⚠ RoGetActivationFactory failed (hr=0x{hr:X8}).")
            CleanupD3D(d3dDevicePtr, d3dCtxPtr, dxgiDevicePtr, IntPtr.Zero)
            Return Nothing
        End If

        Dim interop As IGraphicsCaptureItemInterop =
        CType(Marshal.GetObjectForIUnknown(factoryPtr), IGraphicsCaptureItemInterop)
        Marshal.Release(factoryPtr)
        Dim itemPtr As IntPtr = IntPtr.Zero
        hr = interop.CreateForWindow(hWnd, IID_IGraphicsCaptureItem, itemPtr)
        If hr <> 0 OrElse itemPtr = IntPtr.Zero Then
            main.AppendLog($"⚠ CreateForWindow failed (hr=0x{hr:X8}).")
            CleanupD3D(d3dDevicePtr, d3dCtxPtr, dxgiDevicePtr, IntPtr.Zero)
            Return Nothing
        End If

        Dim item As Windows.Graphics.Capture.GraphicsCaptureItem = Nothing

        Try
            item = Global.WinRT.MarshalInterface(Of Windows.Graphics.Capture.GraphicsCaptureItem).FromAbi(itemPtr)
        Catch ex As Exception
            main.AppendLog("⚠ Failed to project ABI to GraphicsCaptureItem: " & ex.Message)
            CleanupD3D(d3dDevicePtr, d3dCtxPtr, dxgiDevicePtr, IntPtr.Zero)
            Marshal.Release(itemPtr)
            Return Nothing
        Finally
            Marshal.Release(itemPtr)
        End Try
        Dim size As Windows.Graphics.SizeInt32 = item.Size
        If size.Width <= 0 OrElse size.Height <= 0 Then
            size = New Windows.Graphics.SizeInt32 With {.Width = fallbackW, .Height = fallbackH}
        End If
        Dim pool As Windows.Graphics.Capture.Direct3D11CaptureFramePool = Nothing
        Try
            pool = Windows.Graphics.Capture.Direct3D11CaptureFramePool.CreateFreeThreaded(
                   winrtDevice,
                   Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                   2, size)
        Catch
            pool = Windows.Graphics.Capture.Direct3D11CaptureFramePool.Create(
                   winrtDevice,
                   Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                   2, size)
        End Try

        Dim session = pool.CreateCaptureSession(item)
        session.IsCursorCaptureEnabled = False

        Dim gotBitmap As Bitmap = Nothing
        Try
            session.StartCapture()
            Dim deadline = DateTime.UtcNow.AddMilliseconds(1200)
            Do
                Try
                    Using frame = pool.TryGetNextFrame()
                        If frame IsNot Nothing Then
                            Dim sbTask = Windows.Graphics.Imaging.SoftwareBitmap.CreateCopyFromSurfaceAsync(frame.Surface).AsTask()
                            If sbTask.Wait(500) Then
                                Using sb As Windows.Graphics.Imaging.SoftwareBitmap = sbTask.Result
                                    Using ras As New Windows.Storage.Streams.InMemoryRandomAccessStream()
                                        Dim encTask = Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(
                                                      Windows.Graphics.Imaging.BitmapEncoder.PngEncoderId, ras).AsTask()
                                        encTask.Wait()
                                        Dim enc = encTask.Result
                                        enc.SetSoftwareBitmap(sb)
                                        enc.FlushAsync().AsTask().Wait()

                                        ras.Seek(0)
                                        Dim total As ULong = ras.Size
                                        Using input = ras.GetInputStreamAt(0)
                                            Using reader As New Windows.Storage.Streams.DataReader(input)
                                                reader.LoadAsync(CUInt(total)).AsTask().Wait()
                                                Dim bytes(CInt(total) - 1) As Byte
                                                reader.ReadBytes(bytes)
                                                reader.Dispose()
                                                Using ms As New MemoryStream(bytes)
                                                    gotBitmap = New Bitmap(ms)
                                                End Using
                                            End Using
                                        End Using
                                    End Using
                                End Using
                                Exit Do
                            End If
                        End If
                    End Using
                Catch
                End Try
                Threading.Thread.Sleep(15)
            Loop While DateTime.UtcNow < deadline

        Finally
            Try : session.Dispose() : Catch : End Try
            Try : pool.Dispose() : Catch : End Try
            CleanupD3D(d3dDevicePtr, d3dCtxPtr, dxgiDevicePtr, IntPtr.Zero)
        End Try

        If gotBitmap IsNot Nothing Then
            main.AppendLog("✅ Captured via WGC.")
        Else
            main.AppendLog("ℹ WGC returned no frame in time; will fall back.")
        End If
        Return gotBitmap
    End Function

    Private Shared Sub CleanupD3D(dev As IntPtr, ctx As IntPtr, dxgi As IntPtr, wr As IntPtr)
        If wr <> IntPtr.Zero Then Marshal.Release(wr)
        If dxgi <> IntPtr.Zero Then Marshal.Release(dxgi)
        If ctx <> IntPtr.Zero Then Marshal.Release(ctx)
        If dev <> IntPtr.Zero Then Marshal.Release(dev)
    End Sub

    Public Shared Async Function SnapAndSend(path As String, folderName As String, folderDir As String, log As Action(Of String)) As Task(Of String)
        Dim hWnd As IntPtr = PickBotWindow(folderName)
        If hWnd = IntPtr.Zero Then
            main.AppendLog($"⚠ Window for '{folderName}' not found.")
            Return Nothing
        End If

        Dim bmp As Bitmap = Await GetBitmapForWindowAsync(hWnd, log)
        If bmp Is Nothing Then
            main.AppendLog($"⚠ Capture failed for {folderName}.")
            Return Nothing
        End If

        Dim desiredW As Integer = 0, desiredH As Integer = 0, devMode As Boolean = False
        If Not TryReadDreamBotSettings(desiredW, desiredH, devMode, log) Then
            desiredW = bmp.Width : desiredH = bmp.Height : devMode = False
        ElseIf desiredW <= 0 OrElse desiredH <= 0 Then
            desiredW = bmp.Width : desiredH = bmp.Height
        End If

        Dim dpiScale As Double = GetWindowScale(hWnd)

        Dim desiredWpx As Integer = If(desiredW > 0, Scale(desiredW, dpiScale), bmp.Width)
        Dim desiredHpx As Integer = If(desiredH > 0, Scale(desiredH, dpiScale), bmp.Height)

        Dim targetW As Integer = Math.Min(desiredWpx, bmp.Width)
        Dim targetH As Integer = Math.Min(desiredHpx, bmp.Height)

        Dim baseOffsetY As Integer = If(devMode, 10, -5)
        Dim offsetY As Integer = Scale(baseOffsetY, dpiScale)

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
                Dim sensitiveArea As Rectangle = GetStatsRegion(bmp, dpiScale)
                If sensitiveArea.Width >= 2 AndAlso sensitiveArea.Height >= 2 Then
                    BlurRegion(bmp, sensitiveArea, 15)
                    DrawCenteredWatermark(bmp, sensitiveArea)
                Else
                    main.AppendLog("ℹ Skipped blur (stats rect too small).")
                End If
            End If
        Catch ex As Exception
            main.AppendLog($"⚠ Blur/watermark error: {ex.Message}")
        End Try

        If Not IO.Directory.Exists(folderDir) Then IO.Directory.CreateDirectory(folderDir)
        Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        Dim filePath As String = IO.Path.Combine(folderDir, $"dreambot_screenshot_{timestamp}.png")
        bmp.Save(filePath, Imaging.ImageFormat.Png)
        bmp.Dispose()

        main.AppendLog($"📸 Saved screenshot for {folderName} to {filePath}")
        Return filePath
    End Function

    Public Shared Async Function CaptureBotSelfie(folderName As String, outDir As String, log As Action(Of String)) As Task(Of String)
        Dim hWnd As IntPtr = PickBotWindow(folderName)
        If hWnd = IntPtr.Zero Then
            main.AppendLog($"⚠ Window for '{folderName}' not found.")
            Return Nothing
        End If

        Dim bmp As Bitmap = Await GetBitmapForWindowAsync(hWnd, log)
        If bmp Is Nothing Then
            main.AppendLog($"⚠ Capture failed for {folderName}.")
            Return Nothing
        End If

        Dim desiredW As Integer = 0, desiredH As Integer = 0, devMode As Boolean = False
        If Not TryReadDreamBotSettings(desiredW, desiredH, devMode, log) Then
            desiredW = bmp.Width : desiredH = bmp.Height : devMode = False
        ElseIf desiredW <= 0 OrElse desiredH <= 0 Then
            desiredW = bmp.Width : desiredH = bmp.Height
        End If

        Dim dpiScale As Double = GetWindowScale(hWnd)

        Dim desiredWpx As Integer = If(desiredW > 0, Scale(desiredW, dpiScale), bmp.Width)
        Dim desiredHpx As Integer = If(desiredH > 0, Scale(desiredH, dpiScale), bmp.Height)

        Dim targetW As Integer = Math.Min(desiredWpx, bmp.Width)
        Dim targetH As Integer = Math.Min(desiredHpx, bmp.Height)

        Dim baseOffsetY As Integer = If(devMode, 10, -5)
        Dim offsetY As Integer = Scale(baseOffsetY, dpiScale)

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
                Dim sensitiveArea As Rectangle = GetStatsRegion(bmp, dpiScale)
                If sensitiveArea.Width >= 2 AndAlso sensitiveArea.Height >= 2 Then
                    BlurRegion(bmp, sensitiveArea, 15)
                    DrawCenteredWatermark(bmp, sensitiveArea)
                Else
                    main.AppendLog("ℹ Skipped blur (stats rect too small).")
                End If
            End If
        Catch ex As Exception
            main.AppendLog($"⚠ Selfie blur/watermark error: {ex.Message}")
        End Try

        If Not IO.Directory.Exists(outDir) Then IO.Directory.CreateDirectory(outDir)
        Dim stamp As String = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        Dim filePath As String = IO.Path.Combine(outDir, $"dreambot_selfie_{stamp}.png")
        bmp.Save(filePath, Imaging.ImageFormat.Png)
        bmp.Dispose()

        main.AppendLog($"📸 Saved selfie for {folderName} to {filePath}")
        Return filePath
    End Function
End Class
