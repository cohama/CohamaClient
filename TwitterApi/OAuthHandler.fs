namespace TwitterApi

open System
open System.IO
open System.Text
open System.Xml
open System.Xml.Serialization
open OAuthUtility
    
type OAuthHandler( ckey, csec, akey, asec ) =

    let parameters = new OAuthParameters( ckey, csec )

    private new() = new OAuthHandler( "", "", "", "" )

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

    member this.SaveAs file =
        let data = seq [this.ConsumerKey; this.ConsumerSecret; this.AccessKey; this.AccessSecret]
        File.WriteAllLines( file, data )

    static member LoadFrom file =
        let lines = File.ReadAllLines file
        new OAuthHandler( lines.[0], lines.[1], lines.[2], lines.[3] )