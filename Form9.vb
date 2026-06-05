Imports MySql.Data.MySqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.IO
Imports System.Diagnostics

Public Class Form9
    Private Sub Form9_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' Panggil TampilkanData dari sini
        TampilkanDataPrestasi()
    End Sub
    Private Sub TampilkanDataPrestasi()
        Try
            bukaKoneksi()

            Dim query As String = "SELECT " &
            "p.id_pendaftaran, p.nisn, p.nama_lengkap, " &
            "j.nama_jurusan AS pilih_jurusan, " & ' <-- DIPERBAIKI
            "p.jenis_pendaftaran, p.jalur_pendaftaran, p.nik, p.asal_sekolah, " &
            "p.jenis_kelamin, p.agama, p.tempat_lahir, p.tanggal_lahir, p.no_kk, " &
            "p.no_akta_kelahiran AS no_akta, " & ' <-- DIPERBAIKI
            "pp.alamat, pp.sertifikat_pendukung, pp.nilai_raport " &
            "FROM pendaftaran p " &
            "JOIN jurusan j ON p.id_jurusan = j.id_jurusan " &
            "JOIN pendaftaran_prestasi pp ON p.id_pendaftaran = pp.id_pendaftaran " &
            "ORDER BY p.nama_lengkap"
            ' --- Akhir Kueri ---

            da = New MySqlDataAdapter(query, conn)
            dt = New DataTable()
            da.Fill(dt)
            DataGridView1.DataSource = dt

            ' (Sisa kode untuk atur header... biarkan saja)
            DataGridView1.Columns("id_pendaftaran").Visible = False
            DataGridView1.Columns("nisn").HeaderText = "NISN"
            DataGridView1.Columns("nama_lengkap").HeaderText = "Nama Siswa"
            DataGridView1.Columns("pilih_jurusan").HeaderText = "Jurusan"
            DataGridView1.Columns("jenis_pendaftaran").HeaderText = "Jenis Pendaftaran"
            DataGridView1.Columns("jalur_pendaftaran").HeaderText = "Jalur Pendaftaran"
            DataGridView1.Columns("nik").HeaderText = "NIK"
            DataGridView1.Columns("asal_sekolah").HeaderText = "Asal Sekolah"
            DataGridView1.Columns("jenis_kelamin").HeaderText = "Jenis Kelamin"
            DataGridView1.Columns("agama").HeaderText = "Agama"
            DataGridView1.Columns("tempat_lahir").HeaderText = "Tempat Lahir"
            DataGridView1.Columns("tanggal_lahir").HeaderText = "Tanggal Lahir"
            DataGridView1.Columns("no_kk").HeaderText = "No. Kartu Keluarga"
            DataGridView1.Columns("no_akta").HeaderText = "No. Akta Kelahiran"
            DataGridView1.Columns("alamat").HeaderText = "Alamat"
            DataGridView1.Columns("sertifikat_pendukung").HeaderText = "Sertifikat Pendukung"
            DataGridView1.Columns("nilai_raport").HeaderText = "Nilai Raport"

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data siswa prestasi: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    '=====================================================
    ' == 2. SEMUA EVENT HANDLER (TOMBOL, LOAD, DLL) ==
    '=====================================================

    Private Sub Form9_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TampilkanDataPrestasi()
    End Sub

    '--- Tombol Tambah (Button1) ---
    Private Sub Button1Tambah_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' (Kode Anda untuk pindah ke Form6 sudah benar)
        Form6.Show()
        Me.Close()
    End Sub

    '--- Tombol Ubah (Button2) ---
    Private Sub Button2Ubah_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Silakan pilih data siswa yang ingin diubah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Ambil ID pendaftaran dari baris yang dipilih
        Dim id_pendaftaran_pilihan As Integer = CInt(DataGridView1.CurrentRow.Cells("id_pendaftaran").Value)

        ' Panggil Form10 (Form Ubah Data) dan kirim HANYA ID-nya
        Dim formUbah As New Form10()
        formUbah.IdPendaftaranUntukDiubah = id_pendaftaran_pilihan
        formUbah.Show()
        Me.Hide()
    End Sub

    '--- Tombol Cetak Laporan (Button3) ---
    ' Kode ini masih membaca dari DataGridView, ini tidak masalah untuk sekarang
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' 1. Ambil DataTable yang SUDAH terikat ke DataGridView
        Dim dt As DataTable = CType(DataGridView1.DataSource, DataTable)

        ' 2. Cek apakah ada datanya
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            MessageBox.Show("Tidak ada data untuk dicetak.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 3. Kirim DataTable yang benar ini ke FormLaporan
        Dim frm As New FormLaporan()
        frm.DataTableLaporan = dt

        ' Pastikan nama ini SAMA PERSIS dengan nama DataSet di file RDLC Anda
        frm.ReportDataSetName = "DataSiswaPrestasi"

        ' Pastikan nama file ini SAMA PERSIS dengan nama file RDLC Anda
        frm.ReportFileName = "WindowsApp1.LaporanDataSiswa.rdlc" ' Ganti 'WindowsApp1'

        frm.ShowDialog()
    End Sub

    '--- Tombol Cari (Button4) ---
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim keyword As String = TextBox8.Text.Trim()
        If keyword = "" Then
            TampilkanDataPrestasi()
            Exit Sub
        End If
        Try
            bukaKoneksi()

            ' --- Kueri yang Diperbaiki dengan 'AS' ---
            Dim query As String = "SELECT " &
            "p.id_pendaftaran, p.nisn, p.nama_lengkap, " &
            "j.nama_jurusan AS pilih_jurusan, " & ' <-- DIPERBAIKI
            "p.jenis_pendaftaran, p.jalur_pendaftaran, p.nik, p.asal_sekolah, " &
            "p.jenis_kelamin, p.agama, p.tempat_lahir, p.tanggal_lahir, p.no_kk, " &
            "p.no_akta_kelahiran AS no_akta, " & ' <-- DIPERBAIKI
            "pp.alamat, pp.sertifikat_pendukung, pp.nilai_raport " &
            "FROM pendaftaran p " &
            "JOIN jurusan j ON p.id_jurusan = j.id_jurusan " &
            "JOIN pendaftaran_prestasi pp ON p.id_pendaftaran = pp.id_pendaftaran " &
            "WHERE p.nama_lengkap LIKE @keyword OR p.nisn LIKE @keyword OR p.asal_sekolah LIKE @keyword OR p.nik LIKE @keyword " &
            "ORDER BY p.nama_lengkap"
            ' --- Akhir Kueri ---

            cmd = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@keyword", "%" & keyword & "%")
            da = New MySqlDataAdapter(cmd)
            dt = New DataTable()
            da.Fill(dt)
            DataGridView1.DataSource = dt
            If dt.Rows.Count = 0 Then
                MessageBox.Show("Data tidak ditemukan!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("Gagal mencari data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    '--- Tombol Hapus (Button5) ---
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Silakan pilih data siswa yang ingin dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Amjbil KUNCI UTAMA (id_pendaftaran) dan nama
        Dim id As Integer = CInt(DataGridView1.CurrentRow.Cells("id_pendaftaran").Value)
        Dim nama As String = DataGridView1.CurrentRow.Cells("nama_lengkap").Value.ToString()

        Dim result As DialogResult = MessageBox.Show("Apakah kamu yakin ingin menghapus data: " & nama & " (ID: " & id & ")?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.No Then
            Exit Sub
        End If

        ' Eksekusi Hapus
        Try
            bukaKoneksi()
            ' Kita HANYA perlu hapus dari tabel 'pendaftaran'.
            ' Sisanya akan terhapus otomatis berkat 'ON DELETE CASCADE'
            Dim query As String = "DELETE FROM pendaftaran WHERE id_pendaftaran = @id"
            cmd = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
            conn.Close()

            MessageBox.Show("Data siswa berhasil dihapus!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Muat ulang data
            TampilkanDataPrestasi()
        Catch ex As Exception
            MessageBox.Show("Gagal menghapus data: " & ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    '--- Tombol Pindah ke Form11 (Button6) ---
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ' (Kode Anda untuk pindah ke Form11 sudah benar)
        Form11.Show()
        Me.Close()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        ' Biarkan kosong
    End Sub

    Private Sub ButtonLihatSertifikat_Click(sender As Object, e As EventArgs) Handles ButtonLihatSertifikat.Click
        ' LANGKAH 1: Cek apakah ada baris yang dipilih
        ' Ini akan memperbaiki error 'NullReferenceException' yang Anda tunjukkan
        ' di image_3ce75c.jpg, yang terjadi karena CurrentRow kosong (Nothing)
        If DataGridView1.CurrentRow Is Nothing Then
            MessageBox.Show("Silakan pilih baris data siswa terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' LANGKAH 2: Ambil nama file dari sel DataGridView
        Dim namaFile As String = ""
        Try
            ' Ambil nilai dari kolom "sertifikat_pendukung"
            ' Kita ambil sebagai Object dulu untuk mengecek NULL
            Dim cellValue As Object = DataGridView1.CurrentRow.Cells("sertifikat_pendukung").Value

            ' Cek apakah nilainya kosong (DBNull)
            If cellValue IsNot DBNull.Value AndAlso cellValue IsNot Nothing Then
                namaFile = cellValue.ToString()
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal membaca nama file dari DataGridView. Pastikan kolom 'sertifikat_pendukung' ada dalam query Anda." & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try


        ' LANGKAH 3: Cek apakah nama filenya valid
        If String.IsNullOrEmpty(namaFile) Then
            MessageBox.Show("Siswa ini tidak memiliki sertifikat pendukung.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' LANGKAH 4: Gabungkan path dan buka file
        Try
            ' Tentukan lokasi folder 'uploads' (relatif terhadap aplikasi)
            Dim folderUploads As String = Path.Combine(Application.StartupPath, "uploads")
            ' Gabungkan path folder dengan nama file
            Dim pathLengkap As String = Path.Combine(folderUploads, namaFile)

            ' Cek apakah file itu benar-benar ada di folder 'uploads'
            If File.Exists(pathLengkap) Then

                ' --- KODE LAMA (HAPUS ATAU KOMENTARI BARIS INI) ---
                ' Process.Start(pathLengkap) 

                ' --- KODE BARU (PAKAI INI) ---
                ' Kita buat settingan proses yang lebih detail
                Dim psInfo As New System.Diagnostics.ProcessStartInfo()
                psInfo.FileName = pathLengkap
                psInfo.UseShellExecute = True  ' <--- INI KUNCINYA!

                ' Jalankan proses dengan settingan di atas
                System.Diagnostics.Process.Start(psInfo)

            Else
                MessageBox.Show("File tidak ditemukan di folder 'uploads'." & vbCrLf & "Path: " & pathLengkap, "File Hilang", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception
            MessageBox.Show("Gagal membuka file: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class