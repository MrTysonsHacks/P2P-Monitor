Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Text

Public Class ScreenshotHelpers

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

    Private Const SW_RESTORE As Integer = 9

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

    Public Shared Function SnapAndSend(sourceLogPath As String, folderName As String, logRoot As String) As String
        Try
            Dim hWnd = PickBotWindow(folderName)
            If hWnd = IntPtr.Zero Then
                Return ""
            End If

            ShowWindow(hWnd, SW_RESTORE)
            SetForegroundWindow(hWnd)

            Dim rect As RECT
            If Not GetWindowRect(hWnd, rect) Then
                Return ""
            End If

            Dim width As Integer = rect.Right - rect.Left
            Dim height As Integer = rect.Bottom - rect.Top

            Using bmp As New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, New Size(width, height), CopyPixelOperation.SourceCopy)
                End Using

                Dim folderDir As String = IO.Path.Combine(logRoot, folderName)
                If Not IO.Directory.Exists(folderDir) Then
                    IO.Directory.CreateDirectory(folderDir)
                End If

                Dim timestamp As String = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                Dim filePath As String = IO.Path.Combine(folderDir, $"dreambot_screenshot_{timestamp}.png")

                bmp.Save(filePath, Imaging.ImageFormat.Png)

                Return filePath
            End Using

        Catch ex As Exception
            Return ""
        End Try
    End Function

End Class
