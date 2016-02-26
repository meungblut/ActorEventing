using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Xml;
using Euventing.AcceptanceTest.Hosting;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Euventing.AcceptanceTest
{
    [Binding]
    public class AtomEventSubscriptionSteps
    {
        private HttpResponseMessage httpResponseMessage;

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
            HttpClient client = new HttpClient();
            this.httpResponseMessage = client.GetAsync(url).Result;
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
            Assert.AreEqual(expectedBody, body);
        }

        [When(@"an event is raised within my domain with id '(.*)'")]
        public void WhenAnEventIsRaisedWithinMyDomainWithId(string id)
        {

        }

        [Then(@"I should receive that event on my callback channel with id '(.*)'")]
        public void ThenIShouldReceiveThatEventOnMyCallbackChannelWithId(string expectedEventId)
        {
        }

        [Given(@"I have subscribed to an atom feed with a subscription Id of '(.*)'")]
        public async void GivenIHaveSubscribedToAnAtomFeedWithASubscriptionIdOf(string subscriptionId)
        {
        }

        [When(@"'(.*)' events are raised within my domain")]
        public void WhenEventsAreRaisedWithinMyDomain(int numberOfEventsToRaise)
        {
            for (int i = 0; i < numberOfEventsToRaise; i++)
            {
            }
        }

        [Then(@"I should receive a valid atom document with '(.*)' entries")]
        public void ThenIShouldReceiveAValidAtomDocumentWithEntries(int numberOfEventsExpected)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));

            var atomDocument = "";

            using (var xmlReader = XmlReader.Create(atomDocument))
            {
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                if (feed != null)
                {
                    Assert.AreEqual(numberOfEventsExpected, feed.Items.Count());
                }
            }
        }

        [Then(@"I should receive an atom document with a link to the next document in the stream")]
        public void ThenIShouldReceiveAnAtomDocumentWithALinkToTheNextDocumentInTheStream()
        {

        }
    }
}
