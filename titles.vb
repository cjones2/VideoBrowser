Public Class TitleArray
    Inherits System.Collections.CollectionBase
    Private ReadOnly HostForm As Form2 'System.Windows.Forms.Form


    Public Function AddNewTitle(ByVal _x As Integer, ByVal _y As Integer, ByVal _width As Integer, ByVal _height As Integer) As System.Windows.Forms.Label
        ' Create a new instance of the Button class.
        Dim aTitle As New System.Windows.Forms.Label()
        ' Add the picture to the collection's internal list.
        Me.List.Add(aTitle)
        ' Add the pic to the controls collection of the form 
        ' referenced by the HostForm field.
        HostForm.Controls.Add(aTitle)
        ' Set intial properties for the pic object.
        aTitle.SetBounds(_x, _y, _width, _height)
        '        aTitle.Top = y
        '        aTitle.Left = x
        '        aTitle.Height = height
        '        aTitle.Width = width
        aTitle.Tag = Me.Count
        aTitle.Name = "Pic" & Me.Count.ToString
        aTitle.Text = _x.ToString & " " & _y.ToString & "    " & _width.ToString & " x " & _height.ToString




        aTitle.BackColor = HostForm.BackColor

        aTitle.BorderStyle = System.Windows.Forms.BorderStyle.None
        aTitle.Cursor = System.Windows.Forms.Cursors.Default
        aTitle.Font = New System.Drawing.Font("Arial", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        aTitle.ForeColor = System.Drawing.SystemColors.ControlText
        'aTitle.SetIndex(Me._Picture1_11, CType(11, Short))
        'aTitle.Location = New System.Drawing.Point(846, 356)
        aTitle.RightToLeft = System.Windows.Forms.RightToLeft.No
        'aTitle.Size = New System.Drawing.Size(122, 142)
        'aTitle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        aTitle.TextAlign = ContentAlignment.BottomCenter
        aTitle.TabStop = False


        AddHandler aTitle.Click, AddressOf ClickHandler
        AddHandler aTitle.MouseHover, AddressOf MouseHoverHandler
        Return aTitle
    End Function

    Public Sub New(ByVal host As System.Windows.Forms.Form)
        HostForm = host
        '        Me.AddNewPic()
    End Sub

    Default Public ReadOnly Property Item(ByVal Index As Integer) As  _
       System.Windows.Forms.Label
        Get
            Return CType(Me.List.Item(Index), System.Windows.Forms.Label)
        End Get
    End Property

    Public Sub ClickHandler(ByVal sender As Object, ByVal e As  _
       System.EventArgs)
        HostForm.Pictures_Click(CType(sender, System.Windows.Forms.Label).Tag - 1)
        'MessageBox.Show("you have clicked label " & CType(CType(sender,  _
        '   System.Windows.Forms.Label).Tag, String))
    End Sub

    Public Sub MouseHoverHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        ' HostForm.Pictures_MouseHover(CType(sender, System.Windows.Forms.PictureBox).Tag - 1)
    End Sub

End Class
