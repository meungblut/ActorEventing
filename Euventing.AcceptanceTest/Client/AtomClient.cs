using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;

namespace Euventing.AcceptanceTest.Client
{
    public class AtomClient
    {
        public async Task<SyndicationFeed> GetFeed(string url)
        {
            var atomDocument = await GetDocumentStream(url);

            using (var xmlReader = XmlReader.Create(atomDocument))
            {
                return SyndicationFeed.Load(xmlReader);
            }
        }

        private async Task<Stream> GetDocumentStream(string atomFeedUrl)
        {
            HttpClient client = new HttpClient();
            Stream response = await client.GetStreamAsync(new Uri(atomFeedUrl));
            return response;
        }
    }
}
