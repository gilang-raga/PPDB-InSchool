Imports MySql.Data.MySqlClient

Public Class Form8
    ' --- Properti Publik ---
    ' Ini akan diisi oleh Form4
    Public JenisPendaftaran As String
    Public JalurPendaftaran As String
    Public IdJurusanPilihan As Integer
    Public NIK As String
    Public AsalSekolah As String
    ' ----------------------
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Dim f4 As New Form4()
        f4.Show()
        Me.Close()
    End Sub
    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        ' 1. Ambil data dari Form8
        Dim nama_lengkap As String = TextBox1.Text.Trim()
        Dim jenisKelamin As String = ""
        If RadioButton1.Checked Then
            jenisKelamin = "Laki-laki"
        ElseIf RadioButton2.Checked Then
            jenisKelamin = "Perempuan"
        End If
        Dim agama As String = ComboBox1.Text
        Dim tgl_lahir As String = DateTimePicker1.Value.ToString("yyyy-MM-dd") ' Format MySQL
        Dim tmp_lahir As String = TextBox2.Text.Trim()
        Dim no_kk As String = TextBox9.Text.Trim()
        Dim no_akta As String = TextBox10.Text.Trim()
        Dim alamat_lengkap As String = TextBox3.Text.Trim()
        Dim rt_rw As String = TextBox4.Text.Trim()
        Dim desa As String = TextBox5.Text.Trim()
        Dim kecamatan As String = TextBox6.Text.Trim()
        Dim kota As String = TextBox7.Text.Trim()
        Dim provinsi As String = TextBox8.Text.Trim()
        Dim kode_pos As String = TextBox11.Text.Trim()

        ' 2. Validasi form
        If nama_lengkap = "" Or jenisKelamin = "" Or agama = "" Or tmp_lahir = "" Or alamat_lengkap = "" Or no_kk = "" Or no_akta = "" Or rt_rw = "" Or desa = "" Or kecamatan = "" Or kota = "" Or provinsi = "" Or kode_pos = "" Then
            MessageBox.Show("Harap isi semua data terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 3. Konfirmasi simpan
        Dim konfirmasi As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menyimpan data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If konfirmasi = DialogResult.No Then Exit Sub

        ' 4. Proses Transaksi Database
        bukaKoneksi() ' Pakai koneksi global
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        cmd = conn.CreateCommand()
        cmd.Transaction = transaction

        Try
            ' LANGKAH A: INSERT KE TABEL INDUK (pendaftaran)
            ' =========================================================
            ' ==    INI ADALAH BARIS YANG DIPERBAIKI                 ==
            ' =========================================================
            Dim sqlParent As String = "INSERT INTO pendaftaran " &
            "(nisn, id_jurusan, jenis_pendaftaran, jalur_pendaftaran, nik, asal_sekolah, " &
            "nama_lengkap, jenis_kelamin, agama, tempat_lahir, tanggal_lahir, no_kk, no_akta_kelahiran) " & ' <-- SUDAH DIGANTI KE 'no_kk'
            "VALUES " &
            "(@nisn, @id_jurusan, @jenis, @jalur, @nik, @asal, " &
            "@nama, @jk, @agama, @tmp, @tgl, @kk, @akta)"
            ' =========================================================

            cmd.CommandText = sqlParent
            cmd.Parameters.AddWithValue("@nisn", koneksi.nisnSiswaLogin) ' Ambil dari Global
            cmd.Parameters.AddWithValue("@id_jurusan", Me.IdJurusanPilihan) ' Ambil dari Properti
            cmd.Parameters.AddWithValue("@jenis", Me.JenisPendaftaran)
            cmd.Parameters.AddWithValue("@jalur", Me.JalurPendaftaran)
            cmd.Parameters.AddWithValue("@nik", Me.NIK)
            cmd.Parameters.AddWithValue("@asal", Me.AsalSekolah)
            cmd.Parameters.AddWithValue("@nama", nama_lengkap)
            cmd.Parameters.AddWithValue("@jk", jenisKelamin)
            cmd.Parameters.AddWithValue("@agama", agama)
            cmd.Parameters.AddWithValue("@tmp", tmp_lahir)
            cmd.Parameters.AddWithValue("@tgl", tgl_lahir)
            cmd.Parameters.AddWithValue("@kk", no_kk)
            cmd.Parameters.AddWithValue("@akta", no_akta)
            cmd.ExecuteNonQuery()

            ' LANGKAH B: AMBIL ID YANG BARU DIBUAT
            Dim newPendaftaranId As Long = cmd.LastInsertedId

            ' LANGKAH C: INSERT KE TABEL ANAK (pendaftaran_domisili)
            Dim sqlChild As String = "INSERT INTO pendaftaran_domisili " &
            "(id_pendaftaran, alamat, rt_rw, kelurahan_desa, kecamatan, kabupaten_kota, provinsi, kode_pos) " &
            "VALUES " &
            "(@id_pendaftaran, @alamat, @rtrw, @desa, @kecamatan, @kota, @prov, @kode)"

            cmd.CommandText = sqlChild
            cmd.Parameters.Clear() ' Hapus parameter dari query A
            cmd.Parameters.AddWithValue("@id_pendaftaran", newPendaftaranId)
            cmd.Parameters.AddWithValue("@alamat", alamat_lengkap)
            cmd.Parameters.AddWithValue("@rtrw", rt_rw)
            cmd.Parameters.AddWithValue("@desa", desa)
            cmd.Parameters.AddWithValue("@kecamatan", kecamatan)
            cmd.Parameters.AddWithValue("@kota", kota)
            cmd.Parameters.AddWithValue("@prov", provinsi)
            cmd.Parameters.AddWithValue("@kode", kode_pos)
            cmd.ExecuteNonQuery()

            ' LANGKAH D: JIKA SEMUA BERHASIL, COMMIT
            transaction.Commit()
            MessageBox.Show("Selamat! Pendaftaran jalur domisili berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Dim f1 As New Form1()
            f1.Show()
            Me.Close()

        Catch ex As Exception
            ' LANGKAH E: JIKA GAGAL, ROLLBACK
            transaction.Rollback()
            MessageBox.Show("Gagal menyimpan data: " & ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class
