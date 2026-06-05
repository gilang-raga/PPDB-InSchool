Imports MySql.Data.MySqlClient

Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Bisa dikosongkan
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Form1.Show()
        Me.Close()
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        ' Asumsi TextBox1 = ID/Username, TextBox2 = Password
        Dim inputUser As String = TextBox1.Text.Trim()
        Dim inputPass As String = TextBox2.Text

        If inputUser = "" Or inputPass = "" Then
            MessageBox.Show("ID dan Password tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            bukaKoneksi() ' Panggil dari Module

            ' --- INI BAGIAN YANG DIPERBAIKI ---
            ' Query LAMA Anda (mungkin): "SELECT * FROM admin WHERE id = @id"
            ' Query BARU: Menggunakan kolom 'username'
            Dim query As String = "SELECT * FROM admin WHERE username = @user AND password = @pass"

            cmd = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@user", inputUser)
            cmd.Parameters.AddWithValue("@pass", inputPass) ' Cek password (plain text)

            rd = cmd.ExecuteReader()

            If rd.HasRows Then
                ' Login Berhasil
                rd.Close()
                conn.Close()
                MessageBox.Show("Login Admin Berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Buka form data siswa (Form9)
                Form9.Show()
                Me.Close()
            Else
                ' Login Gagal
                rd.Close()
                conn.Close()
                MessageBox.Show("ID atau Password salah.", "Gagal Login", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan database: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class
