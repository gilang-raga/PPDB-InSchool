Imports MySql.Data.MySqlClient

Public Class Form7
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Dim form1 As New Form1()
        form1.Show()
        Me.Close()
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        Dim nisn As String = TextBox1.Text
        Dim nama As String = TextBox2.Text
        Dim password As String = TextBox3.Text

        If nisn = "" Or nama = "" Or password = "" Then
            MessageBox.Show("Silakan isi NISN, Nama Lengkap, dan Password terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            bukaKoneksi()
            Dim query As String = "INSERT INTO siswa (nisn, nama_lengkap, password) VALUES (@nisn, @nama, @password)"
            cmd = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@nisn", nisn)
            cmd.Parameters.AddWithValue("@nama", nama)
            cmd.Parameters.AddWithValue("@password", password)
            cmd.ExecuteNonQuery()
            conn.Close()

            MessageBox.Show("Akun anda berhasil daftar!" & vbCrLf & "NISN: " & nisn & vbCrLf & "Nama Lengkap: " & nama & vbCrLf & "Password: " & password, "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Dim form3 As New Form3()
            form3.Show()
            Me.Close()
        Catch ex As MySqlException
            If ex.Number = 1062 Then
                MessageBox.Show("NISN sudah terdaftar.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                MessageBox.Show("Gagal menyimpan data: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub
End Class
