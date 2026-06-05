' SALIN SEMUA KODE INI DAN TIMPA SELURUH ISI FormTambahd.vb ANDA

Imports MySql.Data.MySqlClient

Public Class FormTambahd
    ' ================================================
    ' ==    VARIABEL PUBLIK UNTUK MENERIMA DATA     ==
    ' ================================================
    Public nisnSiswa As String
    Public idJurusan As Integer
    Public nik As String
    Public asalSekolah As String
    ' ================================================

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form6.Show()
        Me.Close()
    End Sub

    '--- TOMBOL SIMPAN (PERBAIKAN) ---
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' 1. Ambil data dari Form 6 (YANG SUDAH DIOPER)
        ' HAPUS: Dim nisnSiswa As String = Form6.txtNisnSiswa.Text.Trim()
        ' (Kita sekarang gunakan 'Me.nisnSiswa' dari variabel publik di atas)

        Dim jenis_pendaftaran As String = "Siswa Baru"
        Dim jalur_pendaftaran As String = "Domisili"

        ' 2. Ambil data dari FormTambahd (form ini)
        Dim nama_lengkap As String = TextBox1.Text.Trim()
        Dim jenisKelamin As String = If(RadioButton1.Checked, "Laki-laki", "Perempuan")
        Dim agama As String = ComboBox1.Text
        Dim tmp_lahir As String = TextBox2.Text.Trim()
        Dim tgl_lahir As String = DateTimePicker1.Value.ToString("yyyy-MM-dd")
        Dim no_kk As String = TextBox9.Text.Trim()
        Dim no_akta As String = TextBox10.Text.Trim()
        Dim alamat As String = TextBox3.Text.Trim()
        Dim rt_rw As String = TextBox4.Text.Trim()
        Dim desa As String = TextBox5.Text.Trim()
        Dim kecamatan As String = TextBox6.Text.Trim()
        Dim kota As String = TextBox7.Text.Trim()
        Dim provinsi As String = TextBox8.Text.Trim()
        Dim kode_pos As String = TextBox11.Text.Trim()

        ' 3. Validasi Penting
        ' Pesan "NISN... tidak boleh kosong" yang Anda lihat berasal dari sini,
        ' tapi sekarang kita cek 'Me.nisnSiswa'
        If Me.nisnSiswa = "" Or nama_lengkap = "" Or jenisKelamin = "" Or agama = "" Or tmp_lahir = "" Or alamat = "" Or no_kk = "" Or no_akta = "" Or rt_rw = "" Or desa = "" Or kecamatan = "" Or kota = "" Or provinsi = "" Or kode_pos = "" Then
            MessageBox.Show("Harap isi semua data terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 4. Konfirmasi
        Dim konfirmasi As DialogResult = MessageBox.Show("Simpan data pendaftaran untuk NISN: " & Me.nisnSiswa & "?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If konfirmasi = DialogResult.No Then Exit Sub

        ' 5. Transaksi Database
        bukaKoneksi()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        cmd = conn.CreateCommand()
        cmd.Transaction = transaction

        Try
            ' Cek dulu apakah NISN ada di tabel siswa
            cmd.CommandText = "SELECT COUNT(*) FROM siswa WHERE nisn = @nisn_check"
            cmd.Parameters.AddWithValue("@nisn_check", Me.nisnSiswa) ' Gunakan Me.nisnSiswa
            Dim siswaExists As Long = CLng(cmd.ExecuteScalar())

            If siswaExists = 0 Then
                ' Buat akun siswa baru
                cmd.CommandText = "INSERT INTO siswa (nisn, nama_lengkap, password) VALUES (@nisn, @nama_siswa, @pass)"
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@nisn", Me.nisnSiswa)
                cmd.Parameters.AddWithValue("@nama_siswa", nama_lengkap)
                cmd.Parameters.AddWithValue("@pass", Me.nisnSiswa) ' NISN jadi password default
                cmd.ExecuteNonQuery()
            End If

            ' LANGKAH A: INSERT KE pendaftaran
            Dim sqlParent As String = "INSERT INTO pendaftaran " &
                "(nisn, id_jurusan, jenis_pendaftaran, jalur_pendaftaran, nik, asal_sekolah, " &
                "nama_lengkap, jenis_kelamin, agama, tempat_lahir, tanggal_lahir, no_kk, no_akta_kelahiran) " &
                "VALUES " &
                "(@nisn, @id_jurusan, @jenis, @jalur, @nik, @asal, " &
                "@nama, @jk, @agama, @tmp, @tgl, @kk, @akta)"

            cmd.CommandText = sqlParent
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@nisn", Me.nisnSiswa)
            cmd.Parameters.AddWithValue("@id_jurusan", Me.idJurusan)
            cmd.Parameters.AddWithValue("@jenis", jenis_pendaftaran)
            cmd.Parameters.AddWithValue("@jalur", jalur_pendaftaran)
            cmd.Parameters.AddWithValue("@nik", Me.nik)
            cmd.Parameters.AddWithValue("@asal", Me.asalSekolah)
            cmd.Parameters.AddWithValue("@nama", nama_lengkap)
            cmd.Parameters.AddWithValue("@jk", jenisKelamin)
            cmd.Parameters.AddWithValue("@agama", agama)
            cmd.Parameters.AddWithValue("@tmp", tmp_lahir)
            cmd.Parameters.AddWithValue("@tgl", tgl_lahir)
            cmd.Parameters.AddWithValue("@kk", no_kk)
            cmd.Parameters.AddWithValue("@akta", no_akta)
            cmd.ExecuteNonQuery()

            Dim newPendaftaranId As Long = cmd.LastInsertedId

            ' LANGKAH C: INSERT KE pendaftaran_domisili
            Dim sqlChild As String = "INSERT INTO pendaftaran_domisili " &
                "(id_pendaftaran, alamat, rt_rw, kelurahan_desa, kecamatan, kabupaten_kota, provinsi, kode_pos) " &
                "VALUES " &
                "(@id_pendaftaran, @alamat, @rtrw, @desa, @kecamatan, @kota, @prov, @kode)"

            cmd.CommandText = sqlChild
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@id_pendaftaran", newPendaftaranId)
            cmd.Parameters.AddWithValue("@alamat", alamat)
            cmd.Parameters.AddWithValue("@rtrw", rt_rw)
            cmd.Parameters.AddWithValue("@desa", desa)
            cmd.Parameters.AddWithValue("@kecamatan", kecamatan)
            cmd.Parameters.AddWithValue("@kota", kota)
            cmd.Parameters.AddWithValue("@prov", provinsi)
            cmd.Parameters.AddWithValue("@kode", kode_pos)
            cmd.ExecuteNonQuery()

            transaction.Commit()
            MessageBox.Show("Data siswa jalur domisili berhasil ditambah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.Close()
            Form11.Show()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Gagal menyimpan data: " & ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub GroupBox2_Enter(sender As Object, e As EventArgs) Handles GroupBox2.Enter
    End Sub
    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter
    End Sub
End Class