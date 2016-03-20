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
            data += @"<Title>" + feed.Title + @"</Title>";

            if (feed.DocumentId != null)
                data += @"<link href=""" + baseUrl + feed.DocumentId.Id + @"""/>";

            if (!(feed.EarlierEventsDocumentId == null) && !string.IsNullOrEmpty(feed.EarlierEventsDocumentId.Id))
            {
                data += @"<link rel=""prev-archive"" type =""application/atom+xml"" href =""" + baseUrl + feed.EarlierEventsDocumentId.Id + @""" />";
            }

            if (!(feed.LaterEventsDocumentId == null) && !string.IsNullOrEmpty(feed.LaterEventsDocumentId.Id))
            {
                data += @"<link rel=""next-archive"" type =""application/atom+xml"" href =""" + baseUrl + feed.LaterEventsDocumentId.Id + @""" />";
            }

            data += @"<Updated>" + feed.Updated.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") + "</Updated>";
            data += @"<Author><name>" + feed.Author + "</name></Author>";
            data += @"<id>urn:uuid:" + feed.FeedId.Id + "</id>";

            data += @"<documentInformation>" + feed.DocumentInformation + "</documentInformation>";


            foreach (var atomEntry in feed.Entries)
            {
                data += "<entry>";
                data += "<Title>" + atomEntry.Title + "</Title>";
                data += "<id>urn:uuid:" + atomEntry.Id + "</id>";
                data += "<Updated>" + atomEntry.Updated.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") + "</Updated>";
                data += @"<content type=""application/json"">" + atomEntry.Content + "</content>";
                data += "</entry>";
            }
            data += "</feed>";

            return data;
        }
    }
}
