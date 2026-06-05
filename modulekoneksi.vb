Imports MySql.Data.MySqlClient

Module koneksi
    Public conn As MySqlConnection
    Public cmd As MySqlCommand
    Public rd As MySqlDataReader
    Public da As MySqlDataAdapter
    Public ds As DataSet
    Public dt As DataTable

    Public nisnSiswaLogin As String ' Untuk menyimpan NISN siswa setelah login
    Public Sub bukaKoneksi()
        Try
            conn = New MySqlConnection("server=localhost;user id=root;password=;database=aplikasi_pendaftaran")
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
        Catch ex As Exception
            MessageBox.Show("Gagal terhubung ke database: " & ex.Message)
        End Try
    End Sub
End Module
