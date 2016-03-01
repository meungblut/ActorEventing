using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Atom.Document;

namespace Euventing.Atom.Serialization
{
    public class AtomDocumentSerialiser
    {
        public string Serialise(AtomDocument feed, string baseUrl)
        {
            string data = @"<?xml version=""1.0"" encoding=""utf-8""?><feed xmlns=""http://www.w3.org/2005/Atom"">";
            data += @"<title>" + feed.Title + @"</title>";
            data += @"<link href=""" + baseUrl + feed.DocumentId.Id + @"""/>";

            if (!(feed.EarlierEventsDocumentId == null) && !string.IsNullOrEmpty(feed.EarlierEventsDocumentId.Id))
            {
                data += @"<link rel=""prev-archive"" type =""application/atom+xml"" href =""" + baseUrl + feed.EarlierEventsDocumentId.Id + @""" />";
            }

            if (!(feed.LaterEventsDocumentId == null) && !string.IsNullOrEmpty(feed.LaterEventsDocumentId.Id))
            {
                data += @"<link rel=""next-archive"" type =""application/atom+xml"" href =""" + baseUrl + feed.LaterEventsDocumentId.Id + @""" />";
            }

            data += @"<updated>" + feed.Updated.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") + "</updated>";
            data += @"<author><name>" + feed.Author + "</name></author>";
            data += @"<id>urn:uuid:" + feed.FeedId.Id + "</id>";

            foreach (var atomEntry in feed.Entries)
            {
                data += "<entry>";
                data += "<title>" + atomEntry.Title + "</title>";
                data += "<id>urn:uuid:" + atomEntry.Id + "</id>";
                data += "<updated>" + atomEntry.Updated.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") + "</updated>";
                data += @"<content type=""application/json"">" + atomEntry.Content + "</content>";
                data += "</entry>";
            }
            data += "</feed>";

            return data;
        }
    }
}
