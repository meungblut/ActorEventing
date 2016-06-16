using Eventing.Atom.Document;

namespace Eventing.Atom.Serialization
{
    public class AtomDocumentSerialiser
    {
        public string Serialise(AtomDocument feed, string baseUrl)
        {
            string data = @"<?xml version=""1.0"" encoding=""utf-8""?><feed xmlns=""http://www.w3.org/2005/Atom"">";
            data += @"<Title>" + feed.Title + @"</Title>";

            if (feed.DocumentId != null)
                data += @"<link rel=""self"" type =""application/atom+xml"" href=""" + baseUrl + feed.DocumentId.Id + @"""/>";

            if (feed.PreviousArchiveDocumentId != null && !string.IsNullOrEmpty(feed.PreviousArchiveDocumentId.Id))
            {
                data += @"<link rel=""prev-archive"" type =""application/atom+xml"" href =""" + baseUrl + feed.PreviousArchiveDocumentId.Id + @""" />";
            }

            if (feed.NextArchiveDocumentId != null && !string.IsNullOrEmpty(feed.NextArchiveDocumentId.Id))
            {
                data += @"<link rel=""next-archive"" type =""application/atom+xml"" href =""" + baseUrl + feed.NextArchiveDocumentId.Id + @""" />";
            }

            data += @"<Updated>" + feed.Updated.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") + "</Updated>";
            data += @"<Author><name>" + feed.Author + "</name></Author>";
            data += @"<id>urn:uuid:" + feed.DocumentId.Id + "</id>";

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
