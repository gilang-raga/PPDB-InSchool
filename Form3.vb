Imports MySql.Data.MySqlClient

Public Class Form3
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim form7 As New Form7()
        form7.Show()
        Me.Close()
    End Sub

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        koneksi.nisnSiswaLogin = ""
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Dim form1 As New Form1()
        form1.Show()
        Me.Close()
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        Dim nisn As String = TextBox1.Text.Trim()
        Dim nama As String = TextBox2.Text.Trim()
        Dim password As String = TextBox3.Text

        If nisn = "" Or nama = "" Or password = "" Then
            MessageBox.Show("Silakan isi semua field.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' NISN only angka
        If Not IsNumeric(TextBox1.Text) Then
            MessageBox.Show("NISN harus berupa angka.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If


        Try
            bukaKoneksi()

            Dim query As String = "SELECT * FROM siswa WHERE nisn = @nisn AND nama_lengkap = @nama_lengkap AND password = @password"
            cmd = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@nisn", nisn)
            cmd.Parameters.AddWithValue("@nama_lengkap", nama)
            cmd.Parameters.AddWithValue("@password", password)


            rd = cmd.ExecuteReader()

            If rd.HasRows Then
                rd.Close()
                conn.Close()

                koneksi.nisnSiswaLogin = nisn

                MessageBox.Show("Login berhasil. Selamat datang, " & nama & "!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Dim form4 As New Form4()
                form4.Show()
                Me.Close()
            Else
                rd.Close()
                conn.Close()
                MessageBox.Show("Akun tidak ditemukan. Silakan registrasi terlebih dahulu.", "Gagal Login", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class
