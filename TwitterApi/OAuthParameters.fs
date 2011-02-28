
namespace TwitterApi

open OAuthUtility
open OAuthUrl
open System
open System.Text
open System.Security.Cryptography

type private OAuthParameters( consumerKey, consumerSecret ) =

    let encryptByHmacSha1WithBase64 (key:string) (source:string) = 
        let srcByte = Encoding.UTF8.GetBytes( source )
        let keyByte = Encoding.UTF8.GetBytes( key )
        let hmacsha1 = new HMACSHA1( keyByte )
        let hashed = hmacsha1.ComputeHash( srcByte )
        Convert.ToBase64String( hashed )

    let rnd = new System.Random()
    let population = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
    let getNonce() =
        let rec generate (sb:System.Text.StringBuilder) =
            function
            | 0 -> sb
            | i -> generate (sb.Append( population.[rnd.Next( population.Length )] )) (i-1)
        (generate (new StringBuilder()) 30).ToString()

    let getTimeStamp() =
        let ts = DateTime.UtcNow - new DateTime( 1970, 1, 1 )
        Convert.ToInt64( ts.TotalSeconds ).ToString()
    
    let defaultParams () = 
        [
            "oauth_callback", "oob";
            "oauth_consumer_key", consumerKey;
            "oauth_nonce", getNonce();
            "oauth_signature_method", "HMAC-SHA1";
            "oauth_timestamp", getTimeStamp();
            "oauth_version", "1.0";
        ]
        
    let toHeaderString url token httpmethod paramsList exparams =
        paramsList@exparams
        |> List.sort
        |> List.map (fun ( k, v ) -> k + "=" + v)
        |> List.reduce (fun acc item -> acc + "&" + item)
        |> fun param -> httpmethod + "&" + UrlEncode url + "&" + UrlEncode param
        |> encryptByHmacSha1WithBase64 (consumerSecret + "&" + snd token)
        |> fun signature -> ("oauth_signature", signature)::paramsList
        |> List.sort
        |> List.map (fun ( k, v ) -> k + "=\"" + (UrlEncode v) + "\"")
        |> List.reduce (fun acc item -> acc + ", " + item)
        |> fun s -> "OAuth " + s
        
    member this.ToHeaderStringForRequestToken() =
        toHeaderString RequestTokenUrl ("", "") "POST" (defaultParams()) []

    member this.ToHeaderStringForAccessToken( requestToken, verifier ) = 
        let paramsForAccess = ("oauth_token", fst requestToken)::("oauth_verifier", verifier)::defaultParams()
        toHeaderString AccessTokenUrl requestToken "POST" paramsForAccess []

    member this.ToHeaderStringForApi( accessToken, url, httpmethod, parameters ) =
        let paramsForApi = ("oauth_token", fst accessToken)::defaultParams()
        toHeaderString url accessToken httpmethod paramsForApi parameters