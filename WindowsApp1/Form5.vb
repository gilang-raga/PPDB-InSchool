Imports MySql.Data.MySqlClient

Public Class Form5
    Dim conn As MySqlConnection
    Dim cmd As MySqlCommand
    Private Sub button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            bukaKoneksi()

            Dim jk As String = ""
            If RadioButton1.Checked Then
                jk = "Laki-laki"
            ElseIf RadioButton2.Checked Then
                jk = "Perempuan"
            End If

            Dim sql As String = "INSERT INTO tb_siswa (jenis_pendaftaran, jalur_pendaftaran, jurusan, nik, asal_sekolah, nama_lengkap, jenis_kelamin, agama, tempat_lahir, tanggal_lahir, alamat, no_kk, no_akta)
                                 VALUES (@jenis, @jalur, @jurusan, @nik, @asal, @nama, @jk, @agama, @tempat, @tgl, @alamat, @kk, @akta)"

            cmd = New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@jenis", Form4.ComboBox1.Text)
            cmd.Parameters.AddWithValue("@jalur", Form4.ComboBox2.Text)
            cmd.Parameters.AddWithValue("@jurusan", Form4.ComboBox3.Text)
            cmd.Parameters.AddWithValue("@nik", Form4.TextBox1.Text)
            cmd.Parameters.AddWithValue("@asal", Form4.TextBox2.Text)
            cmd.Parameters.AddWithValue("@nama", TextBox1.Text)
            cmd.Parameters.AddWithValue("@jk", jk)
            cmd.Parameters.AddWithValue("@agama", ComboBox1.Text)
            cmd.Parameters.AddWithValue("@tempat", TextBox2.Text)
            cmd.Parameters.AddWithValue("@tgl", DateTimePicker1.Value.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@alamat", TextBox5.Text)
            cmd.Parameters.AddWithValue("@kk", TextBox3.Text)
            cmd.Parameters.AddWithValue("@akta", TextBox4.Text)

            cmd.ExecuteNonQuery()

            MsgBox("Data berhasil disimpan ke database MySQL", MsgBoxStyle.Information)

            conn.Close()
            Form6.Show()
            Me.Hide()

        Catch ex As Exception
            MsgBox("Gagal menyimpan data: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
