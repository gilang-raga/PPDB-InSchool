Public Class Form1
    Private Sub btnSiswa_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form7.Show()
        Me.Hide()
    End Sub

    Private Sub btnAdmin_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form2.Show()
        Me.Hide()
    End Sub
End Class
