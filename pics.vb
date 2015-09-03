Public Class PicArray
    Inherits System.Collections.CollectionBase
    Private ReadOnly HostForm As Form2 ' System.Windows.Forms.Form


    Public Function AddNewPic(ByVal _x As Integer, ByVal _y As Integer, ByVal _width As Integer, ByVal _height As Integer) As System.Windows.Forms.PictureBox
        ' Create a new instance of the Button class.
        Dim aPic As New System.Windows.Forms.PictureBox()
        ' Add the picture to the collection's internal list.
        Me.List.Add(aPic)
        ' Add the pic to the controls collection of the form 
        ' referenced by the HostForm field.
        HostForm.Controls.Add(aPic)
        ' Set intial properties for the pic object.
        aPic.SetBounds(_x, _y, _width, _height)
        '        aPic.Top = y
        '        aPic.Left = x
        '        aPic.Height = height
        '        aPic.Width = width
        aPic.Tag = Me.Count
        aPic.Name = "Pic" & Me.Count.ToString


        aPic.BackColor = System.Drawing.SystemColors.HotTrack
        aPic.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        aPic.Cursor = System.Windows.Forms.Cursors.Default
        aPic.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        aPic.ForeColor = System.Drawing.SystemColors.ControlText
        'aPic.SetIndex(Me._Picture1_11, CType(11, Short))
        'aPic.Location = New System.Drawing.Point(846, 356)
        aPic.RightToLeft = System.Windows.Forms.RightToLeft.No
        'aPic.Size = New System.Drawing.Size(122, 142)
        aPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        'aPic.TabIndex = 11
        aPic.TabStop = False


        AddHandler aPic.Click, AddressOf ClickHandler
        AddHandler aPic.MouseHover, AddressOf MouseHoverHandler
        Return aPic
    End Function

    Public Sub New(ByRef host As Form2) 'System.Windows.Forms.Form)
        HostForm = host
        '        Me.AddNewPic()
    End Sub

    Default Public ReadOnly Property Item(ByVal Index As Integer) As  _
       System.Windows.Forms.PictureBox
        Get
            Return CType(Me.List.Item(Index), System.Windows.Forms.PictureBox)
        End Get
    End Property

    Public Sub ClickHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        HostForm.Pictures_Click(CType(sender, System.Windows.Forms.PictureBox).Tag - 1)
        ' MessageBox.Show("you have clicked picture " & CType(CType(sender,  _
        '    System.Windows.Forms.PictureBox).Tag, String))
    End Sub

    Public Sub MouseHoverHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        HostForm.Pictures_MouseHover(CType(sender, System.Windows.Forms.PictureBox).Tag - 1)
    End Sub

End Class