module OAuth

let OAuthRoot = "http://twitter.com/oauth/"
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
            -> sb.Append( sprintf "%%%02X" (System.Convert.ToInt32( c )) )

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

    let getNonce() =
        sprintf "%08d" (rnd.Next( 10000000 ))

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

    member this.ToHeaderStringForRequestToken() = toHeaderString RequestTokenUrl ("", "") "GET" defaultParams

    member this.ToHeaderStringForAccessToken( requestToken, verifier ) = 
        let paramsForAccess = ("oauth_token", fst requestToken)::("oauth_verifier", verifier)::defaultParams
        toHeaderString AccessTokenUrl requestToken "GET" paramsForAccess

    member this.ToHeaderStringForApi( accessToken, url ) =
        let paramsForApi = ("oauth_token", fst accessToken)::defaultParams
        toHeaderString url accessToken "GET" paramsForApi

        
let GetRequestToken( consumerKey, consumerSecret ) = 

    let authParams = new OAuthParameters( consumerKey, consumerSecret )

    let headerString = authParams.ToHeaderStringForRequestToken()

    let response = 
        try
            let req = System.Net.WebRequest.Create( RequestTokenUrl )
            req.Headers.Add( "Authorization", headerString )
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
        
    use stream = response.GetResponseStream()
    use sr = new System.IO.StreamReader( stream )
    let reqToken = sr.ReadToEnd().Split( '&' )
    let splitValue (s:string) = s.Split( '=' ).[1]
    (splitValue reqToken.[0], splitValue reqToken.[1])

let GetAuthorizationUrl requestToken = AuthorizeUrl + @"?oauth_token=" + fst requestToken

let GetAccessToken( consumerKey, consumerSecret, requestToken, verifier ) =
    
    let authParams = new OAuthParameters( consumerKey, consumerSecret )

    let headerString = authParams.ToHeaderStringForAccessToken( requestToken, verifier )

    let response = 
        try
            let req = System.Net.WebRequest.Create( AccessTokenUrl )
            req.Headers.Add( "Authorization", headerString )
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
        
    use stream = response.GetResponseStream()
    use sr = new System.IO.StreamReader( stream )
    let reqToken = sr.ReadToEnd().Split( '&' )
    let splitValue (s:string) = s.Split( '=' ).[1]
    (splitValue reqToken.[0], splitValue reqToken.[1])