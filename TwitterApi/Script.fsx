// このファイルは、F# Interactive を使用して実行できるスクリプトです。  
// ライブラリ プロジェクトの調査やテストに使用できます。
// スクリプト ファイルはプロジェクト ビルドの一部ではないことに注意してください。

#load "TwitterApi.fs"
#load "OAuth.fs"
open TwitterApi
open OAuth
open System.Net

let oauthReq = new OAuthRequest( "YFrqm75k9YVg5jTCymRzg", "yF5sa6xYFvi22A3fTfszKH9aduQ5aESAVImaj9i30t0" )
printfn "%A" (oauthReq.GetRequestToken())

let signature =
    authParams
    |> List.sort
    |> List.map (fun ( k, v ) -> k + "=" + v)
    |> List.reduce (fun acc item -> acc + "&" + item)
    |> urlEncode
    |> fun urlEncoded -> "POST&" + OAuthUrl + RequestTokenUrl + "&" + urlEncoded
    |> EncryptByHmacSha1WithBase64 (consumerSecret + "&")
    |> urlEncode
let newParams = ("oauth_signature", signature)::authParams

let headerString =
    newParams
    |> List.sort
    |> List.map (fun ( k, v ) -> k + "=\"" + (urlEncode v) + "\"")
    |> List.reduce (fun acc item -> acc + ", " + item)
    |> fun s -> "Authorization: OAuth " + s
printfn "%s" headerString

let headerString =
    newParams
    |> Seq.sort
    |> Seq.map (fun ( k, v ) -> k + "=\"" + (urlEncode v) + "\"")
    |> Seq.reduce (fun acc item -> acc + ", " + item)
    |> fun s -> "Authorization: OAuth " + s

let url = "https://cohama.backlog.jp/LoginDisplay.action;jsessionid=E3E47A1EE360473E09AAB64426083937.h"
let req = HttpWebRequest.Create( url )
let res = req.GetResponse() :?> HttpWebResponse
printfn "%A" res.StatusCode;;

let time = System.Environment.TickCount

let s = @"/vmxzvcxzmv/xzmvcx/zmvr9>u0921-="
let t = System.Web.HttpUtility.UrlEncode( s )
printfn "%s" t;;


let authParams = new System.Collections.Generic.SortedSet<string>()
authParams.Add("oauth_consumer_key=" + "aiueo")
authParams.Add("oauth_signature_method=HMAC_SHA1")
authParams.Add("oauth_version=1.0")
authParams.Add("oauth_callback=oob")


let m = Map.ofSeq [ "aho","desuka"; "kasu","desuyo"; "123","ichinisan"];;
let a =
    m
    |> Map.map (fun k v -> k + "=" + v);;

let data = "The quick brown fox jumps over the lazy dog"
let histogram =
    data.ToCharArray()
    |> Seq.groupBy (fun c -> c)
    |> Map.ofSeq
    |> Map.map( fun k v -> Seq.length v )
for (KeyValue(c,n)) in histogram do 
    printfn "Number of '%c' characters = %d" c n 

let nums = seq[1 .. 100]
let gr =
    nums
    |> Seq.groupBy (fun c -> c%5)
    |> Map.ofSeq
Map.iter (printfn "%A%A") gr

let sortedDict = 
    Map [
        ( "aiueo", "kasu" );
        ( "01234", "desu" );
        ( "ooooo", "yo !" );
    ]
Map.iter (printfn "%A->>>%A") sortedDict

let authParams = 
    seq [
        "oauth_consumer_key", "consumerKey";
        "oauth_signature_method", "HMAC_SHA1";
        "oauth_version", "1.0";
        "oauth_callback", "oob";
    ]
let a =
    authParams
    |> Seq.map (fun (k, v) -> k + " : aho : " + v) 
    |> Seq.reduce (fun acc item -> acc + "\n" + item)
