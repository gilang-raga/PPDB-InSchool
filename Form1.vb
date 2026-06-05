Public Class Form1
    Inherits System.Windows.Forms.Form

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim form3 As New Form3
        form3.Show()
        Me.Hide()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim form2 As New Form2
        form2.Show()
        Me.Hide()

    End Sub
End Class
