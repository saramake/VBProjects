Imports Newtonsoft.Json
Imports System.IO
Imports System.Text.RegularExpressions


Public Class regexcheck

    ' シリアライズするクラスの定義
    Public Class Target
        Public Property Label As String
        Public Property Folder As String
        Public Property File As String

        '条件に合った場合返すエラーレベル
        Public Property ErrorLevel As Integer

        'ファイル内正規表現検索を行うかのフラグ
        'falseの場合
        ' Folder Fileの条件似合うファイルがあった場合エラーレベルを返す
        'trueの場合正規表現による検索を行う
        Public Property RegexSearchFlag As Boolean
        '検索する正規表現
        Public Property RegexPattern As String
        '検索して見つかったらエラーにするか/見つからなかったらエラーにするか
        Public Property ErrorOnFoundFlag As Boolean

        'メッセージファイルを出力するかのフラグ
        Public Property OutputFlag As Boolean

        'メッセージ置き換え用の文言切り出しようの正規表現／ラベル
        Public Property LabelRegexes As LabelRegex()
        Public Class LabelRegex
            Public Folder As String
            Public File As String
            Public LabelRegexPatten As String
            Public LabelName As String
        End Class

        '{{@ラベル名}}を切り出した文言に置き換えて出力
        Public Property OutputMessage As String
        '出力先
        Public Property OutputFile As String


        <JsonIgnore> Public SearchFiles As String() = Nothing
        <JsonIgnore> Public ResultErrorLevel As Integer = 0
        <JsonIgnore> Private log As dcpflog

        Public Sub patterncheck(_log As dcpflog)
            log = _log
            SearchFiles = SearchFolder(Folder, File)
            If RegexSearchFlag = True Then
                For Each f As String In SearchFiles
                    If ErrorOnFoundFlag = True And ErrorLevel = 0 AndAlso CheckPatternMatch(RegexPattern, f) = True Then
                        ResultErrorLevel = ErrorLevel
                        Exit For
                    ElseIf ErrorOnFoundFlag = False And ErrorLevel = 0 AndAlso CheckPatternMatch(RegexPattern, f) = False Then
                        ResultErrorLevel = ErrorLevel
                    End If
                Next
            Else
                If Not SearchFiles Is Nothing Then
                    ResultErrorLevel = ErrorLevel
                End If
            End If


            Dim tmpOutputString As String = OutputMessage
            For Each r As LabelRegex In LabelRegexes
                Dim datalist As List(Of String) = ProcessFiles(r.File, r.Folder, r.LabelRegexPatten)
                tmpOutputString = tmpOutputString.Replace("@{" & r.LabelName & "}", String.Join(",", datalist))
                tmpOutputString = tmpOutputString.Replace("@@{" & r.LabelName & "}", String.Join(vbCrLf, datalist))
            Next
            IO.File.WriteAllText(Environment.GetEnvironmentVariable(OutputFile), tmpOutputString)

        End Sub



        Private Function SearchFolder(baseFolderwithEnv As String, SearchFileNamewithEnv As String) As String()
            ' 環境変数WORKFOLDERを取得
            Dim baseFolder As String = Environment.GetEnvironmentVariable(baseFolderwithEnv)
            Dim searchFileName As String = Environment.GetEnvironmentVariable(SearchFileNamewithEnv)
            Try
                If baseFolder Is Nothing Then
                    log.PutLog("%WORKFOLDER% 環境変数が見つかりません。")
                    Return Nothing
                End If

                ' ディレクトリが存在するか確認
                If Not Directory.Exists(baseFolder) Then
                    log.PutLog("指定されたパスが存在しません: " & baseFolder)
                    Return Nothing
                End If

                ' "123_*"に一致するすべてのファイルを検索（サブディレクトリも含む）
                Dim files As String() = Directory.GetFiles(baseFolder, searchFileName, SearchOption.AllDirectories)
                For Each file As String In files
                    log.PutLog("見つかったファイル: " & file)
                Next

                ' 一致するファイルがない場合
                If files.Length = 0 Then
                    log.PutLog("一致するファイルが見つかりませんでした。")
                End If
                Return files
            Catch e As Exception
                log.PutLog("catch a exception" & e.ToString)
                Return Nothing
            End Try

        End Function

        Private Function CheckPatternMatch(ByVal pattern As String, ByVal filePath As String) As Boolean
            Try
                ' ファイルの内容を読み込む
                Dim fileContent As String = IO.File.ReadAllText(filePath)

                ' Regexオブジェクトを作成
                Dim regex As New Regex(pattern)

                ' ファイル内容で正規表現と一致する対象をすべて検索
                Dim matches As MatchCollection = regex.Matches(fileContent)

                ' マッチが見つかったかどうかを返す
                Return matches.Count > 0
            Catch e As Exception
                log.PutLog("catch a exception" & e.ToString)
                Return False
            End Try

        End Function

        ' 指定されたフォルダ内のファイルを検索し、正規表現パターンにマッチする文字列をリストとして返すFunction
        Private Function ProcessFiles(ByVal folder As String, ByVal file As String, ByVal regexPattern As String) As List(Of String)
            Try
                Dim matchedResults As New List(Of String)()

                ' 指定されたフォルダー以下のすべてのファイルを取得
                Dim files As String() = Directory.GetFiles(folder, file, SearchOption.AllDirectories)

                ' 各ファイルに対して処理を行う
                For Each filePath As String In files
                    log.PutLog("Readfile:" & filePath)
                    ' ファイルの内容を読み込む
                    Dim fileContent As String = IO.File.ReadAllText(filePath)

                    ' Regexオブジェクトを作成
                    Dim regex As New Regex(regexPattern)

                    ' ファイル内容で正規表現と一致する対象をすべて検索
                    Dim matches As MatchCollection = regex.Matches(fileContent)

                    ' 一致するすべてのマッチをリストに追加
                    For Each match As Match In matches
                        log.PutLog("match:" & match.Value)
                        matchedResults.Add(match.Value)
                    Next
                Next

                Return matchedResults
            Catch e As Exception
                log.PutLog("catch a exception" & e.ToString)
                Return Nothing
            End Try

        End Function
    End Class





End Class
