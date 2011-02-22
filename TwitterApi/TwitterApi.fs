[<RequireQualifiedAccess>]
module TwitterApi

open System.Net
open System.IO
open System.Xml

let TwitterApiUrl = "http://api.twitter.com/1/"

let PublicTimeLine () =
    let path = "statuses/public_timeline.xml"
    let req = WebRequest.Create( TwitterApiUrl + path )
    let res = req.GetResponse()
    use st = res.GetResponseStream()
    use sr = new StreamReader( st )

    let xmlDoc = XmlDocument()
    xmlDoc.LoadXml( sr.ReadToEnd() )
    
    xmlDoc.SelectNodes( "/statuses/status" );