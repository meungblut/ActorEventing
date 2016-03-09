using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Euventing.AcceptanceTest.Hosting;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Euventing.Core;
using System.IO;
using Euventing.AcceptanceTest.Client;

namespace Euventing.AcceptanceTest
{
    [Binding]
    public class AtomEventSubscriptionSteps
    {
        private HttpResponseMessage httpResponseMessage;
        private string url;
        private EventPublisher publisher;
        private string subscriptionId;
        private SyndicationFeed retrievedFeed;

        [Given(@"I have an eventing url at '(.*)'")]
        public void GivenIHaveAnEventingUrlAt(string url)
        {
            this.url = url;
            publisher = SpecflowGlobal.Host.Get<EventPublisher>();
        }


        [Given(@"I PUT a message to '(.*)' with the body")]
        public void GivenIputaMessageToWithTheBody(string url, string requestBody)
        {
            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            this.httpResponseMessage = client.PutAsync(url, content).Result;
        }

        [When(@"I request the subscription from url '(.*)'")]
        public void WhenIRequestTheSubscriptionFromUrl(string url)
        {
            Thread.Sleep(TimeSpan.FromSeconds(15));
            HttpClient client = new HttpClient();
            this.httpResponseMessage = client.GetAsync(url + subscriptionId).Result;
        }

        [Then(@"I should receive a response with the http status code '(.*)'")]
        public void ThenIShouldReceiveAResponseWithTheHttpStatusCode(HttpStatusCode statusCode)
        {
            Assert.AreEqual(statusCode, this.httpResponseMessage.StatusCode);
        }

        [Then(@"a content type of '(.*)'")]
        public void ThenAContentTypeOf(string expectedContentType)
        {
            Assert.AreEqual(expectedContentType, this.httpResponseMessage.Content.Headers.ContentType.MediaType);
        }

        [Then(@"a body")]
        public void ThenABody(string expectedBody)
        {
            string body = this.httpResponseMessage.Content.ReadAsStringAsync().Result;
            Assert.AreEqual(expectedBody, body, body);
        }

        [Given(@"I have subscribed to an atom feed with a generated subscription Id")]
        public void GivenIHaveSubscribedToAnAtomFeedWithASubscriptionIdOf()
        {
            this.subscriptionId = Guid.NewGuid().ToString();
            string contents = @"
                {
                'channel' : 'atom',
		        'subscriptionId' : '{subscriptionId}'
                }
            ".Replace("{subscriptionId}", subscriptionId);

            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(contents, Encoding.UTF8, "application/json");
            httpResponseMessage
                = client.PutAsync(url, content).Result;
            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [When(@"'(.*)' events are raised within my domain")]
        public void WhenEventsAreRaisedWithinMyDomain(int numberOfEventsToRaise)
        {
            for (int i = 0; i < numberOfEventsToRaise; i++)
            {
                publisher.PublishMessage(new DummyEvent(i.ToString()));
            }
        }

        [Then(@"I should receive a valid atom document with '(.*)' entries from '(.*)'")]
        public void ThenIShouldReceiveAValidAtomDocumentWithEntriesFrom(int numberOfEventsExpected, string atomUrl)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));

            var atomClient = new AtomClient();
            retrievedFeed = atomClient.GetFeed(atomUrl + subscriptionId).Result;
            Assert.AreEqual(numberOfEventsExpected, retrievedFeed.Items.Count());
        }

        [Then(@"I should be able to retrieve the earlier document by issuing a GET to its url")]
        public void ThenIShouldBeAbleToRetrieveTheEarlierDocumentByIssuingAGETToItsUrl()
        {
            var atomClient = new AtomClient();
            var url = retrievedFeed.Links.First(x => x.RelationshipType == "prev-archive").Uri.ToString();
            retrievedFeed = atomClient.GetFeed(url).Result;
            var atomDocument = GetAtomDocument(url);
            Assert.AreEqual(150, retrievedFeed.Items.Count());
        }

        private string GetAtomDocument(string atomUrl)
        {
            HttpClient client = new HttpClient();
            httpResponseMessage = client.GetAsync(atomUrl).Result;
            Assert.IsTrue(httpResponseMessage.IsSuccessStatusCode);
            var atomDocument = httpResponseMessage.Content.ReadAsStringAsync().Result;
            Console.WriteLine(atomDocument);
            return atomDocument;
        }

        public async Task<Stream> GetDocumentStream(string atomFeedUrl)
        {
            HttpClient client = new HttpClient();
            Stream response = await client.GetStreamAsync(new Uri(atomFeedUrl));
            return response;
        }

        [Then(@"I should receive an atom document with a link to the next document in the stream from '(.*)'")]
        public void ThenIShouldReceiveAnAtomDocumentWithALinkToTheNextDocumentInTheStreamFrom(string atomUrl)
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));

            var atomDocument = GetAtomDocument(atomUrl + subscriptionId);
            var atomClient = new AtomClient();
            retrievedFeed = atomClient.GetFeed(atomUrl + subscriptionId).Result;

            Assert.IsTrue(atomDocument.Contains("prev-archive"));
        }
    }
}
