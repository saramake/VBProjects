Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Runtime.CompilerServices

Public Class dcpflog

    Public pLogfile As String
    'Public Encoding As System.Text.Encoding = System.Text.Encoding.GetEncoding("Shift_JIS")
    Public Encoding As System.Text.Encoding = System.Text.Encoding.GetEncoding("UTF-8")

    Public Sub New(ByVal logfile As String)
        pLogfile = logfile

    End Sub

    Public Sub SetEncodingString(ByVal enc As String)
        Encoding = System.Text.Encoding.GetEncoding(enc)
    End Sub
    Public Sub PutLog(ByVal Message As String, <CallerMemberName> Optional memberName As String = "")

        Try
            For i = 0 To 300
                File.AppendAllText(pLogfile, Format(Now, "yyyy/MM/dd HH:mm:ss.fff") & "      " & memberName & "() " & Message & vbCrLf, Encoding)
                Return
            Next
        Catch ex As Exception

        End Try

    End Sub
End Class

