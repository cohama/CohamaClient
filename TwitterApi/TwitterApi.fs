
namespace TwitterApi

open System.Net
open System.IO
open System.Xml
open OAuth

type TwitterApi( oauth:OAuth ) =

    let TwitterApiUrl = "http://api.twitter.com/1/"

    member this.PublicTimeLine() =
        let path = "statuses/public_timeline.xml"
        let req = WebRequest.Create( TwitterApiUrl + path )
        let res = req.GetResponse()
        use st = res.GetResponseStream()
        use sr = new StreamReader( st )

        let xmlDoc = XmlDocument()
        xmlDoc.LoadXml( sr.ReadToEnd() )
    
        xmlDoc.SelectNodes( "/statuses/status" );

    member this.HomeTimeLine() =
        let path = "statuses/home_timeline.xml"
        let url = TwitterApiUrl + path
        
        let res = GetOAuthWebResponse url (oauth.ToAuthorizationHeader url "GET") "GET"

        use st = res.GetResponseStream()
        use sr = new StreamReader( st )

        let xmlDoc = new XmlDocument()
        xmlDoc.LoadXml( sr.ReadToEnd() )
        
        xmlDoc.SelectNodes( "/statuses/status" )