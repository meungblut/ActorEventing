using System;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

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

        public async Task<string> GetFeedAsString(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(new Uri(url));
            return response;
        }

        private async Task<Stream> GetDocumentStream(string atomFeedUrl)
        {
            HttpClient client = new HttpClient();
            Stream response = await client.GetStreamAsync(new Uri(atomFeedUrl));
            return response;
        }
    }
}
