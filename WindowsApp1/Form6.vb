Imports MySql.Data.MySqlClient

Public Class Form6
    Dim conn As MySqlConnection
    Dim cmd As MySqlCommand
    Dim da As MySqlDataAdapter
    Dim dt As DataTable

    '=== FUNGSI KONEKSI DATABASE ===
    Sub koneksi()
        conn = New MySqlConnection("server=localhost;user id=root;password=;database=db_pendaftaran")
        conn.Open()
    End Sub

    '=== FUNGSI MENAMPILKAN DATA KE DATAGRID ===
    Sub tampilData()
        Try
            koneksi()
            da = New MySqlDataAdapter("SELECT * FROM tabel", conn)
            dt = New DataTable
            da.Fill(dt)
            DataGridView1.DataSource = dt
            conn.Close()
        Catch ex As Exception
            MsgBox("Gagal menampilkan data: " & ex.Message)
        End Try
    End Sub

    '=== FUNGSI UNTUK KOSONGKAN INPUT ===
    Sub kosongkanForm()
        ComboBox1.Text = ""
        ComboBox2.Text = ""
        ComboBox3.Text = ""
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox7.Clear()
        RadioButton1.Checked = False
        RadioButton2.Checked = False
        ComboBox4.Text = ""
        TextBox6.Clear()
        DateTimePicker1.Value = Date.Today
        TextBox5.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
    End Sub

    '=== EVENT LOAD FORM ===
    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tampilData()
    End Sub

    'SIMPAN
    Private Sub btnSimpan_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            koneksi()

            ' Ambil jenis kelamin
            Dim jk As String = ""
            If RadioButton1.Checked Then
                jk = "Laki-laki"
            ElseIf RadioButton2.Checked Then
                jk = "Perempuan"
            End If

            ' Query SQL
            Dim sql As String = "INSERT INTO tabel (jenis_pendaftaran, jalur_pendaftaran, jurusan, nik, asal_sekolah, nama_lengkap, jenis_kelamin, agama, tempat_lahir, tanggal_lahir, alamat, no_kk, no_akta)
                                 VALUES (@jenis, @jalur, @jurusan, @nik, @asal, @nama, @jk, @agama, @tempat, @tgl, @alamat, @kk, @akta)"
            cmd = New MySqlCommand(sql, conn)

            ' Isi parameter dari form
            cmd.Parameters.AddWithValue("@jenis", ComboBox1.Text)
            cmd.Parameters.AddWithValue("@jalur", ComboBox2.Text)
            cmd.Parameters.AddWithValue("@jurusan", ComboBox3.Text)
            cmd.Parameters.AddWithValue("@nik", TextBox1.Text)
            cmd.Parameters.AddWithValue("@asal", TextBox2.Text)
            cmd.Parameters.AddWithValue("@nama", TextBox7.Text)
            cmd.Parameters.AddWithValue("@jk", jk)
            cmd.Parameters.AddWithValue("@agama", ComboBox4.Text)
            cmd.Parameters.AddWithValue("@tempat", TextBox6.Text)
            cmd.Parameters.AddWithValue("@tgl", DateTimePicker1.Value.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@alamat", TextBox5.Text)
            cmd.Parameters.AddWithValue("@kk", TextBox3.Text)
            cmd.Parameters.AddWithValue("@akta", TextBox4.Text)

            cmd.ExecuteNonQuery()
            MsgBox("Data berhasil disimpan!", MsgBoxStyle.Information)

            conn.Close()
            tampilData()
            kosongkanForm()

        Catch ex As Exception
            MsgBox("Gagal menyimpan data: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub
    Public Sub New(jenis As String, jalur As String, jurusan As String, nik As String, asal As String,
               nama As String, kelamin As String, agama As String, tempat As String,
               tanggal As String, alamat As String, kk As String, akte As String)
        InitializeComponent()

        ' Tambah kolom jika belum ada
        If DataGridView1.Columns.Count = 0 Then
            With DataGridView1
                .Columns.Add("jenis_pendaftaran", "Jenis Pendaftaran")
                .Columns.Add("jalur_pendaftaran", "Jalur Pendaftaran")
                .Columns.Add("Jurusan", "Pilih Jurusan")
                .Columns.Add("nik", "NIK")
                .Columns.Add("asal_sekolah", "Asal Sekolah")
                .Columns.Add("nama_lengkap", "Nama Lengkap")
                .Columns.Add("jenis_kelamin", "Jenis Kelamin")
                .Columns.Add("agama", "Agama")
                .Columns.Add("tempat_lahir", "Tempat Lahir")
                .Columns.Add("tanggal_lahir", "Tanggal Lahir")
                .Columns.Add("alamat_jalan", "Alamat Jalan")
                .Columns.Add("no_kk", "No. KK")
                .Columns.Add("no_akta_kelahiran", "No. Akta Kelahiran")
            End With
        End If

        ' Tambah baris data baru
        DataGridView1.Rows.Add(jenis, jalur, jurusan, nik, asal, nama, kelamin, agama, tempat, tanggal, alamat, kk, akte)
    End Sub
End Class
