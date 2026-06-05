Imports MySql.Data.MySqlClient

Public Class Form11
    Private Sub Form11_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' Panggil TampilData dari sini
        TampilDataSiswa()
    End Sub
    '=== Saat Form Dibuka ===
    Private Sub Form11_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TampilDataSiswa()
    End Sub

    '=== Menampilkan data dari tabel siswa_domisili ===
    Private Sub TampilDataSiswa()
        Try
            bukaKoneksi()

            ' --- Kueri yang Diperbaiki dengan 'AS' ---
            Dim query As String = "SELECT " &
            "p.id_pendaftaran, p.nisn, p.nama_lengkap, " &
            "j.nama_jurusan AS pilihan_jurusan, " & ' <-- DIPERBAIKI
            "p.jenis_pendaftaran, p.jalur_pendaftaran, p.nik, p.asal_sekolah, " &
            "p.jenis_kelamin, p.agama, p.tempat_lahir, p.tanggal_lahir, p.no_kk, " &
            "p.no_akta_kelahiran AS no_akta, " & ' <-- DIPERBAIKI
            "pd.alamat, pd.rt_rw, pd.kelurahan_desa, pd.kecamatan, pd.kabupaten_kota, pd.provinsi, pd.kode_pos " &
            "FROM pendaftaran p " &
            "JOIN jurusan j ON p.id_jurusan = j.id_jurusan " &
            "JOIN pendaftaran_domisili pd ON p.id_pendaftaran = pd.id_pendaftaran " &
            "ORDER BY p.nama_lengkap"
            ' --- Akhir Kueri ---

            da = New MySqlDataAdapter(query, conn)
            dt = New Data.DataTable()
            da.Fill(dt)
            DataGridView2.DataSource = dt

            ' (Sisa kode untuk atur header... biarkan saja)
            DataGridView2.Columns("id_pendaftaran").Visible = False
            DataGridView2.Columns("nisn").HeaderText = "NISN"
            DataGridView2.Columns("nama_lengkap").HeaderText = "Nama Siswa"
            DataGridView2.Columns("pilihan_jurusan").HeaderText = "Jurusan"
            DataGridView2.Columns("jenis_pendaftaran").HeaderText = "Jenis Pendaftaran"
            DataGridView2.Columns("jalur_pendaftaran").HeaderText = "Jalur Pendaftaran"
            DataGridView2.Columns("nik").HeaderText = "NIK"
            DataGridView2.Columns("asal_sekolah").HeaderText = "Asal Sekolah"
            DataGridView2.Columns("jenis_kelamin").HeaderText = "Jenis Kelamin"
            DataGridView2.Columns("agama").HeaderText = "Agama"
            DataGridView2.Columns("tempat_lahir").HeaderText = "Tempat Lahir"
            DataGridView2.Columns("tanggal_lahir").HeaderText = "Tanggal Lahir"
            DataGridView2.Columns("no_kk").HeaderText = "No. Kartu Keluarga"
            DataGridView2.Columns("no_akta").HeaderText = "No. Akta Kelahiran"
            DataGridView2.Columns("alamat").HeaderText = "Alamat"
            DataGridView2.Columns("rt_rw").HeaderText = "Rt/Rw"
            DataGridView2.Columns("kelurahan_desa").HeaderText = "Kelurahann/Desa"
            DataGridView2.Columns("kecamatan").HeaderText = "Kecamatan"
            DataGridView2.Columns("kabupaten_kota").HeaderText = "Kabupaten/Kota"
            DataGridView2.Columns("provinsi").HeaderText = "Provinsi"
            DataGridView2.Columns("kode_pos").HeaderText = "Kode Pos"

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data siswa domisili: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    '=== Tombol Hapus ===
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If DataGridView2.CurrentRow Is Nothing Then
            MessageBox.Show("Pilih data yang akan dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Ambil KUNCI UTAMA (id_pendaftaran) dan nama
        Dim id As Integer = CInt(DataGridView2.CurrentRow.Cells("id_pendaftaran").Value)
        Dim nama As String = DataGridView2.CurrentRow.Cells("nama_lengkap").Value.ToString()

        Dim result As DialogResult = MessageBox.Show("Hapus data: " & nama & " (ID: " & id & ")?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.No Then Exit Sub

        Try
            bukaKoneksi()
            ' Hapus dari tabel 'pendaftaran', sisanya akan ikut terhapus
            Dim query As String = "DELETE FROM pendaftaran WHERE id_pendaftaran = @id"
            cmd = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@id", id)
            cmd.ExecuteNonQuery()
            conn.Close()

            MessageBox.Show("Data berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
            TampilDataSiswa() ' Muat ulang data
        Catch ex As Exception
            MessageBox.Show("Gagal menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' 1. Ambil DataTable yang SUDAH terikat ke DataGridView
        Dim dt As DataTable = CType(DataGridView2.DataSource, DataTable)

        ' 2. Cek apakah ada datanya
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            MessageBox.Show("Tidak ada data untuk dicetak.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 3. Kirim DataTable yang benar ini ke FormLaporan
        Dim frm As New FormLaporan()
        frm.DataTableLaporan = dt

        ' Pastikan nama ini SAMA PERSIS dengan nama DataSet di file RDLC Anda
        frm.ReportDataSetName = "DataSiswaDomisili"

        ' Pastikan nama file ini SAMA PERSIS dengan nama file RDLC Anda
        frm.ReportFileName = "WindowsApp1.LaporanSiswaDomisili.rdlc" ' Ganti 'WindowsApp1' jika nama proyek Anda beda

        frm.ShowDialog()
    End Sub

    '=== Tombol Cari ===
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim keyword As String = TextBox8.Text.Trim()
        If keyword = "" Then
            TampilDataSiswa()
            Exit Sub
        End If
        Try
            bukaKoneksi()

            ' --- Kueri yang Diperbaiki dengan 'AS' ---
            Dim query As String = "SELECT " &
            "p.id_pendaftaran, p.nisn, p.nama_lengkap, " &
            "j.nama_jurusan AS pilihan_jurusan, " & ' <-- DIPERBAIKI
            "p.jenis_pendaftaran, p.jalur_pendaftaran, p.nik, p.asal_sekolah, " &
            "p.jenis_kelamin, p.agama, p.tempat_lahir, p.tanggal_lahir, p.no_kk, " &
            "p.no_akta_kelahiran AS no_akta, " & ' <-- DIPERBAIKI
            "pd.alamat, pd.rt_rw, pd.kelurahan_desa, pd.kecamatan, pd.kabupaten_kota, pd.provinsi, pd.kode_pos " &
            "FROM pendaftaran p " &
            "JOIN jurusan j ON p.id_jurusan = j.id_jurusan " &
            "JOIN pendaftaran_domisili pd ON p.id_pendaftaran = pd.id_pendaftaran " &
            "WHERE p.nama_lengkap LIKE @keyword OR p.nisn LIKE @keyword OR p.asal_sekolah LIKE @keyword OR pd.kabupaten_kota LIKE @keyword " &
            "ORDER BY p.nama_lengkap"
            ' --- Akhir Kueri ---

            cmd = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@keyword", "%" & keyword & "%")
            da = New MySqlDataAdapter(cmd)
            dt = New Data.DataTable()
            da.Fill(dt)
            DataGridView2.DataSource = dt
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

    '=== Tombol Kembali ke Form9 ===
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Form9.Show()
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If DataGridView2.CurrentRow Is Nothing Then
            MessageBox.Show("Silakan pilih data siswa yang ingin diubah!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Ambil ID pendaftaran dari baris yang dipilih
        Dim id_pendaftaran_pilihan As Integer = CInt(DataGridView2.CurrentRow.Cells("id_pendaftaran").Value)

        ' Panggil Form10 (Form Ubah Data) dan kirim HANYA ID-nya
        Dim formUbah As New Form10()
        formUbah.IdPendaftaranUntukDiubah = id_pendaftaran_pilihan
        formUbah.Show()
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Tombol ini seharusnya membuka Form6 (Form Tambah Data Admin)
        Dim formTambah As New Form6()
        formTambah.Show()
        Me.Hide()
    End Sub
End Class
