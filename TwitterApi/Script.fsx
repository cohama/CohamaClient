// このファイルは、F# Interactive を使用して実行できるスクリプトです。  
// ライブラリ プロジェクトの調査やテストに使用できます。
// スクリプト ファイルはプロジェクト ビルドの一部ではないことに注意してください。

#load "TwitterApi.fs"
open TwitterApi

open System.Net
let url = "http://cacoo.com"
let req = WebRequest.Create( url )
let res = webreq.GetResponse() :?> HttpWebResponse
printfn "%A" res.StatusDescription;;

open System.Net
let url = "https://cohama.backlog.jp/LoginDisplay.action;jsessionid=E3E47A1EE360473E09AAB64426083937.h"
let req = WebRequest.Create( urls )
let res = req.GetResponse() :?> HttpWebResponse
printfn "%A" res.StatusCode;;

