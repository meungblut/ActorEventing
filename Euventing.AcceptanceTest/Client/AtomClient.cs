using System;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Euventing.AcceptanceTest.Client
{
    public class AtomClient
    {
        internal string RetrievedString { get; set; }
        public async Task<SyndicationFeed> GetFeed(string url, TimeSpan timeout)
        {
            var atomDocument = await GetDocumentStream(url, timeout);

            using (var xmlReader = XmlReader.Create(atomDocument))
            {
                return SyndicationFeed.Load(xmlReader);
            }
        }
      
        private async Task<Stream> GetDocumentStream(string atomFeedUrl, TimeSpan timeout)
        {
            var client = new HttpClient();
            client.Timeout = timeout;
            var response = await client.GetAsync(new Uri(atomFeedUrl));
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ReadAsStringAsync().Result);

            RetrievedString = response.Content.ReadAsStringAsync().Result;

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
