Imports Microsoft.Reporting.WinForms

Public Class FormLaporan
    ' Properti publik untuk menerima data dari form lain
    Public Property DataTableLaporan As DataTable
    Public Property ReportDataSetName As String
    Public Property ReportFileName As String

    Private Sub FormLaporan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Pastikan ReportViewer kosong dulu
            Me.ReportViewer1.LocalReport.DataSources.Clear()

            ' Pastikan file RDLC ditentukan
            If String.IsNullOrEmpty(ReportFileName) Then
                MessageBox.Show("File laporan (.rdlc) belum ditentukan!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' Tentukan file RDLC yang digunakan (embedded resource)
            Me.ReportViewer1.LocalReport.ReportEmbeddedResource = ReportFileName

            ' Pastikan DataTable tidak kosong
            If DataTableLaporan Is Nothing OrElse DataTableLaporan.Rows.Count = 0 Then
                MessageBox.Show("Tidak ada data untuk ditampilkan pada laporan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            ' Tambahkan sumber data ke laporan
            Dim rds As New ReportDataSource(ReportDataSetName, DataTableLaporan)
            Me.ReportViewer1.LocalReport.DataSources.Add(rds)

            ' Refresh laporan
            Me.ReportViewer1.RefreshReport()

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat memuat laporan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class