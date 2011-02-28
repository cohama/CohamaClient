
namespace TwitterApi

open System
open System.IO
open System.Net
open System.Text

module OAuthUtility =
    let UrlEncode url =
        let population = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
        let checkChar (sb:StringBuilder) (c:char) =
            match c with
            | '.'
            | '-'
            | '_'
                -> sb.Append( c )
            | c when Seq.exists ((=) c) population
                -> sb.Append( c )
            | ' '
                -> sb.Append( '+' )
            | _
                -> sb.Append( sprintf "%%%X" (Convert.ToInt32( c )) )

        let sb = Seq.fold checkChar (new StringBuilder()) url
        sb.ToString()

    let UrlEncodeWithUtf8 (url:string) =
        url
        |> Encoding.UTF8.GetBytes
        |> Seq.map char
        |> UrlEncode

    let FromString (token:string) =
        let reqToken = token.Split( '&' )
        let splitValue (s:string) = s.Split( '=' ).[1]
        (splitValue reqToken.[0], splitValue reqToken.[1])   

    let GetOAuthRequestResult (url:string) headerString httpMethod =
        let response =
            try
                let req = WebRequest.Create( url )
                req.Headers.Add( "Authorization", headerString )
                req.Method <- httpMethod
                req.GetResponse()
            
            with
            | :? System.Net.WebException as ex ->
                use st = ex.Response.GetResponseStream()
                use sr = new StreamReader( st )
                printfn "---->\n%s <----" <| sr.ReadToEnd()
                reraise()

        use stream = response.GetResponseStream()
        use reader = new StreamReader( stream )
        reader.ReadToEnd()
