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
        public async Task<SyndicationFeed> GetFeed(string url, TimeSpan timeout)
        {
            var atomDocument = await GetDocumentStream(url, timeout);

            using (var xmlReader = XmlReader.Create(atomDocument))
            {
                return SyndicationFeed.Load(xmlReader);
            }
        }

        public async Task<string> GetFeedAsString(string url, TimeSpan timeout)
        {
            var client = new HttpClient();
            client.Timeout = timeout;
            var response = await client.GetStringAsync(new Uri(url));
            return response;
        }

        private async Task<Stream> GetDocumentStream(string atomFeedUrl, TimeSpan timeout)
        {
            var client = new HttpClient();
            client.Timeout = timeout;
            var response = await client.GetStreamAsync(new Uri(atomFeedUrl));
            return response;
        }
    }
}
