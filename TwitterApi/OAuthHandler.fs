
namespace TwitterApi

open System
open System.Text
open System.Xml
open OAuthUtility
    
type OAuthHandler( ckey, csec, akey, asec ) =

    let parameters = new OAuthParameters( ckey, csec )

    member this.ConsumerKey = ckey
    member this.ConsumerSecret = csec
    member this.AccessKey = akey
    member this.AccessSecret = asec

    member this.GetRequest url =
        let headerstring = parameters.ToHeaderStringForApi( (this.AccessKey, this.AccessSecret), url, "GET", [] )
        let xmlDoc = new XmlDocument()
        GetOAuthRequestResult url headerstring "GET"
        |> xmlDoc.LoadXml
        xmlDoc

    member this.PostRequest url body =
        let encodedBody =
            body
            |> List.map (fun (k, v) -> k, UrlEncodeWithUtf8 v)
        let urlWithParam =
            encodedBody
            |> List.map (fun (k, v) -> k + "=" + v)
            |> List.reduce (fun acc item -> acc + "&" + item)
            |> (+) (url + "?")
        let headerstring = parameters.ToHeaderStringForApi( (this.AccessKey, this.AccessSecret), url, "POST", encodedBody )
        let xmlDoc = new XmlDocument()
        GetOAuthRequestResult urlWithParam headerstring "POST"
        |> xmlDoc.LoadXml
        xmlDoc