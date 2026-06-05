Public Class Form7
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim nisn As String = TextBox1.Text
        Dim nama_lengkap As String = TextBox2.Text
        If nisn <> "" And nama_lengkap <> "" Then
            MsgBox("Login berhasil, selamat datang " & nama_lengkap, MsgBoxStyle.Information)
            Form4.Show()
            Me.Hide()
        Else
            MsgBox("Isi NISN dan Nama Lengkap terlebih dahulu", MsgBoxStyle.Exclamation)
        End If
    End Sub
End Class