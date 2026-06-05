' SALIN SEMUA KODE INI DAN TIMPA SELURUH ISI Form10.vb ANDA (PERBAIKAN BUG)

Imports MySql.Data.MySqlClient
Imports System.IO

Public Class Form10
    Public IdPendaftaranUntukDiubah As Integer

    ' Variabel untuk menyimpan data asli
    Private _jalurPendaftaranAsli As String = ""
    Private _nisnAsli As String = ""
    Private _sertifikatAsli As String = ""

    Private Sub Form10_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MuatDataUntukDiubah()
    End Sub

    Private Sub MuatDataUntukDiubah()
        If IdPendaftaranUntukDiubah <= 0 Then
            MessageBox.Show("ID Pendaftaran tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
            Exit Sub
        End If

        Try
            bukaKoneksi()
            ' LANGKAH A: Ambil data UMUM
            Dim queryParent As String = "SELECT * FROM pendaftaran WHERE id_pendaftaran = @id"
            cmd = New MySqlCommand(queryParent, conn)
            cmd.Parameters.AddWithValue("@id", IdPendaftaranUntukDiubah)
            rd = cmd.ExecuteReader()

            If rd.HasRows Then
                rd.Read()
                _jalurPendaftaranAsli = rd.GetString("jalur_pendaftaran")
                _nisnAsli = rd.GetString("nisn")
                TextBox8.Text = rd.GetString("nama_lengkap")
                ComboBox1.Text = rd.GetString("agama")
                TextBox7.Text = rd.GetString("tempat_lahir")
                DateTimePicker1.Value = rd.GetDateTime("tanggal_lahir")
                TextBox9.Text = rd.GetString("no_kk")
                TextBox10.Text = rd.GetString("no_akta_kelahiran")
                If rd.GetString("jenis_kelamin") = "Laki-laki" Then
                    RadioButton1.Checked = True
                Else
                    RadioButton2.Checked = True
                End If
                rd.Close()

                ' LANGKAH B: Cek Jalur dan ambil data SPESIFIK
                If _jalurPendaftaranAsli.ToLower() = "prestasi" Then
                    NonaktifkanFieldDomisili(True)
                    cmd.CommandText = "SELECT * FROM pendaftaran_prestasi WHERE id_pendaftaran = @id"
                    rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        TextBox17.Text = rd.GetString("alamat")
                        TextBox19.Text = rd.GetDecimal("nilai_raport").ToString()

                        ' =========================================================
                        ' ==    INI ADALAH BARIS KODE YANG DIPERBAIKI            ==
                        ' =========================================================
                        _sertifikatAsli = If(rd("sertifikat_pendukung") Is DBNull.Value, "", rd.GetString("sertifikat_pendukung"))
                        ' =========================================================

                        TextBox18.Text = _sertifikatAsli
                    End If
                    rd.Close()
                ElseIf _jalurPendaftaranAsli.ToLower() = "domisili" Then
                    NonaktifkanFieldPrestasi(True)
                    cmd.CommandText = "SELECT * FROM pendaftaran_domisili WHERE id_pendaftaran = @id"
                    rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        TextBox17.Text = rd.GetString("alamat")
                        TextBox16.Text = rd.GetString("rt_rw")
                        TextBox15.Text = rd.GetString("kelurahan_desa")
                        TextBox14.Text = rd.GetString("kecamatan")
                        TextBox13.Text = rd.GetString("kabupaten_kota")
                        TextBox12.Text = rd.GetString("provinsi")
                        TextBox11.Text = rd.GetString("kode_pos")
                    End If
                    rd.Close()
                End If
            Else
                rd.Close()
                MessageBox.Show("Data pendaftaran tidak ditemukan.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("Gagal memuat data: " & ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then
                rd.Close()
            End If
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    '--- Helper untuk Nonaktifkan Field ---
    Private Sub NonaktifkanFieldDomisili(ByVal nonaktif As Boolean)
        TextBox16.Enabled = Not nonaktif ' RT/RW
        TextBox15.Enabled = Not nonaktif ' Desa
        TextBox14.Enabled = Not nonaktif ' Kecamatan
        TextBox13.Enabled = Not nonaktif ' Kota
        TextBox12.Enabled = Not nonaktif ' Provinsi
        TextBox11.Enabled = Not nonaktif ' Kode Pos
        If nonaktif Then
            TextBox16.Text = "N/A"
            TextBox15.Text = "N/A"
            TextBox14.Text = "N/A"
            TextBox13.Text = "N/A"
            TextBox12.Text = "N/A"
            TextBox11.Text = "N/A"
        End If
    End Sub

    Private Sub NonaktifkanFieldPrestasi(ByVal nonaktif As Boolean)
        TextBox19.Enabled = Not nonaktif ' Nilai Raport
        TextBox18.Enabled = Not nonaktif ' Sertifikat
        Button6.Enabled = Not nonaktif ' Tombol Upload
        If nonaktif Then
            TextBox19.Text = "0"
            TextBox18.Text = "N/A"
        End If
    End Sub
    '--- KODE TOMBOL LAINNYA ---
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        OpenFileDialog1.Filter = "PDF Files|*.pdf|Image Files|*.jpg;*.jpeg;*.png"
        OpenFileDialog1.Title = "Pilih Sertifikat Pendukung"
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            TextBox18.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub KembaliKeFormList()
        If _jalurPendaftaranAsli.ToLower() = "prestasi" Then
            Form9.Show()
        Else
            Form11.Show()
        End If
    End Sub

    Private Sub GroupBox3_Enter(sender As Object, e As EventArgs) Handles GroupBox3.Enter
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        KembaliKeFormList()
        Me.Close()
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        ' (Kode ini sama seperti sebelumnya, tidak perlu diubah)
        Dim nama_lengkap As String = TextBox8.Text
        Dim jenis_kelamin As String = If(RadioButton1.Checked, "Laki-laki", "Perempuan")
        Dim agama As String = ComboBox1.Text
        Dim tmp_lahir As String = TextBox7.Text
        Dim tgl_lahir As String = DateTimePicker1.Value.ToString("yyyy-MM-dd")
        Dim no_kk As String = TextBox9.Text
        Dim no_akta As String = TextBox10.Text
        Dim alamat As String = TextBox17.Text

        Dim konfirmasi As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menyimpan perubahan data ini?", "Konfirmasi Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If konfirmasi = DialogResult.No Then Exit Sub

        bukaKoneksi()
        Dim transaction As MySqlTransaction = conn.BeginTransaction()
        cmd = conn.CreateCommand()
        cmd.Transaction = transaction

        Try
            ' LANGKAH A: UPDATE TABEL INDUK
            Dim sqlParent As String = "UPDATE pendaftaran SET " &
                "nama_lengkap = @nama, jenis_kelamin = @jk, agama = @agama, " &
                "tempat_lahir = @tmp, tanggal_lahir = @tgl, " &
                "no_kk = @kk, no_akta_kelahiran = @akta " &
                "WHERE id_pendaftaran = @id"

            cmd.CommandText = sqlParent
            cmd.Parameters.AddWithValue("@nama", nama_lengkap)
            cmd.Parameters.AddWithValue("@jk", jenis_kelamin)
            cmd.Parameters.AddWithValue("@agama", agama)
            cmd.Parameters.AddWithValue("@tmp", tmp_lahir)
            cmd.Parameters.AddWithValue("@tgl", tgl_lahir)
            cmd.Parameters.AddWithValue("@kk", no_kk)
            cmd.Parameters.AddWithValue("@akta", no_akta)
            cmd.Parameters.AddWithValue("@id", IdPendaftaranUntukDiubah)
            cmd.ExecuteNonQuery()

            ' LANGKAH B: UPDATE TABEL ANAK (Kondisional)
            If _jalurPendaftaranAsli.ToLower() = "prestasi" Then
                Dim nilaiRaportDecimal As Decimal = 0
                Decimal.TryParse(TextBox19.Text, nilaiRaportDecimal)
                Dim pathSertifikat As String = TextBox18.Text
                Dim namaFileSertifikatFinal As String = _sertifikatAsli

                If pathSertifikat <> _sertifikatAsli AndAlso File.Exists(pathSertifikat) Then
                    Dim folderTujuan As String = Path.Combine(Application.StartupPath, "uploads")
                    namaFileSertifikatFinal = _nisnAsli & "_" & Path.GetFileName(pathSertifikat)
                    Dim pathTujuan As String = Path.Combine(folderTujuan, namaFileSertifikatFinal)
                    File.Copy(pathSertifikat, pathTujuan, True)
                End If

                Dim sqlChild As String = "UPDATE pendaftaran_prestasi SET " &
                    "alamat = @alamat, sertifikat_pendukung = @sertifikat, nilai_raport = @nilai " &
                    "WHERE id_pendaftaran = @id"

                cmd.CommandText = sqlChild
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@alamat", alamat)
                cmd.Parameters.AddWithValue("@sertifikat", namaFileSertifikatFinal)
                cmd.Parameters.AddWithValue("@nilai", nilaiRaportDecimal)
                cmd.Parameters.AddWithValue("@id", IdPendaftaranUntukDiubah)
                cmd.ExecuteNonQuery()

            ElseIf _jalurPendaftaranAsli.ToLower() = "domisili" Then
                Dim rt_rw As String = TextBox16.Text
                Dim desa As String = TextBox15.Text
                Dim kecamatan As String = TextBox14.Text
                Dim kota As String = TextBox13.Text
                Dim provinsi As String = TextBox12.Text
                Dim kode_pos As String = TextBox11.Text

                Dim sqlChild As String = "UPDATE pendaftaran_domisili SET " &
                    "alamat = @alamat, rt_rw = @rtrw, kelurahan_desa = @desa, " &
                    "kecamatan = @kecamatan, kabupaten_kota = @kota, " &
                    "provinsi = @prov, kode_pos = @kode " &
                    "WHERE id_pendaftaran = @id"

                cmd.CommandText = sqlChild
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@alamat", alamat)
                cmd.Parameters.AddWithValue("@rtrw", rt_rw)
                cmd.Parameters.AddWithValue("@desa", desa)
                cmd.Parameters.AddWithValue("@kecamatan", kecamatan)
                cmd.Parameters.AddWithValue("@kota", kota)
                cmd.Parameters.AddWithValue("@prov", provinsi)
                cmd.Parameters.AddWithValue("@kode", kode_pos)
                cmd.Parameters.AddWithValue("@id", IdPendaftaranUntukDiubah)
                cmd.ExecuteNonQuery()
            End If

            transaction.Commit()
            MessageBox.Show("Data siswa berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)

            KembaliKeFormList()
            Me.Close()
        Catch ex As Exception
            transaction.Rollback()
            MessageBox.Show("Gagal memperbarui data: " & ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class