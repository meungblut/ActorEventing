using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.AcceptanceTest.Client
{
    public class SubscriptionClient
    {
        private readonly string subscriptionUrl;

        public SubscriptionClient(string subscriptionUrl)
        {
            this.subscriptionUrl = subscriptionUrl;
        }

        public async Task Subscribe(string subscriptionId)
        {
            string contents = @"
                {
                'channel' : 'atom',
		        'subscriptionId' : '{subscriptionId}'
                }
            ".Replace("{subscriptionId}", subscriptionId);

            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(contents, Encoding.UTF8, "application/json");
            var httpResponseMessage
                = await client.PutAsync(subscriptionUrl, content);

            if (!httpResponseMessage.IsSuccessStatusCode)
                throw new CouldNotCreateSubscriptionException(httpResponseMessage.ReasonPhrase);
        }

        public async Task<string> GetSubscriptionDetails(string url)
        {
            HttpClient client = new HttpClient();
            var httpResponseMessage = client.GetAsync(url).Result;
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        public async Task Unsubscribe(string subscriptionId)
        {
            HttpClient client = new HttpClient();

            var httpResponseMessage
                = await client.DeleteAsync(subscriptionUrl + "/" + subscriptionId);

            if (!httpResponseMessage.IsSuccessStatusCode)
                throw new CouldNotCreateSubscriptionException(httpResponseMessage.ReasonPhrase);
        }
    }
}
