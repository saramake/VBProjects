Imports dcpf_regexcheck.regexcheck
Imports Newtonsoft.Json

Module MainPG

    Sub Main()

    End Sub

    ' JSONファイルの出力（配列データ）
    Public Sub WriteJsonArrayToFile()

        Dim Targets As Target() = {New Target With {
                                                    .Label = "BVR同期警告",
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
    Sub read()
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

End Module
