Imports System.IO
Imports System.IO.Path
Imports System.Text

Imports System.Text.RegularExpressions

Public Class Settings


    Public iniFile As String = Combine(GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
        GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) & ".ini")
    Public LOGFILEPATH As String = " "
    Public SettingJsonFilePath As String = Combine(GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
        GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) & ".json")
    Public EncodingString As String = "shift_jis"

    Public lg As dcpflog

    Public TemplateOutPutPath As String = Combine(GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
        GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location) & "_template.json")

    Public TemplateCreatemode As Boolean = False
    Public Sub New()

        Dim cmds As String() = System.Environment.GetCommandLineArgs()
        Dim rPattern As New Regex("/(?<option>[\w\-_]+):(?<value>[\w\W]+)")
        For Each l As String In cmds
            Dim m As Match = rPattern.Match(l)
            Select Case (m.Groups("option").Value).ToUpper
                Case "I"
                    iniFile = m.Groups("value").ToString
                Case "J"
                    SettingJsonFilePath = m.Groups("value").ToString

                Case "L"
                    LOGFILEPATH = m.Groups("value").ToString
                Case "T"
                    If m.Groups("value").ToString.ToLower = "true" Then
                        TemplateCreatemode = True
                    End If

            End Select
        Next

        lg = New dcpflog(LOGFILEPATH)
        SettingRead()

    End Sub

    Public Sub SettingRead()
        Using iniStream As New StreamReader(iniFile)

            Try
                Do While iniStream.Peek <> -1
                    Dim r As String = iniStream.ReadLine

                    Dim rPattern As New Regex("^(?<opt>[\w\-_]+)=(?<val>[\w\W]+)")
                    Dim m As Match = rPattern.Match(r)
                    Select Case (m.Groups("opt").Value).ToUpper
                        Case "ENCODING"
                            EncodingString = m.Groups("val").ToString
                    End Select

                Loop
            Catch E As Exception
                lg.PutLog(E.ToString)
            End Try
        End Using
    End Sub

    Public Function SettingCheck() As Boolean

        SettingCheck = False
        Dim cmds As String() = System.Environment.GetCommandLineArgs()

        '自分自身のAssemblyを取得
        Dim asm As System.Reflection.Assembly =
            System.Reflection.Assembly.GetExecutingAssembly()
        'バージョンの取得
        Dim ver As System.Version = asm.GetName().Version

        lg.PutLog("-- " & Path.GetFileName(asm.Location) & " : " & ver.ToString & " --")
        For Each cmd As String In My.Application.CommandLineArgs
            lg.PutLog("Commandline: " & cmd)
        Next
        lg.PutLog("------------------------------------")

        lg.PutLog("LOGFILEPATH=" & LOGFILEPATH)
        lg.PutLog("iniFilePath=" & iniFile)
        lg.PutLog("EncodingString=" & EncodingString)


        Return True
    End Function
End Class

