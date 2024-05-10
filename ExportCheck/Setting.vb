Imports ExportCheck.Setting.Target
Imports Newtonsoft.Json
Imports System.IO
Imports System.Text.Json.Nodes

Public Class Setting

    ' シリアライズするクラスの定義
    Public Class Target
        Public Folder As String
        Public File As String

        '条件に合った場合返すエラーレベル
        Public ErrorLevel As Integer

        'ファイル内正規表現検索を行うかのフラグ
        Public RegexSearchFlag As Boolean
        '検索する正規表現
        Public RegexPattern As String
        '検索して見つかったらエラーにするか/見つからなかったらエラーにするか
        Public ErrorOnFoundFlag As Boolean

        'メッセージファイルを出力するかのフラグ
        Public OutputFlag As Boolean

        'メッセージ置き換え用の文言切り出しようの正規表現／ラベル
        Public Property LabelRegexes As LabelRegex()
        Public Class LabelRegex
            Public Folder As String
            Public File As String
            Public LabelRegexPatten As String
            Public LabelName As String
        End Class

        '{{@ラベル名}}を切り出した文言に置き換えて出力
        Public OutputMessage As String
        '出力先
        Public OutputFile As String
    End Class

    ' JSONファイルの出力（配列データ）
    Public Sub WriteJsonArrayToFile()

        Dim Targets As Target() = {New Target With {
                                                    .Folder = "%WORKFOLDER%¥exp2D¥",
                                                    .File = "exp_*.txt",
                                                    .RegexSearchFlag = True,
                                                    .RegexPattern = "正規表現",
                                                    .ErrorOnFoundFlag = True,
                                                    .OutputFlag = True,
                                                    .LabelRegexes = {New Target.LabelRegex With {
                                                        .Folder = "%WORKFOLDER%¥exp2D¥",
                                                        .File = "exp_*.txt",
                                                        .LabelRegexPatten = "ITEMID",
                                                        .LabelName = "ItemID"
                                                        },
                                                        New Target.LabelRegex With {
                                                        .Folder = "%WORKFOLDER%¥exp2D¥",
                                                        .File = "exp_*.txt",
                                                        .LabelRegexPatten = "FILENAME",
                                                        .LabelName = "@DB/{{@ItemID}}/001.*\n.*\n.*Clone_Name: ""(.*?)"""
                                                        }
                                                    },
                                                    .OutputMessage = "{{@ITEMID}}が出力できない。{{@FILENAME}}"
                                             }
                                    }

        '                       New Propaty With {.id = 2, .name = "田中", .sikaku = {"基本"}}}

        Dim Json As String = JsonConvert.SerializeObject(Targets)
        IO.File.WriteAllText("propatys.json", Json)
    End Sub


    ' JSONファイルの読み込み（配列データ）
    Sub Main()
        Dim Json As String = IO.File.ReadAllText("propatys.json")
        Dim Targets As Target() = JsonConvert.DeserializeObject(Of Target())(Json)

        For Each Target In Targets
            Console.WriteLine()
            Console.WriteLine(Target.File)
            For Each LabelRegex In Target.LabelRegexes
                Console.WriteLine(LabelRegex.LabelName)
            Next
        Next
    End Sub




End Class
