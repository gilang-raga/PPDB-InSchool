' SALIN SEMUA KODE INI DAN TIMPA SELURUH ISI Form6.vb ANDA

Imports MySql.Data.MySqlClient

Public Class Form6
    ' Buat BindingSource untuk ComboBox Jurusan
    Private bsJurusan As New BindingSource()

    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MuatDataJurusan()
    End Sub

    Private Sub MuatDataJurusan()
        Try
            bukaKoneksi()
            Dim query As String = "SELECT id_jurusan, nama_jurusan FROM jurusan ORDER BY nama_jurusan"
            da = New MySqlDataAdapter(query, conn)
            dt = New DataTable()
            da.Fill(dt)
            bsJurusan.DataSource = dt
            ComboBox3.DataSource = bsJurusan
            ComboBox3.DisplayMember = "nama_jurusan"
            ComboBox3.ValueMember = "id_jurusan"
            ComboBox3.SelectedIndex = -1
        Catch ex As Exception
            MessageBox.Show("Gagal memuat data jurusan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Form9.Show()
        Me.Close()
    End Sub
    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        ' 1. Validasi data di Form6
        If txtNisnSiswa.Text.Trim() = "" Then
            MessageBox.Show("Silakan isi NISN Siswa terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        If ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("Silakan pilih Jalur Pendaftaran!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        If ComboBox3.SelectedIndex = -1 Then
            MessageBox.Show("Silakan pilih Jurusan terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 2. Ambil semua data dari Form6 (form ini)
        Dim dataNisn As String = txtNisnSiswa.Text.Trim()
        Dim dataIdJurusan As Integer = CInt(ComboBox3.SelectedValue)
        Dim dataNik As String = TextBox1.Text.Trim() ' Asumsi TextBox1 adalah NIK
        Dim dataAsalSekolah As String = TextBox2.Text.Trim() ' Asumsi TextBox2 adalah Asal Sekolah
        Dim jalur_pendaftaran As String = ComboBox2.Text

        ' 3. Oper data ke form selanjutnya
        If jalur_pendaftaran = "Prestasi" Then
            Dim fTambahP As New FormTambahp()
            ' --- KIRIM DATA ke form baru ---
            fTambahP.nisnSiswa = dataNisn
            fTambahP.idJurusan = dataIdJurusan
            fTambahP.nik = dataNik
            fTambahP.asalSekolah = dataAsalSekolah
            ' Tampilkan form baru
            fTambahP.Show()
            Me.Hide()
        ElseIf jalur_pendaftaran = "Domisili" Then
            Dim fTambahD As New FormTambahd()
            ' --- KIRIM DATA ke form baru ---
            fTambahD.nisnSiswa = dataNisn
            fTambahD.idJurusan = dataIdJurusan
            fTambahD.nik = dataNik
            fTambahD.asalSekolah = dataAsalSekolah
            ' Tampilkan form baru
            fTambahD.Show()
            Me.Hide()
        Else
            MessageBox.Show("Jalur pendaftaran tidak dikenali.", "Peringeatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub
End Class