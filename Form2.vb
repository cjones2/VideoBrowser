Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Public Class Form2
    Inherits System.Windows.Forms.Form
    Dim MainPath As String
    Dim rootPath As String
    Dim VLCExt(20) As String
    Dim VLCExtNum As Integer
    Dim paths(30) As String
    Dim videoIcons(4) As String
    Dim numpaths As Short
    Dim folders(80) As String
    Dim numfolders As Short
    Dim folderpaths(80) As String

    Dim Pictures As PicArray
    Dim Titles As TitleArray

    Dim Rows As Integer
    Dim columns As Integer
    Dim Items As Integer

    Dim ignoremouse As Integer
    Dim currentmouse As Integer

    Dim currentObj As Short
    Dim topObj As Object
    Dim totalObjs As Object
    Dim highlight As Object

    Dim roottop As Object
    Dim rootcurrent As Object
    Dim objType(400) As String
    Dim clicked As Integer
    Dim wheeled As Integer


    Public NotInheritable Class Win32Helper
        <System.Runtime.InteropServices.DllImport("user32.dll", _
        EntryPoint:="SetForegroundWindow", _
        CallingConvention:=Runtime.InteropServices.CallingConvention.StdCall, _
        CharSet:=Runtime.InteropServices.CharSet.Unicode, SetLastError:=True)> _
        Public Shared Function _
             SetForegroundWindow(ByVal handle As IntPtr) As Boolean
            ' Leave function empty
        End Function

        <System.Runtime.InteropServices.DllImport("user32.dll", _
        EntryPoint:="ShowWindow", _
        CallingConvention:=Runtime.InteropServices.CallingConvention.StdCall, _
        CharSet:=Runtime.InteropServices.CharSet.Unicode, SetLastError:=True)> _
        Public Shared Function ShowWindow(ByVal handle As IntPtr, _
                                     ByVal nCmd As Int32) As Boolean
            ' Leave function empty 
        End Function

    End Class ' End Win32Helper 

    Private Property fname As String
    Private Property cropstring As String


    Public Shared Function GetRunningInstance(ByVal _
                       processName As String) As Process
        Dim proclist() As Process = _
           Process.GetProcessesByName(processName)
        For Each p As Process In proclist
            If p.Id <> Process.GetCurrentProcess().Id Then
                Return p
            End If
        Next
        Return Nothing
    End Function


    Sub findfolders(ByRef path As Object)
        On Error Resume Next
        Dim DirName, i As Object ' Declare variables.
        DirName = Dir(path, 16) ' Get first directory name.
        List1.Items.Clear()
        List2.Items.Clear()

        Do While DirName <> ""
            ' A file or directory name was returned
            'UPGRADE_WARNING: Couldn't resolve default property of object DirName. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If DirName <> "." And DirName <> ".." Then
                ' Not a parent or current directory entry so process it
                i = GetAttr(path + DirName)
                If (i And 16) <> 0 Then
                    ' This is a directory
                    folders(numfolders) = DirName
                    folderpaths(numfolders) = path & DirName & "\"
                    numfolders = numfolders + 1
                End If
            End If
            DirName = Dir() ' Get another directory name.
        Loop

    End Sub

    Sub FindDirs(ByRef path As Object)
        On Error Resume Next
        Dim DirName, i As Object ' Declare variables.
        Dim fullname As String
        DirName = Dir(path, 16) ' Get first directory name.
        List1.Items.Clear()
        List2.Items.Clear()

        Do While DirName <> ""
            ' A file or directory name was returned
            If DirName <> "." And DirName <> ".." Then
                ' Not a parent or current directory entry so process it
                Dim ext As String = UCase(VB.Right(DirName, 4))
                fullname = path & DirName
                i = GetAttr(fullname)
                If (i And 16) <> 0 Then
                    ' This is a directory
                    List1.Items.Add(fullname)
                ElseIf ext = ".URL" Then
                    List2.Items.Add(fullname)
                ElseIf IsVLCExt(DirName) Then
                    List2.Items.Add(fullname)
                End If
            End If
                DirName = Dir() ' Get another directory name.
        Loop
        totalObjs = List1.Items.Count + List2.Items.Count
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Function IsVLCExt(ByVal DirName As String) As Boolean
        Dim l As Integer
        Dim ext As String = UCase(VB.Right(DirName, 4))
        For l = 1 To VLCExtNum
            If ext = VLCExt(l) Then
                Return True
            End If
        Next
        Return False
    End Function

    Function ShowName(ByRef nam As String) As String
        Dim i, l As Object
        l = 3
        For i = 1 To Len(nam)
            If Mid(nam, i, 1) = "\" Then
                l = i
            End If
        Next
        ShowName = Mid(nam, l + 1, Len(nam) - l - 4)
    End Function

    Function TryPicture(ByRef pic As System.Windows.Forms.PictureBox, ByRef nam As String) As Boolean
        Try
            pic.Image.Dispose()

        Catch ex As Exception

        End Try

        TryPicture = False
        Try
            pic.Image = System.Drawing.Image.FromFile(nam)
            TryPicture = True
        Catch ex As System.IO.FileNotFoundException
        End Try
    End Function


    Sub SetPicture(ByRef pic As System.Windows.Forms.PictureBox, ByRef nam As String, ByRef index As Integer)

        nam = Mid(nam, 1, Len(nam) - 4)
        If TryPicture(pic, nam & ".jpg") = True Then
            Exit Sub
        End If
        If TryPicture(pic, nam & ".png") = True Then
            Exit Sub
        End If
        If TryPicture(pic, nam & ".gif") = True Then
            Exit Sub
        End If
        If TryPicture(pic, nam & ".bmp") = True Then
            Exit Sub
        End If
        If (objType(index) = "folder") Or (objType(index) = "root") Then
            TryPicture(pic, videoIcons(0))
            Exit Sub
        End If

        If objType(index) = "folderup" Then
            TryPicture(pic, videoIcons(1))
            Exit Sub
        End If

        If objType(index) = "url" Then
            TryPicture(pic, videoIcons(3))
            Exit Sub
        End If

        TryPicture(pic, videoIcons(2))

fallthrough2:

    End Sub
    Sub ShowDirs()
        Dim i, l As Object
        'Dim c As String
        Dim val As String

        Titles(highlight).BorderStyle = System.Windows.Forms.BorderStyle.None
        Pictures(highlight).BorderStyle = System.Windows.Forms.BorderStyle.None
        i = 0
        If rootPath = "" Then
            Button4.Text = "Refresh"
            For l = 0 To numfolders - 1
                If (l >= topObj) And (l < topObj + (Items - 1)) Then
                    Titles(i - topObj).Text = folders(l)
                    Titles(i - topObj).Visible = True
                    Pictures(i - topObj).Visible = True
                    objType(i) = "root"
                    SetPicture(Pictures(i - topObj), folderpaths(l) & "folder.dir", i - topObj)
                    If topObj + currentObj = i Then
                        Titles(i - topObj).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
                        Pictures(i - topObj).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
                        highlight = i - topObj
                        Label2.Text = Titles(i - topObj).Text
                        Label2.Left = (Me.Width - Label2.Width) / 2
                    End If
                End If
                i = i + 1
            Next
            For l = i - topObj To (Items - 1)
                Titles(l).Visible = False
                Pictures(l).Visible = False
            Next
            Exit Sub
        End If

        Button4.Text = "Exit"
        If Len(MainPath) > 3 Then
            If topObj = 0 Then
                objType(i - topObj) = "uplink"
                Titles(i).Text = "Parent Folder"
                Titles(i).Visible = True
                Pictures(i).Visible = True
                Pictures(i).Image = System.Drawing.Image.FromFile(videoIcons(1))
                If topObj + currentObj = i Then
                    Titles(i).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
                    Pictures(i).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
                    highlight = i - topObj
                    Label2.Text = Titles(i - topObj).Text
                    Label2.Left = (Me.Width - Label2.Width) / 2
                End If
            End If
            i = i + 1
        End If

        For l = 0 To List1.Items.Count - 1
            If (i >= topObj) And (i < topObj + 24) Then
                objType(i - topObj) = "folder"
                Titles(i - topObj).Text = ShowName(VB6.GetItemString(List1, l))
                Titles(i - topObj).Visible = True
                Pictures(i - topObj).Visible = True
                SetPicture(Pictures(i - topObj), VB6.GetItemString(List1, l), i - topObj)
                If topObj + currentObj = i Then
                    Titles(i - topObj).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
                    Pictures(i - topObj).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
                    highlight = i - topObj
                    Label2.Text = Titles(i - topObj).Text
                    Label2.Left = (Me.Width - Label2.Width) / 2
                End If
            End If
            i = i + 1
        Next
        For l = 0 To List2.Items.Count - 1
            If (i >= topObj) And (i < topObj + (Rows * columns)) Then
                val = VB6.GetItemString(List2, l)
                objType(i - topObj) = Mid(val, Len(val) - 3)
                Titles(i - topObj).Text = ShowName(val)
                Titles(i - topObj).Visible = True
                Pictures(i - topObj).Visible = True
                SetPicture(Pictures(i - topObj), val, i - topObj)
                If topObj + currentObj = i Then
                    Titles(i - topObj).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
                    Pictures(i - topObj).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
                    highlight = i - topObj
                    Label2.Text = Titles(i - topObj).Text
                    Label2.Left = (Me.Width - Label2.Width) / 2
                End If
            End If
            i = i + 1
        Next
        For l = i - topObj To (Items - 1)
            Titles(l).Visible = False
            Pictures(l).Visible = False
        Next
    End Sub
    Sub ChangeHighlight(ByRef offs As Object)
        Titles(highlight).BorderStyle = System.Windows.Forms.BorderStyle.None
        Pictures(highlight).BorderStyle = System.Windows.Forms.BorderStyle.None
        highlight = highlight + offs
        Titles(highlight).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Pictures(highlight).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Label2.Text = Titles(highlight).Text
        Label2.Left = (Me.Width - Label2.Width) / 2

    End Sub
    Sub NewHighlight(ByRef offs As Object)
        Titles(highlight).BorderStyle = System.Windows.Forms.BorderStyle.None
        Pictures(highlight).BorderStyle = System.Windows.Forms.BorderStyle.None
        highlight = offs
        currentObj = offs
        Titles(highlight).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Pictures(highlight).BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Label2.Text = Titles(highlight).Text
        Label2.Left = (Me.Width - Label2.Width) / 2
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        Dim bHandled As Boolean = False
        Select Case keyData
            Case Keys.Up
                'do whatever....up arrow pressed
                ignoremouse = currentmouse
                bHandled = True
                DoKey(38)
            Case Keys.Down
                'do whatever....down arrwo pressed
                ignoremouse = currentmouse
                bHandled = True
                DoKey(40)
            Case Keys.Left
                ignoremouse = currentmouse
                bHandled = True
                DoKey(37)
            Case Keys.Right
                ignoremouse = currentmouse
                bHandled = True
                DoKey(39)
        End Select
        Return bHandled

    End Function


    Private Sub Form2_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Dim KeyCode As Short = eventArgs.KeyCode
        Dim Shift As Short = eventArgs.KeyData \ &H10000
        ignoremouse = currentmouse
        'MessageBox.Show("you have hit key " & KeyCode.ToString)
        DoKey(KeyCode)
    End Sub
    Public Sub DoKey(ByVal KeyCode As Short)
        Dim nextobj As Integer

        If KeyCode = 27 Or KeyCode = 178 Then

            MainPath = ""
            rootPath = ""
            topObj = roottop
            currentObj = rootcurrent
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            ShowDirs()
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End If

        If rootPath = "" Then
            ' right arrow
            If KeyCode = 39 Then
                If topObj + currentObj + 1 < numfolders Then
                    If currentObj = (Items - 1) Then
                        currentObj = (columns * (Rows - 1))
                        topObj = topObj + columns
                        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                        ShowDirs()
                        Me.Cursor = System.Windows.Forms.Cursors.Default
                    Else
                        currentObj = currentObj + 1
                        ChangeHighlight(1)
                    End If
                End If
            End If

            'down arrow
            If KeyCode = 40 Then
                If topObj + currentObj + columns < numfolders Then
                    If currentObj < (columns * (Rows - 1)) Then
                        currentObj = currentObj + columns
                        ChangeHighlight(columns)
                    Else
                        topObj = topObj + columns
                        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                        ShowDirs()
                        Me.Cursor = System.Windows.Forms.Cursors.Default
                    End If
                Else
                    If topObj + (columns * Rows) <= numfolders Then
                        topObj = topObj + columns
                        currentObj = totalObjs - 1
                        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                        ShowDirs()
                        Me.Cursor = System.Windows.Forms.Cursors.Default
                    End If
                End If
            End If
        Else
            ' right arrow
            If KeyCode = 39 Then
                If topObj + currentObj < totalObjs Then
                    If currentObj = (Items - 1) Then
                        currentObj = (columns * (Rows - 1))
                        topObj = topObj + columns
                        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                        ShowDirs()
                        Me.Cursor = System.Windows.Forms.Cursors.Default
                    Else
                        currentObj = currentObj + 1
                        ChangeHighlight(1)
                    End If
                End If
            End If

            'down arrow
            If KeyCode = 40 Then
                If topObj + currentObj + columns <= totalObjs Then
                    If currentObj < (columns * (Rows - 1)) Then
                        currentObj = currentObj + columns
                        ChangeHighlight(columns)
                    Else
                        topObj = topObj + columns
                        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                        ShowDirs()
                        Me.Cursor = System.Windows.Forms.Cursors.Default
                    End If
                Else
                    nextobj = topObj + currentObj + (columns - (currentObj Mod columns))
                    If nextobj <= totalObjs Then
                        nextobj = (Rows * columns) - (columns - (totalObjs Mod columns))
                        nextobj = nextobj - highlight
                        If topObj + (Rows * columns) <= totalObjs Then
                            topObj = topObj + columns
                            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                            ShowDirs()
                            Me.Cursor = System.Windows.Forms.Cursors.Default
                        End If
                        currentObj = currentObj + nextobj
                        ChangeHighlight(nextobj)
                    End If
                End If
            End If
        End If

        'up arrow
        If KeyCode = 38 Then
            If topObj + currentObj > (columns - 1) Then
                If currentObj > (columns - 1) Then
                    currentObj = currentObj - columns
                    ChangeHighlight(-columns)
                Else
                    topObj = topObj - columns
                    Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                    ShowDirs()
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                End If
            End If
        End If

        'left arrow
        If KeyCode = 37 Then
            If topObj + currentObj > 0 Then
                If currentObj = 0 Then
                    currentObj = (columns - 1)
                    topObj = topObj - columns
                    Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                    ShowDirs()
                    Me.Cursor = System.Windows.Forms.Cursors.Default
                Else
                    currentObj = currentObj - 1
                    ChangeHighlight(-1)
                End If
            End If
        End If

    End Sub
    Private Sub Form2_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        If KeyAscii = 13 Then
            Pictures_Click(currentObj)
        ElseIf KeyAscii = 27 Then
            DoKey(27)
            On Error Resume Next
        End If

        eventArgs.KeyChar = Chr(KeyAscii)
        If KeyAscii = 0 Then
            eventArgs.Handled = True
        End If
    End Sub

    Private Sub TerminateProcs(ByRef procname As String)
        ' terminate any running Video Lan applications
        Dim procs() As Process = System.Diagnostics.Process.GetProcessesByName(procname)
        For Each proc As Process In procs
            proc.Kill()
        Next

    End Sub

    Private Sub Form2_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        Dim tmp As Object
        Dim l As Object
        Dim i As Object
        'Dim x As Integer
        'Dim y As Integer
        Dim LeftEdge As Integer = 120
        Dim TopEdge As Integer = 60

        Dim SizeY As Integer
        Dim LabelY As Integer
        Dim BarX As Integer
        Dim BarY As Integer
        Dim GapX As Integer
        Dim GapY As Integer


        Me.KeyPreview = True

        ' Define rows and columns here and they get used everywhere
        ' Form2_Load lays out the controls
        Rows = 3
        columns = 8
        Items = Rows * columns

        Dim intX As Integer = Screen.PrimaryScreen.Bounds.Width
        Dim intY As Integer = Screen.PrimaryScreen.Bounds.Height

        Button5.Left = intX - Button5.Width
        Button6.Left = Button5.Left
        Button6.Focus()

        GapY = (intY - TopEdge) / (Rows * 14)
        GapX = (intX - LeftEdge) / (columns * 10)

        LabelY = GapY * 3

        SizeY = ((intY - TopEdge) - ((Rows + 2) * GapY)) / Rows
        BarX = ((intX - LeftEdge) - ((columns + 2) * GapY)) / columns

        BarY = SizeY - (LabelY + GapY)

        Pictures = New PicArray(Me)
        Titles = New TitleArray(Me)


        Me.SetBounds(0, 0, intX, intY)

        For i = 0 To Rows - 1 Step 1
            For l = 0 To columns - 1 Step 1
                Pictures.AddNewPic(LeftEdge + GapX + ((GapX + BarX) * l), TopEdge + GapY + ((GapY + SizeY) * i) + LabelY + GapY, BarX, BarY)
                Titles.AddNewTitle(LeftEdge + GapX + ((GapX + BarX) * l), TopEdge + GapY + ((GapY + SizeY) * i), BarX, LabelY)
            Next
        Next

        VLCExt(1) = ".ISO"
        VLCExt(2) = ".VOB"
        VLCExt(3) = ".MPG"
        VLCExt(4) = ".AVI"
        VLCExt(5) = ".MOV"
        VLCExt(6) = ".MP4"
        VLCExt(7) = ".WMV"
        VLCExtNum = 7

        TerminateProcs("vlc")
        TerminateProcs("RICOH THETA")
        ' exit if we are already running
        Dim pInstance As Process = GetRunningInstance(Process.GetCurrentProcess().ProcessName)
        If Not pInstance Is Nothing Then
            Dim handle As IntPtr = pInstance.MainWindowHandle
            If Not IntPtr.Zero.Equals(handle) Then
                Win32Helper.ShowWindow(handle, 1)
                Win32Helper.SetForegroundWindow(handle)
            End If
            Application.ExitThread()
            Exit Sub
        End If

        If System.IO.File.Exists("C:\videopaths.txt") <> True Then
            MessageBox.Show("C:\VideoPaths.txt does not exist", "VideoBrowser")
            Application.ExitThread()
            Exit Sub
        End If
        Try
            FileOpen(1, "c:\VideoPaths.txt", OpenMode.Input)
        Catch io As System.IO.IOException
            MessageBox.Show(io.Message, "VideoBrowser")
            Application.ExitThread()
            Exit Sub
        Catch ex As Exception
            MessageBox.Show(ex.Message, "VideoBrowser")
            Application.ExitThread()
            Exit Sub
        End Try

        Do While Not EOF(1)
                Input(1, paths(numpaths))
                numpaths = numpaths + 1
            Loop
        FileClose(1)

        If System.IO.File.Exists("C:\videoicons.txt") <> True Then
            MessageBox.Show("C:\VideoIcons.txt does not exist", "VideoBrowser")
            Application.ExitThread()
            Exit Sub
        End If

        FileOpen(1, "c:\VideoIcons.txt", OpenMode.Input)
        Input(1, videoIcons(0)) ' folder
        Input(1, videoIcons(1)) ' folder up
        Input(1, videoIcons(2)) ' movie
        Input(1, videoIcons(3)) ' url
        FileClose(1)
        For i = 0 To numpaths - 1
            findfolders((paths(i)))
        Next
        For i = numfolders - 1 To 1 Step -1
            For l = 0 To i - 1
                If UCase(folders(l)) > UCase(folders(l + 1)) Then
                    tmp = folders(l)
                    folders(l) = folders(l + 1)
                    folders(l + 1) = tmp
                    tmp = folderpaths(l)
                    folderpaths(l) = folderpaths(l + 1)
                    folderpaths(l + 1) = tmp
                End If
            Next
        Next
        rootPath = ""
        MainPath = ""
        topObj = 0
        currentObj = 0
        'Me.Width = 1360
        'Me.Height = 766
        Me.CenterToScreen()

        Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
        ShowDirs()
        Me.Cursor = System.Windows.Forms.Cursors.Default

    End Sub

  
    Public Sub Pictures_Click(ByVal index As Short)
        'Dim Index As Short '= Pictures.GetIndex(eventSender)
        Dim i, l As Object
        Dim ex As String
        Dim fullName As String
        Dim pInstance As Process

        If clicked <> 0 Then
            Exit Sub
        End If
        clicked = 1
        Timer1.Enabled = True

        On Error Resume Next
        If rootPath = "" Then
            roottop = topObj
            rootcurrent = index

            MainPath = folderpaths(topObj + index)
            rootPath = MainPath
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            FindDirs(MainPath)
            NewHighlight(0)
            topObj = 0
            ShowDirs()
            Me.Cursor = System.Windows.Forms.Cursors.Default
            Exit Sub
        End If

        If topObj + index = 0 Then
            If MainPath = rootPath Then
                MainPath = ""
                rootPath = ""
                topObj = roottop
                currentObj = rootcurrent
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                ShowDirs()
                Me.Cursor = System.Windows.Forms.Cursors.Default
                Exit Sub
            Else
                l = 0
                For i = 1 To Len(MainPath) - 1
                    If Mid(MainPath, i, 1) = "\" Then
                        l = i
                    End If
                Next
                MainPath = Mid(MainPath, 1, l)
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                FindDirs(MainPath)
                NewHighlight(0)
                topObj = 0
                ShowDirs()
                Me.Cursor = System.Windows.Forms.Cursors.Default
                Exit Sub
            End If
        Else
            i = 0
            i = GetAttr(rootPath & Titles(index).Text)
            If (i And 16) <> 0 Then
                MainPath = MainPath & Titles(index).Text & "\"
                Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
                FindDirs(MainPath)
                NewHighlight(0)
                topObj = 0
                ShowDirs()
                Me.Cursor = System.Windows.Forms.Cursors.Default
                Exit Sub
            End If

            fullName = rootPath & Titles(index).Text + objType(index)
            i = 16
            i = GetAttr(fullName)
            If (i And 16) = 0 Then
                If IsVLCExt(objType(index)) Then
                    pInstance = GetRunningInstance("vlc")
                    If Not pInstance Is Nothing Then
                        'Dim handle As IntPtr = pInstance.MainWindowHandle
                        'If Not IntPtr.Zero.Equals(handle) Then
                        'Win32Helper.ShowWindow(Handle, 1)
                        'Win32Helper.SetForegroundWindow(Handle)
                        'End If
                        pInstance.Kill()
                    End If
                    'ps = GetProcessesByName("VLC media player")
                    'm_hWnd = FindWindow(vbNullString, "VLC media player")
                    'If m_hWnd > 0 Then

                    fname = rootPath + Titles(index).Text
                    If My.Computer.FileSystem.FileExists(fname + ".360") Then
                        Process.Start("C:\Program Files (x86)\RICOH THETA\RICOH THETA.exe", Chr(34) + fullName + Chr(34))
                        Exit Sub
                    End If

                    If My.Computer.FileSystem.FileExists(fname + ".crop") Then
                        cropstring = " --crop 16:9 "
                    End If

                    Process.Start("C:\Program Files\VideoLAN\VLC\vlc.exe", cropstring + " --volume=400 -f " + Chr(34) + fullName + Chr(34))
                    NewHighlight((index))
                ElseIf UCase(objType(index)) = ".URL" Then
                    Process.Start(fullName)
                    NewHighlight((index))
                End If
                Exit Sub
            End If
        End If

    End Sub


    Private Sub Form2_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
        wheeled = 1
        Timer2.Enabled = True
        If e.Delta < 0 Then
            DoKey(38)
        End If

        If e.Delta > 0 Then
            DoKey(40)
        End If

    End Sub

    Public Sub Pictures_MouseHover(ByVal index As Integer)
        'Dim Index As Short '= Pictures.GetIndex(sender)
        If wheeled <> 0 Then
            Exit Sub
        End If
        If (index = ignoremouse) Then
            Exit Sub
        Else
            ignoremouse = -1
        End If
        If (highlight <> index) Then
            If rootPath = "" Then
                If index + topObj < numfolders Then
                    currentmouse = index
                    NewHighlight(index)
                End If
            Else
                If index + topObj <= totalObjs Then
                    currentmouse = index
                    NewHighlight(index)
                End If
            End If
        End If
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        clicked = 0
        Timer1.Enabled = False
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        wheeled = False
        Timer2.Enabled = False
    End Sub

    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        Dim pInstance As Process

        pInstance = GetRunningInstance("vlc")
        '        If Not pInstance Is Nothing Then
        ' Button2.Visible = True
        'Else
        'Button2.Visible = False
        'End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim pInstance As Process

        pInstance = GetRunningInstance("vlc")
        If Not pInstance Is Nothing Then
            Dim handle As IntPtr = pInstance.MainWindowHandle
            If Not IntPtr.Zero.Equals(handle) Then
                Win32Helper.ShowWindow(handle, 1)
                Win32Helper.SetForegroundWindow(handle)
            End If
        End If
        Button6.Focus()

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        DoKey(38)
    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        DoKey(40)
    End Sub

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Label4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        DoKey(38)
    End Sub

    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        DoKey(40)
    End Sub

    Private Sub _Pictures_3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub


    Private Sub Button1_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        DoKey(38)
        Button6.Focus()

    End Sub

    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        DoKey(40)
        Button6.Focus()
    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Me.WindowState = FormWindowState.Minimized
        Button6.Focus()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim tmp As Object
        Dim l As Object
        Dim i As Object
        'Dim pInstance As Process

        If rootPath = "" Then
            numfolders = 0
            For i = 0 To numpaths - 1
                findfolders((paths(i)))
            Next
            For i = numfolders - 1 To 1 Step -1
                For l = 0 To i - 1
                    If UCase(folders(l)) > UCase(folders(l + 1)) Then
                        tmp = folders(l)
                        folders(l) = folders(l + 1)
                        folders(l + 1) = tmp
                        tmp = folderpaths(l)
                        folderpaths(l) = folderpaths(l + 1)
                        folderpaths(l + 1) = tmp
                    End If
                Next
            Next
            rootPath = ""
            MainPath = ""
            topObj = 0
            currentObj = 0
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
            ShowDirs()
            Me.Cursor = System.Windows.Forms.Cursors.Default

        Else
            DoKey(27)
        End If
        Button6.Focus()
    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click
        End
    End Sub

    Private Sub Button6_Click(sender As System.Object, e As System.EventArgs) Handles Button6.Click
        Pictures_Click(currentObj)
    End Sub

    Private Sub Form2_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        Button6.Focus()

    End Sub
End Class