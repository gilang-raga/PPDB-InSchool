Imports MySql.Data.MySqlClient

Public Class Form4
    Private bsJurusan As New BindingSource()
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MuatDataJurusan()
    End Sub

    Private Sub MuatDataJurusan()
        Try
            bukaKoneksi()
            Dim query As String = "SELECT id_jurusan, nama_jurusan FROM jurusan ORDER BY nama_jurusan"
            da = New MySqlDataAdapter(query, conn)
            dt = New DataTable()
            da.Fill(dt)

            ' Atur BindingSource
            bsJurusan.DataSource = dt

            ' Atur ComboBox3
            ComboBox3.DataSource = bsJurusan
            ComboBox3.DisplayMember = "nama_jurusan" ' Yang dilihat siswa
            ComboBox3.ValueMember = "id_jurusan"    ' Yang kita simpan (ID-nya)
            ComboBox3.SelectedIndex = -1 ' Kosongkan pilihan awal
        Catch ex As Exception
            MessageBox.Show("Gagal memuat data jurusan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        ' Validasi
        If ComboBox1.Text = "" Or ComboBox2.Text = "" Or ComboBox3.SelectedIndex = -1 Or TextBox1.Text.Trim() = "" Or TextBox2.Text.Trim() = "" Then
            MessageBox.Show("Harap isi semua data terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Ambil data dari form
        Dim jenisPendaftaran As String = ComboBox1.Text
        Dim jalurPendaftaran As String = ComboBox2.Text
        Dim idJurusanPilihan As Integer = CInt(ComboBox3.SelectedValue) ' Ambil ID Jurusan
        Dim namaJurusanPilihan As String = ComboBox3.Text ' Ambil Nama Jurusan
        Dim nik As String = TextBox1.Text.Trim()
        Dim asalSekolah As String = TextBox2.Text.Trim()

        MessageBox.Show("Data Awal OK. Lanjut ke form berikutnya.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

        If jalurPendaftaran.ToLower() = "prestasi" Then
            Dim f5 As New Form5()
            ' Isi properti publik di Form5 (ini akan kita buat di langkah 3)
            f5.JenisPendaftaran = jenisPendaftaran
            f5.JalurPendaftaran = jalurPendaftaran
            f5.IdJurusanPilihan = idJurusanPilihan
            f5.NIK = nik
            f5.AsalSekolah = asalSekolah
            f5.Show()
            Me.Hide()
        ElseIf jalurPendaftaran.ToLower() = "domisili" Then
            Dim f8 As New Form8()
            ' Isi properti publik di Form8 (ini akan kita buat di langkah 4)
            f8.JenisPendaftaran = jenisPendaftaran
            f8.JalurPendaftaran = jalurPendaftaran
            f8.IdJurusanPilihan = idJurusanPilihan
            f8.NIK = nik
            f8.AsalSekolah = asalSekolah
            f8.Show()
            Me.Hide()
        Else
            MessageBox.Show("Jalur pendaftaran tidak dikenali. Silakan pilih Prestasi atau Domisili.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
End Class