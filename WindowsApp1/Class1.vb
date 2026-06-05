Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Module ModuleKoneksi
    Public conn As MySqlConnection
    Public cmd As MySqlCommand
    Public da As MySqlDataAdapter
    Public ds As DataSet
    Public dr As MySqlDataReader

    Public Sub bukaKoneksi()
        Try
            conn = New MySqlConnection("server=localhost;user id=root;password=;database=db_pendaftaran")
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
        Catch ex As Exception
            MsgBox("Koneksi database gagal: " & ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub
End Module
