[<RequireQualifiedAccess>]
module OAuth

let OAuthRoot = "https://twitter.com/oauth/"
let RequestTokenUrl = OAuthRoot + "request_token"
let AccessTokenUrl = OAuthRoot + "access_token"
let AuthorizeUrl = OAuthRoot + "authorize"

let UrlEncode (url:string) =
    let checkChar (sb:System.Text.StringBuilder) (c:char) =
        match c with
        | '.'
        | '-'
        | '_'
            -> sb.Append( c )
        | c when System.Char.IsLetterOrDigit( c )
            -> sb.Append( c )
        | _
            -> sb.Append( sprintf "%%%X" (System.Convert.ToInt32( c )) )

    let sb = Seq.fold checkChar (new System.Text.StringBuilder()) url
    sb.ToString()

type OAuthParameters( consumerKey, consumerSecret ) =

    let encryptByHmacSha1WithBase64 (key:string) (source:string) = 
        let srcByte = System.Text.Encoding.UTF8.GetBytes( source )
        let keyByte = System.Text.Encoding.UTF8.GetBytes( key )
        let hmacsha1 = new System.Security.Cryptography.HMACSHA1( keyByte )
        let hashed = hmacsha1.ComputeHash( srcByte )
        System.Convert.ToBase64String( hashed )

    let rnd = new System.Random()
    let population = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
    let getNonce() =
        let rec generate (sb:System.Text.StringBuilder) =
            function
            | 0 -> sb
            | i -> generate (sb.Append( population.[rnd.Next( population.Length )] )) (i-1)
        (generate (System.Text.StringBuilder()) 30).ToString()

    let getTimeStamp() =
        let ts = System.DateTime.UtcNow - new System.DateTime( 1970, 1, 1 )
        System.Convert.ToInt64( ts.TotalSeconds ).ToString()
    
    let defaultParams = 
        [
            "oauth_callback", "oob";
            "oauth_consumer_key", consumerKey;
            "oauth_nonce", getNonce();
            "oauth_signature_method", "HMAC-SHA1";
            "oauth_timestamp", getTimeStamp();
            "oauth_version", "1.0";
        ]
        
    let toHeaderString url token httpmethod paramsList =
        paramsList
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

    member this.ToHeaderStringForRequestToken() = toHeaderString RequestTokenUrl ("", "") "POST" defaultParams

    member this.ToHeaderStringForAccessToken( requestToken, verifier ) = 
        let paramsForAccess = ("oauth_token", fst requestToken)::("oauth_verifier", verifier)::defaultParams
        toHeaderString AccessTokenUrl requestToken "POST" paramsForAccess

    member this.ToHeaderStringForApi( accessToken, url ) =
        let paramsForApi = ("oauth_token", fst accessToken)::defaultParams
        toHeaderString url accessToken "POST" paramsForApi

let private tokenFromString (token:string) =
    let reqToken = token.Split( '&' )
    let splitValue (s:string) = s.Split( '=' ).[1]
    (splitValue reqToken.[0], splitValue reqToken.[1])

let GetOAuthWebResponse (url:string) (headerString:string) (method:string) =
    try
        let req = System.Net.WebRequest.Create( url )
        req.Headers.Add( "Authorization", headerString )
        req.Method <- method
        req.GetResponse()
            
    with
    | :? System.Net.WebException as ex ->
        match ex.Status with
        | System.Net.WebExceptionStatus.ProtocolError -> 
            use st = ex.Response.GetResponseStream()
            use sr = new System.IO.StreamReader( st )
            printfn "---->\n%s <----" <| sr.ReadToEnd()
            reraise()
        | _ ->
            reraise()
        
let GetRequestToken( consumerKey, consumerSecret ) = 

    let authParams = new OAuthParameters( consumerKey, consumerSecret )

    let headerString = authParams.ToHeaderStringForRequestToken()

    let response = GetOAuthWebResponse RequestTokenUrl headerString "POST"
        
    use stream = response.GetResponseStream()
    use sr = new System.IO.StreamReader( stream )
    tokenFromString <| sr.ReadToEnd()

let GetAuthorizationUrl requestToken = AuthorizeUrl + @"?oauth_token=" + fst requestToken

let GetAccessToken( consumerKey, consumerSecret, requestToken, verifier ) =
    
    let authParams = new OAuthParameters( consumerKey, consumerSecret )

    let headerString = authParams.ToHeaderStringForAccessToken( requestToken, verifier )

    let response = GetOAuthWebResponse AccessTokenUrl headerString "POST"

    use stream = response.GetResponseStream()
    use sr = new System.IO.StreamReader( stream )
    tokenFromString <| sr.ReadToEnd()