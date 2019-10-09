﻿Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Windows.Automation

Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.Win32


Public Class Form1

    <DllImport("user32.dll", EntryPoint:="FindWindow", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindowByClass(ByVal lpClassName As String, ByVal zero As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll", ExactSpelling:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetParent(ByVal hWnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function SetParent(ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInt32) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal wMsg As Int32, ByVal wParam As Boolean, ByVal lParam As Int32) As Integer
    End Function

    <DllImport("kernel32.dll")>
    Private Shared Function SetProcessWorkingSetSize(ByVal hProcess As IntPtr, ByVal dwMinimumWorkingSetSize As Int32, ByVal dwMaximumWorkingSetSize As Int32) As Int32
    End Function




    Dim SWP_NOSIZE As UInt32 = 1
    Dim SWP_NOMOVE As UInt32 = 2
    Dim SWP_NOZORDER As UInt32 = 4
    Dim SWP_NOREDRAW As UInt32 = 8
    Dim SWP_NOACTIVATE As UInt32 = 16
    Dim SWP_DRAWFRAME As UInt32 = 32
    Dim SWP_FRAMECHANGED As UInt32 = 32
    Dim SWP_SHOWWINDOW As UInt32 = 64
    Dim SWP_HIDEWINDOW As UInt32 = 128
    Dim SWP_NOCOPYBITS As UInt32 = 256
    Dim SWP_NOOWNERZORDER As UInt32 = 512
    Dim SWP_NOREPOSITION As UInt32 = 512
    Dim SWP_NOSENDCHANGING As UInt32 = 1024
    Dim SWP_DEFERERASE As UInt32 = 8192
    Dim SWP_ASYNCWINDOWPOS As UInt32 = 16384

    Private Const WM_SETICON = &H80

    Private Const WM_SETREDRAW As Integer = 11
    Private Const WM_PAINT = &HF
    Private Const WM_ERASEBKGND = &H14
    Private Const WM_DESTROY = &H2
    Private Const WM_ENABLE = &HA


    Dim HWND_TOP As IntPtr = 0
    Dim HWND_BOTTOM As IntPtr = 1
    Dim HWND_TOPMOST As IntPtr = -1
    Dim HWND_NOTOPMOST As IntPtr = -2

    Public Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    Dim Shell_TrayWnd As AutomationElement = AutomationElement.FromHandle(FindWindowByClass("Shell_TrayWnd", 0))
    Dim MSTaskListWClass As AutomationElement = Shell_TrayWnd.FindFirst(TreeScope.Descendants, New PropertyCondition(AutomationElement.ClassNameProperty, "MSTaskListWClass"))
    Dim TrayNotifyWnd As AutomationElement = Shell_TrayWnd.FindFirst(TreeScope.Descendants, New PropertyCondition(AutomationElement.ClassNameProperty, "TrayNotifyWnd"))
    Dim StartButton As AutomationElement = Shell_TrayWnd.FindFirst(TreeScope.Descendants, New PropertyCondition(AutomationElement.ClassNameProperty, "Start"))
    Dim MSTaskSwWClass = GetParent(MSTaskListWClass.Current.NativeWindowHandle)
    Dim ReBarWindow32 = GetParent(MSTaskSwWClass)
    Dim Desktop = GetParent(FindWindowByClass("Shell_TrayWnd", 0))

    Dim DesktopPtr As IntPtr = Desktop
    Dim Shell_TrayWndPtr As IntPtr = Shell_TrayWnd.Current.NativeWindowHandle
    Dim MSTaskListWClassPtr As IntPtr = MSTaskListWClass.Current.NativeWindowHandle
    Dim StartButtonPtr As IntPtr = StartButton.Current.NativeWindowHandle
    Dim TrayNotifyWndPtr As IntPtr = TrayNotifyWnd.Current.NativeWindowHandle
    Dim MSTaskSwWClassPtr As IntPtr = MSTaskSwWClass
    Dim ReBarWindow32Ptr As IntPtr = ReBarWindow32

    Dim TaskbarWidthFull As Integer
    Dim TaskbarLeft As Integer
    Dim SecondaryTaskbarLeft As Integer
    Public IsTaskbarMoving As Boolean
    Public TaskbarNewPos As Integer
    Dim SecondaryTaskbarNewPos As Integer
    Dim Launch As Boolean
    Public UpdateTaskbar As Boolean
    Dim Horizontal As Boolean
    Dim StickyStartButton As Boolean
    Dim StartButtonWidth As Integer
    Dim StartButtonHeight As Integer
    Dim StickyTray As Boolean
    Dim TrayBarWidth As Integer
    Dim TrayBarHeight As Integer
    Public StartUp As Boolean

    Dim Radiobutton1Value As Boolean
    Dim Radiobutton2Value As Boolean
    Dim Radiobutton3Value As Boolean
    Dim Radiobutton4Value As Boolean
    Dim Radiobutton5Value As Boolean
    Dim Radiobutton6Value As Boolean



    Sub RestartExplorer()
        For Each MyProcess In Process.GetProcessesByName("explorer")
            MyProcess.Kill()
        Next
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        StartUp = True

        Form2.Show()


        If Application.StartupPath.Contains("40210ChrisAndriessen") Then
            CheckBox7.Visible = False
            CheckBox7.Checked = False
            Dim strx As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Microsoft\Windows\Start Menu\Programs\Startup"
            If File.Exists(strx + "\FalconX.lnk") Then
                Dim str As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Microsoft\Windows\Start Menu\Programs\Startup"
                File.Delete(str + "\FalconX.lnk")
            End If

        End If

        Launch = True

        If ComboBox1.Text = Nothing Then
            ComboBox1.Text = "QuadEaseOut"
        End If


        RunAtStartUp()


        Dim CurrentProcess As Process = Process.GetCurrentProcess
        CurrentProcess.PriorityClass = ProcessPriorityClass.Idle

        SendMessage(ReBarWindow32Ptr, WM_SETREDRAW, False, 0)


        IsTaskbarMoving = False

        Dim TitlebarHeight As Integer = Me.Height - Me.ClientSize.Height - 2
        Me.Size = New Size(Me.Width, Button2.Location.Y + TitlebarHeight + Button2.Height + 14)

        Dim t1 As System.Threading.Thread = New System.Threading.Thread(AddressOf TaskbarCalculator)
        t1.Start()


    End Sub

    Sub TaskbarCalculator()
        Do

            Try


                Dim Laps As Integer
                Dim Trigger As Integer
                Dim TaskbarWidth As Integer = 0
                Dim OldTaskbarCount As Integer
                Dim TaskbarCount As Integer = 0

                Dim OldTrayWidth As Integer
                Dim TrayWidth As Integer = 0

                Dim Resolution As Integer = 0
                Dim OldResolution As Integer
                Dim tw As TreeWalker = TreeWalker.ControlViewWalker
                Dim child As AutomationElement = tw.GetLastChild(MSTaskListWClass)

                tw = Nothing

                If MSTaskListWClass.Current.BoundingRectangle.Height >= 200 Then
                    If Horizontal = True Then
                        UpdateTaskbar = True

                    End If
                    Horizontal = False
                Else
                    If Horizontal = False Then
                        UpdateTaskbar = True

                    End If
                    Horizontal = True
                End If

                If Horizontal = False Then
                    TaskbarCount = child.Current.BoundingRectangle.Top
                    Resolution = Screen.PrimaryScreen.Bounds.Height
                    TrayWidth = Resolution - TrayNotifyWnd.Current.BoundingRectangle.Height
                Else
                    TaskbarCount = child.Current.BoundingRectangle.Left
                    Resolution = Screen.PrimaryScreen.Bounds.Width
                    TrayWidth = TrayNotifyWnd.Current.BoundingRectangle.Left
                End If

                System.Threading.Thread.Sleep(NumericUpDown3.Value)
                If Not TaskbarCount = OldTaskbarCount Or Not Resolution = OldResolution Or Not TrayWidth = OldTrayWidth Or UpdateTaskbar = True Then


                    Dim CurrentProcess As Process = Process.GetCurrentProcess
                    CurrentProcess.PriorityClass = ProcessPriorityClass.High

                    OldTaskbarCount = TaskbarCount
                    OldResolution = Resolution
                    OldTrayWidth = TrayWidth

                    For Each ui As AutomationElement In MSTaskListWClass.FindAll(TreeScope.Descendants, New PropertyCondition(AutomationElement.IsControlElementProperty, True))
                        If Not ui.Current.Name = Nothing Then
                            If Horizontal = False Then
                                TaskbarWidth = TaskbarWidth + ui.Current.BoundingRectangle.Height
                            Else
                                TaskbarWidth = TaskbarWidth + ui.Current.BoundingRectangle.Width
                            End If
                            System.Threading.Thread.Sleep(5)
                        End If
                    Next
                    Console.WriteLine(TaskbarWidth)
                    Dim rct As RECT
                    GetWindowRect(ReBarWindow32Ptr, rct)

                    If Horizontal = False Then
                        TaskbarLeft = rct.Top
                    Else
                        TaskbarLeft = rct.Left
                    End If

                    Console.WriteLine(rct.Bottom)

                    TaskbarWidthFull = TaskbarWidth
                    Dim TaskbarWidthHalf = TaskbarWidthFull / 2
                    Dim position As Integer


                    If Horizontal = True Then
                        If CheckBox1.Checked = True Then
                            Dim offset = (TrayNotifyWnd.Current.BoundingRectangle.Width / 2 - (TaskbarLeft \ 2))
                            position = Screen.PrimaryScreen.Bounds.Width / 2 - TaskbarWidthHalf - TaskbarLeft + NumericUpDown2.Value - 4 - offset
                        Else
                            position = Screen.PrimaryScreen.Bounds.Width / 2 - TaskbarWidthHalf - TaskbarLeft + NumericUpDown2.Value - 4
                        End If
                    Else
                        If CheckBox1.Checked = True Then
                            Dim offset = (TrayNotifyWnd.Current.BoundingRectangle.Height / 2 - (TaskbarLeft \ 2))
                            position = Screen.PrimaryScreen.Bounds.Height / 2 - TaskbarWidthHalf - TaskbarLeft + NumericUpDown2.Value - 4 - offset
                        Else
                            position = Screen.PrimaryScreen.Bounds.Height / 2 - TaskbarWidthHalf - TaskbarLeft + NumericUpDown2.Value - 4
                        End If
                    End If

                    TaskbarNewPos = position


                    If StickyStartButton = True Then
                        StartButtonWidth = StartButton.Current.BoundingRectangle.Width
                        StartButtonHeight = StartButton.Current.BoundingRectangle.Height
                    End If

                    If StickyTray = True Then
                        TrayBarWidth = TrayNotifyWnd.Current.BoundingRectangle.Width
                        TrayBarHeight = TrayNotifyWnd.Current.BoundingRectangle.Height
                    End If

                    Me.Invoke(Sub()
                                  Label1.Text = position
                              End Sub)

                End If



                Laps = Laps + 1

                If Laps = 50 Then
                    Laps = 0
                    Console.WriteLine("SetProcessWorkingSetSize" & Environment.NewLine)
                    SaveMemory()
                End If


            Catch ex As Exception
                Console.WriteLine("TaskbarCalculator : " & ex.Message & Environment.NewLine)

                If ex.ToString.Contains("E_ACCESSDENIED") Then


                    Dim Handle As IntPtr
                    Dim Laps2 As Integer

                    Console.WriteLine("Looking for Explorer..." & Environment.NewLine)

                    SaveMemory()

                    Do

                        Laps2 = Laps2 + 1

                        If Laps2 = 50 Then
                            Laps2 = 0
                            SaveMemory()
                        End If


                        Handle = Nothing
                        System.Threading.Thread.Sleep(250)
                        Handle = FindWindowByClass("Shell_TrayWnd", 0)

                    Loop Until Not Handle = Nothing

                    Console.WriteLine("Explorer detected! Restarting..." & Environment.NewLine)

                    NotifyIcon1.Visible = False
                    Application.Restart()
                    End
                End If

            End Try

        Loop
    End Sub

    Sub MovetoPos()
        SendMessage(ReBarWindow32Ptr, WM_SETREDRAW, True, 0)

        Dim position = Form2.Panel1.Left

        If Horizontal = False Then
            SetWindowPos(MSTaskListWClassPtr, IntPtr.Zero, 0, position, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_ASYNCWINDOWPOS Or SWP_NOSENDCHANGING Or SWP_NOACTIVATE)
        Else
            SetWindowPos(MSTaskListWClassPtr, IntPtr.Zero, position, 0, 0, 0, SWP_NOSIZE Or SWP_ASYNCWINDOWPOS Or SWP_NOACTIVATE)
        End If

        SendMessage(ReBarWindow32Ptr, WM_SETREDRAW, False, 0)
    End Sub
    Private Sub Label1_TextChanged(sender As Object, e As EventArgs) Handles Label1.TextChanged

        If Form2.Visible = True Then
            Form2.needtomove()
        End If

    End Sub


    Public Function SaveMemory() As Int32

        Return SetProcessWorkingSetSize(Diagnostics.Process.GetCurrentProcess.Handle, 2097152, 2097152)

    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Console.WriteLine("Saving Settings..." & Environment.NewLine)
        My.Settings.Save()

        SendMessage(ReBarWindow32Ptr, WM_SETREDRAW, True, 0)

        SetWindowPos(MSTaskListWClassPtr, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE Or SWP_ASYNCWINDOWPOS Or SWP_NOSENDCHANGING Or SWP_NOACTIVATE Or SWP_NOCOPYBITS Or SWP_NOOWNERZORDER)
        SetWindowPos(StartButtonPtr, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE Or SWP_ASYNCWINDOWPOS Or SWP_NOSENDCHANGING Or SWP_NOACTIVATE Or SWP_NOCOPYBITS Or SWP_NOOWNERZORDER)

        NotifyIcon1.Visible = False

        Console.WriteLine("Closing...")
        Me.Close()
        End
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("https://chrisandriessen.nl")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Console.WriteLine("Saving Settings..." & Environment.NewLine)
        My.Settings.Save()
        Me.Hide()
        Me.Opacity = 0

    End Sub


    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.Click

        Me.Opacity = 100
        Me.Show()

    End Sub

    Sub RestartApp()
        NotifyIcon1.Visible = False
        Application.Restart()
        End
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        RestartApp()
    End Sub

    Sub RunAtStartUp()
        If Application.StartupPath.Contains("40210ChrisAndriessen") Then
            Exit Sub
        End If

        Dim regKey As Microsoft.Win32.RegistryKey
        regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
        regKey.DeleteValue(Application.ProductName, False)
        regKey.Close()


        Dim strx As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Microsoft\Windows\Start Menu\Programs\Startup"

        If File.Exists(strx + "\FalconX.lnk") Then
            Dim str As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Microsoft\Windows\Start Menu\Programs\Startup"
            File.Delete(str + "\FalconX.lnk")
        End If


        If CheckBox7.Checked = True Then
            Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
            objectValue = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
            Dim objectValue2 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, "SpecialFolders", New Object() {"Startup"}, Nothing, Nothing, Nothing))
            Dim objectValue3 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, "CreateShortcut", New Object() {Operators.ConcatenateObject(objectValue2, "\FalconX.lnk")}, Nothing, Nothing, Nothing))
            NewLateBinding.LateSet(objectValue3, Nothing, "TargetPath", New Object() {NewLateBinding.LateGet(objectValue, Nothing, "ExpandEnvironmentStrings", New Object() {Application.ExecutablePath}, Nothing, Nothing, Nothing)}, Nothing, Nothing)
            NewLateBinding.LateSet(objectValue3, Nothing, "WorkingDirectory", New Object() {NewLateBinding.LateGet(objectValue, Nothing, "ExpandEnvironmentStrings", New Object() {Application.StartupPath}, Nothing, Nothing, Nothing)}, Nothing, Nothing)
            NewLateBinding.LateSet(objectValue3, Nothing, "WindowStyle", New Object() {4}, Nothing, Nothing)
            NewLateBinding.LateCall(objectValue3, Nothing, "Save", New Object(-1) {}, Nothing, Nothing, Nothing, True)
        End If

        Console.WriteLine("Saving Settings..." & Environment.NewLine)
        My.Settings.Save()

    End Sub

    Private Sub CheckBox7_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox7.CheckedChanged
        RunAtStartUp()
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown





        If Launch = True Then
            Me.Hide()
            Launch = False
        End If
    End Sub





    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        UpdateTaskbar = True
    End Sub

    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged
        UpdateTaskbar = True
    End Sub

    Private Sub NumericUpDown2_KeyDown(sender As Object, e As KeyEventArgs) Handles NumericUpDown2.KeyDown
        UpdateTaskbar = True
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Select Case MsgBox("Are you Sure?", MsgBoxStyle.YesNo, "Reset settings...")
            Case MsgBoxResult.Yes
                ComboBox1.Text = "QuadEaseOut"
                NumericUpDown1.Value = 250
                NumericUpDown2.Value = 0
                NumericUpDown3.Value = 400
                CheckBox1.Checked = False
            Case MsgBoxResult.No

        End Select
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        My.Settings.Save()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Process.Start("https://easings.net/")
    End Sub
End Class