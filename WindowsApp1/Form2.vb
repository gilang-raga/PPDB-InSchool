Public Class Form2
    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim id As String = TextBox1.Text
        Dim pass As String = TextBox2.Text

        If id = "admin" And pass = "12345" Then
            MsgBox("Login berhasil sebagai Admin", MsgBoxStyle.Information)
            Form6()
            Me.Hide()
        Else
            MsgBox("ID atau Password salah", MsgBoxStyle.Critical)
        End If
    End Sub
End Class
