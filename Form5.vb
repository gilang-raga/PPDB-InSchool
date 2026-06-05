Imports MySql.Data.MySqlClient
Imports System.IO ' Diperlukan untuk proses upload file
Public Class Form5
    ' --- Properti Publik ---
    ' Ini akan diisi oleh Form4
    Public JenisPendaftaran As String
    Public JalurPendaftaran As String
    Public IdJurusanPilihan As Integer
    Public NIK As String
    Public AsalSekolah As String
    ' ----------------------
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Atur filter agar hanya bisa memilih file PDF, JPG, PNG
        OpenFileDialog1.Filter = "PDF Files|*.pdf|Image Files|*.jpg;*.jpeg;*.png"
        OpenFileDialog1.Title = "Pilih Sertifikat Pendukung"

        ' Jika file berhasil dipilih
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            txtSertifikat.Text = OpenFileDialog1.FileName
        End If


    End Sub
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Dim f4 As New Form4()
        f4.Show()
        Me.Close()
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        ' 1. Validasi Data Form5
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
        Dim alamat As String = TextBox3.Text.Trim()
        Dim no_kk As String = TextBox4.Text.Trim()
        Dim no_akta As String = TextBox5.Text.Trim()
        Dim pathSertifikat As String = txtSertifikat.Text.Trim()
        Dim nilai_rapot As String = TextBox6.Text.Trim()

        If nama_lengkap = "" Or jenisKelamin = "" Or agama = "" Or tmp_lahir = "" Or alamat = "" Or no_kk = "" Or no_akta = "" Then
            MessageBox.Show("Harap isi semua data terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim nilaiRaportDecimal As Decimal
        If Not Decimal.TryParse(nilai_rapot, nilaiRaportDecimal) Then
            MessageBox.Show("Nilai Rapot harus berupa angka (contoh: 85.50).", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 2. Konfirmasi
        Dim konfirmasi As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menyimpan data pendaftaran ini?", "Konfirmasi Simpan", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If konfirmasi = DialogResult.No Then
            Exit Sub
        End If

        '3. Proses Upload File (Sederhana)
        ' Kita akan copy file ke folder 'uploads' di dalam folder 'bin/Debug'
        Dim namaFileSertifikat As String = ""
        If pathSertifikat <> "" Then
            Try
                Dim folderTujuan As String = Path.Combine(Application.StartupPath, "uploads")
                ' Buat folder jika belum ada
                If Not Directory.Exists(folderTujuan) Then
                    Directory.CreateDirectory(folderTujuan)
                End If

                ' Buat nama file unik (NISN + nama file asli)
                namaFileSertifikat = koneksi.nisnSiswaLogin & "_" & Path.GetFileName(pathSertifikat)
                Dim pathTujuan As String = Path.Combine(folderTujuan, namaFileSertifikat)

                ' Copy file
                File.Copy(pathSertifikat, pathTujuan, True) ' True = overwrite jika ada
            Catch ex As Exception
                MessageBox.Show("Gagal meng-upload sertifikat: " & ex.Message, "Error File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub ' Batalkan simpan jika file gagal di-upload
            End Try
        End If

        ' 4. Proses Transaksi Database
        bukaKoneksi()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        cmd = conn.CreateCommand()
        cmd.Transaction = transaction

        Try
            ' LANGKAH A: INSERT KE TABEL INDUK (pendaftaran)
            Dim sqlParent As String = "INSERT INTO pendaftaran " &
                "(nisn, id_jurusan, jenis_pendaftaran, jalur_pendaftaran, nik, asal_sekolah, " &
                "nama_lengkap, jenis_kelamin, agama, tempat_lahir, tanggal_lahir, no_kk, no_akta_kelahiran) " &
                "VALUES " &
                "(@nisn, @id_jurusan, @jenis, @jalur, @nik, @asal, " &
                "@nama, @jk, @agama, @tmp, @tgl, @kk, @akta)"

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

            ' LANGKAH C: INSERT KE TABEL ANAK (pendaftaran_prestasi)
            Dim sqlChild As String = "INSERT INTO pendaftaran_prestasi " &
                "(id_pendaftaran, alamat, sertifikat_pendukung, nilai_raport) " &
                "VALUES (@id_pendaftaran, @alamat, @sertifikat, @nilai)"

            cmd.CommandText = sqlChild
            cmd.Parameters.Clear() ' Hapus parameter dari query A
            cmd.Parameters.AddWithValue("@id_pendaftaran", newPendaftaranId)
            cmd.Parameters.AddWithValue("@alamat", alamat)
            cmd.Parameters.AddWithValue("@sertifikat", namaFileSertifikat) ' Simpan nama filenya
            cmd.Parameters.AddWithValue("@nilai", nilaiRaportDecimal)
            cmd.ExecuteNonQuery()

            ' LANGKAH D: JIKA SEMUA BERHASIL, COMMIT
            transaction.Commit()
            MessageBox.Show("Selamat! Pendaftaran jalur prestasi berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Dim f1 As New Form1()
            f1.Show()
            Me.Close()
        Catch ex As Exception
            ' LANGKAH E: JIKA GAGAL, ROLLBACK
            transaction.Rollback()
            MessageBox.Show("Gagal menyimpan data ke database. Transaksi dibatalkan. Error: " & ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class