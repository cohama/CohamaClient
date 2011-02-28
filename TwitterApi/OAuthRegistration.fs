
namespace TwitterApi

open OAuthUrl
open OAuthUtility

type OAuthRegistration( ckey, csec ) =

    let authParams = new OAuthParameters( ckey, csec )

    member this.ConsumerKey = ckey
    member this.ConsumerSecret = csec
        
    member this.GetRequestToken() = 
        let headerString = authParams.ToHeaderStringForRequestToken()
        GetOAuthRequestResult RequestTokenUrl headerString "POST"
        |> FromString

    member this.GetAuthorizationUrl requestToken =
        AuthorizeUrl + @"?oauth_token=" + fst requestToken

    member this.GetAccessToken( requestToken, verifier ) =
        let headerString = authParams.ToHeaderStringForAccessToken( requestToken, verifier )
        GetOAuthRequestResult AccessTokenUrl headerString "POST"
        |> FromString