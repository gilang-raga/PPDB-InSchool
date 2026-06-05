' SALIN SEMUA KODE INI DAN TIMPA SELURUH ISI FormTambahp.vb ANDA

Imports MySql.Data.MySqlClient
Imports System.IO

Public Class FormTambahp
    ' ================================================
    ' ==    VARIABEL PUBLIK UNTUK MENERIMA DATA     ==
    ' ================================================
    Public nisnSiswa As String
    Public idJurusan As Integer
    Public nik As String
    Public asalSekolah As String
    ' ================================================

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form6.Show()
        Me.Close()
    End Sub

    '--- TOMBOL SIMPAN (PERBAIKAN) ---
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' 1. Ambil data dari Form 6 (YANG SUDAH DIOPER)
        ' HAPUS: Dim nisnSiswa As String = Form6.txtNisnSiswa.Text.Trim()
        ' (Kita sekarang gunakan 'Me.nisnSiswa' dari variabel publik di atas)

        Dim jenis_pendaftaran As String = "Siswa Baru"
        Dim jalur_pendaftaran As String = "Prestasi"

        ' 2. Ambil data dari FormTambahp (form ini)
        Dim nama_lengkap As String = TextBox1.Text
        Dim jenis_kelamin As String = If(RadioButton1.Checked, "Laki-laki", "Perempuan")
        Dim agama As String = ComboBox4.Text
        Dim tmp_lahir As String = TextBox2.Text
        Dim tgl_lahir As String = DateTimePicker1.Value.ToString("yyyy-MM-dd")
        Dim alamat As String = TextBox3.Text
        Dim no_kk As String = TextBox4.Text
        Dim no_akta As String = TextBox5.Text
        Dim pathSertifikat As String = txtSertifikat.Text
        Dim nilai_rapot_text As String = TextBox6.Text

        ' 3. Validasi Penting
        If Me.nisnSiswa = "" Or nama_lengkap = "" Or jenis_kelamin = "" Or agama = "" Or tmp_lahir = "" Or alamat = "" Or no_kk = "" Or no_akta = "" Then
            MessageBox.Show("Harap isi semua data terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim nilaiRaportDecimal As Decimal
        If Not Decimal.TryParse(nilai_rapot_text, nilaiRaportDecimal) Then
            MessageBox.Show("Nilai Rapot harus berupa angka.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 4. Proses Upload File
        Dim namaFileSertifikat As String = ""
        If pathSertifikat <> "" AndAlso File.Exists(pathSertifikat) Then
            Try
                Dim folderTujuan As String = Path.Combine(Application.StartupPath, "uploads")
                If Not Directory.Exists(folderTujuan) Then
                    Directory.CreateDirectory(folderTujuan)
                End If
                namaFileSertifikat = Me.nisnSiswa & "_" & Path.GetFileName(pathSertifikat)
                Dim pathTujuan As String = Path.Combine(folderTujuan, namaFileSertifikat)
                File.Copy(pathSertifikat, pathTujuan, True)
            Catch ex As Exception
                MessageBox.Show("Gagal meng-upload sertifikat: " & ex.Message, "Error File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End Try
        End If

        ' 5. Konfirmasi
        Dim konfirmasi As DialogResult = MessageBox.Show("Simpan data pendaftaran untuk NISN: " & Me.nisnSiswa & "?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If konfirmasi = DialogResult.No Then Exit Sub

        ' 6. Transaksi Database
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
            cmd.Parameters.AddWithValue("@jk", jenis_kelamin)
            cmd.Parameters.AddWithValue("@agama", agama)
            cmd.Parameters.AddWithValue("@tmp", tmp_lahir)
            cmd.Parameters.AddWithValue("@tgl", tgl_lahir)
            cmd.Parameters.AddWithValue("@kk", no_kk)
            cmd.Parameters.AddWithValue("@akta", no_akta)
            cmd.ExecuteNonQuery()

            Dim newPendaftaranId As Long = cmd.LastInsertedId

            ' LANGKAH C: INSERT KE pendaftaran_prestasi
            Dim sqlChild As String = "INSERT INTO pendaftaran_prestasi " &
                "(id_pendaftaran, alamat, sertifikat_pendukung, nilai_raport) " &
                "VALUES (@id_pendaftaran, @alamat, @sertifikat, @nilai)"

            cmd.CommandText = sqlChild
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@id_pendaftaran", newPendaftaranId)
            cmd.Parameters.AddWithValue("@alamat", alamat)
            cmd.Parameters.AddWithValue("@sertifikat", namaFileSertifikat)
            cmd.Parameters.AddWithValue("@nilai", nilaiRaportDecimal)
            cmd.ExecuteNonQuery()

            transaction.Commit()
            MessageBox.Show("Data siswa jalur prestasi berhasil ditambah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.Close()
            Form9.Show()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Gagal menyimpan data: " & ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    '--- Tombol Upload Sertifikat ---
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        OpenFileDialog1.Filter = "PDF Files|*.pdf|Image Files|*.jpg;*.jpeg;*.png"
        OpenFileDialog1.Title = "Pilih Sertifikat Pendukung"
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            txtSertifikat.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
    End Sub
    Private Sub GroupBox2_Enter(sender As Object, e As EventArgs) Handles GroupBox2.Enter
    End Sub
End Class