module OAuth

open System
open System.Net
open System.Text
open System.IO
open System.Xml
open System.Security.Cryptography

let OAuthRoot = "https://twitter.com/oauth/"
let RequestTokenUrl = OAuthRoot + "request_token"
let AccessTokenUrl = OAuthRoot + "access_token"
let AuthorizeUrl = OAuthRoot + "authorize"

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

type OAuthToken( key:string, secret:string ) =

    member this.Key = key
    member this.Secret = secret
    
    new() = new OAuthToken( "", "" )

    static member Empty = new OAuthToken()

    static member FromString (token:string) =
        let reqToken = token.Split( '&' )
        let splitValue (s:string) = s.Split( '=' ).[1]
        new OAuthToken(splitValue reqToken.[0], splitValue reqToken.[1])

type private OAuthParameters( consumerToken:OAuthToken ) =

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
        (generate (StringBuilder()) 30).ToString()

    let getTimeStamp() =
        let ts = DateTime.UtcNow - new DateTime( 1970, 1, 1 )
        Convert.ToInt64( ts.TotalSeconds ).ToString()
    
    let defaultParams = 
        [
            "oauth_callback", "oob";
            "oauth_consumer_key", consumerToken.Key;
            "oauth_nonce", getNonce();
            "oauth_signature_method", "HMAC-SHA1";
            "oauth_timestamp", getTimeStamp();
            "oauth_version", "1.0";
        ]
        
    let toHeaderString url (token:OAuthToken) httpmethod paramsList exparams =
        paramsList@exparams
        |> List.sort
        |> List.map (fun ( k, v ) -> k + "=" + v)
        |> List.reduce (fun acc item -> acc + "&" + item)
        |> fun param -> httpmethod + "&" + UrlEncode url + "&" + UrlEncode param
        |> encryptByHmacSha1WithBase64 (consumerToken.Secret + "&" + token.Secret)
        |> fun signature -> ("oauth_signature", signature)::paramsList
        |> List.sort
        |> List.map (fun ( k, v ) -> k + "=\"" + (UrlEncode v) + "\"")
        |> List.reduce (fun acc item -> acc + ", " + item)
        |> fun s -> "OAuth " + s
        
    member this.ToHeaderStringForRequestToken() = toHeaderString RequestTokenUrl OAuthToken.Empty "POST" defaultParams []

    member this.ToHeaderStringForAccessToken( requestToken:OAuthToken, verifier ) = 
        let paramsForAccess = ("oauth_token", requestToken.Key)::("oauth_verifier", verifier)::defaultParams
        toHeaderString AccessTokenUrl requestToken "POST" paramsForAccess []

    member this.ToHeaderStringForApi( accessToken:OAuthToken, url, httpmethod, parameters ) =
        let paramsForApi = ("oauth_token", accessToken.Key)::defaultParams
        toHeaderString url accessToken httpmethod paramsForApi parameters

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
    
type OAuth( ckey, csec ) =
    
    let mutable m_aToken:OAuthToken = OAuthToken.Empty

    member this.ConsumerToken = new OAuthToken( ckey, csec )
    member this.AccessToken
        with get()           = m_aToken
        and  set accessToken = m_aToken <- accessToken

    member this.GetRequest url =
        let parameters = new OAuthParameters( this.ConsumerToken )
        let headerstring = parameters.ToHeaderStringForApi( this.AccessToken, url, "GET", [] )
        let xmlDoc = new XmlDocument()
        GetOAuthRequestResult url headerstring "GET"
        |> xmlDoc.LoadXml
        xmlDoc

    member this.PostRequest url body =
        let parameters = new OAuthParameters( this.ConsumerToken )
        let urlWithParam =
            body
            |> List.map (fun (k, v) -> k + "=" + v)
            |> List.reduce (fun acc item -> acc + "&" + item)
            |> (+) (url + "?")
        let headerstring = parameters.ToHeaderStringForApi( this.AccessToken, url, "POST", body )
        let xmlDoc = new XmlDocument()
        GetOAuthRequestResult urlWithParam headerstring "POST"
        |> xmlDoc.LoadXml
        xmlDoc
        
    member this.GetRequestToken() = 
        let authParams = new OAuthParameters( this.ConsumerToken )
        let headerString = authParams.ToHeaderStringForRequestToken()
        GetOAuthRequestResult RequestTokenUrl headerString "POST"
        |> OAuthToken.FromString

    member this.GetAuthorizationUrl (requestToken:OAuthToken) =
        AuthorizeUrl + @"?oauth_token=" + requestToken.Key

    member this.GetAccessToken( requestToken, verifier ) =
        let authParams = new OAuthParameters( this.ConsumerToken)
        let headerString = authParams.ToHeaderStringForAccessToken( requestToken, verifier )
        GetOAuthRequestResult AccessTokenUrl headerString "POST"
        |> OAuthToken.FromString