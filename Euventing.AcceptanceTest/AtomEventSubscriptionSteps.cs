using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Euventing.AcceptanceTest.Hosting;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Euventing.Core;
using System.IO;

namespace Euventing.AcceptanceTest
{
    [Binding]
    public class AtomEventSubscriptionSteps
    {
        private HttpResponseMessage httpResponseMessage;
        private string url;
        private EventPublisher publisher;
        private string subscriptionId;

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
            Console.WriteLine(DateTime.Now.ToString("Start M:ss ffff"));

            HttpClient client = new HttpClient();
            this.httpResponseMessage = client.GetAsync(url + subscriptionId).Result;

            Console.WriteLine(DateTime.Now.ToString("Finish M:ss ffff"));

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
            Console.WriteLine(DateTime.Now.ToString("Start: M:ss ffff"));
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

            Console.WriteLine(DateTime.Now.ToString("Finish: M:ss ffff"));

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [When(@"'(.*)' events are raised within my domain")]
        public void WhenEventsAreRaisedWithinMyDomain(int numberOfEventsToRaise)
        {
            Console.WriteLine(DateTime.Now.ToString("Start M:ss ffff"));

            for (int i = 0; i < numberOfEventsToRaise; i++)
            {
                publisher.PublishMessage(new DummyEvent(i.ToString()));
                Console.WriteLine(DateTime.Now.ToString("M:ss ffff"));

            }
            Console.WriteLine(DateTime.Now.ToString("Finish M:ss ffff"));

        }

        [Then(@"I should receive a valid atom document with '(.*)' entries from '(.*)'")]
        public void ThenIShouldReceiveAValidAtomDocumentWithEntriesFrom(int numberOfEventsExpected, string atomUrl)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));

            var atomDocument = GetDocumentStream(atomUrl + subscriptionId).Result;

            using (var xmlReader = XmlReader.Create(atomDocument))
            {
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                if (feed != null)
                {
                    Assert.AreEqual(numberOfEventsExpected, feed.Items.Count());
                }
            }
        }

        [Then(@"I should be able to retrieve the earlier document by issuing a GET to its url")]
        public void ThenIShouldBeAbleToRetrieveTheEarlierDocumentByIssuingAGETToItsUrl()
        {
            ScenarioContext.Current.Pending();
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

            Assert.IsTrue(atomDocument.Contains("prev-archive"));
        }
    }
}
