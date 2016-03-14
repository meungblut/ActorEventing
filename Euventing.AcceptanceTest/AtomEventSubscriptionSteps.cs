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
using Euventing.Test.Shared;

namespace Euventing.AcceptanceTest
{
    [Binding]
    public class AtomEventSubscriptionSteps
    {
        private HttpResponseMessage httpResponseMessage;
        private string url;
        private IEventPublisher publisher;
        private string subscriptionId;
        private SyndicationFeed retrievedFeed;

        [AfterScenario("atomEvents", "multiNode")]
        public void UnsubscribeFromMessages()
        {
            var client = new SubscriptionClient(url);
            client.Unsubscribe(subscriptionId).Wait();
        }

        [Given(@"I have an eventing url at '(.*)'")]
        public void GivenIHaveAnEventingUrlAt(string url)
        {
            this.url = url;
            publisher = SpecflowGlobal.InProcessHost.Get<IEventPublisher>();
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
            HttpClient client = new HttpClient();
            this.httpResponseMessage = client.GetAsync(url + subscriptionId).Result;
        }

        [Given(@"I wait for the subscription to be created at'(.*)'")]
        public void GivenIWaitForTheSubscriptionToBeCreatedAt(string resourceLocation)
        {
            if (!WaitForSubscriptionToBeCreated(resourceLocation))
                throw new TimeoutException("Subscription was not created within expected time");
        }

        private bool WaitForSubscriptionToBeCreated(string resourceLocation)
        {
            HttpClient client = new HttpClient();
            for (int i = 0; i < 20; i++)
            {
                this.httpResponseMessage = client.GetAsync(resourceLocation + subscriptionId).Result;
                if (this.httpResponseMessage.IsSuccessStatusCode)
                    return true;

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            return false;
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
            Console.WriteLine("Subscribed with subscription id " + subscriptionId);
            var client = new SubscriptionClient(url);
            client.Subscribe(subscriptionId).Wait();
        }

        [Given(@"I have subscribed to an atom feed with a subscription Id of '(.*)'")]
        public void GivenIHaveSubscribedToAnAtomFeedWithASubscriptionIdOf(string idToSubscribeWith)
        {
            this.subscriptionId = idToSubscribeWith;
            var client = new SubscriptionClient(url);
            client.Subscribe(subscriptionId).Wait();
        }

        [Given(@"'(.*)' events are raised within my domain")]
        [When(@"'(.*)' events are raised within my domain")]
        public void WhenEventsAreRaisedWithinMyDomain(int numberOfEventsToRaise)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            for (int i = 0; i < numberOfEventsToRaise; i++)
            {
                Console.WriteLine("Raising event " + DateTime.Now.ToString("dd/mm/yy hh:mm:ss ffff"));
                publisher.PublishMessage(new DummyEvent(i.ToString()));
            }
        }

        [When(@"I cancel the subscription")]
        public void WhenICancelTheSubscription()
        {
            var client = new SubscriptionClient(url);
            client.Unsubscribe(subscriptionId).Wait();
        }


        [When(@"'(.*)' events are raised on a different node")]
        public void WhenEventsAreRaisedOnADifferentNode(int numberOfEventsToRaise)
        {
            HttpClient client = new HttpClient();

            for (int i = 0; i < numberOfEventsToRaise; i++)
            {
                HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var result = this.httpResponseMessage = client.PutAsync("http://localhost:3601/events/" + i, content).Result;
                Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
            }
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }


        [Then(@"I should receive a valid atom document with '(.*)' entries from '(.*)'")]
        public void ThenIShouldReceiveAValidAtomDocumentWithEntriesFrom(int numberOfEventsExpected, string atomUrl)
        {
            Assert.AreEqual(numberOfEventsExpected, retrievedFeed.Items.Count());
        }

        [Then(@"I should have an atom document with '(.*)' events")]
        public void ThenIShouldHaveAnAtomDocumentWithEvents(int numberOfEventsExpected)
        {
            Assert.AreEqual(numberOfEventsExpected, retrievedFeed.Items.Count());
        }


        [When(@"I get the feed from '(.*)'")]
        public void WhenIGetTheFeedFrom(string atomUrl)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(200));

            GetFeed(atomUrl);
        }

        private void GetFeed(string atomUrl)
        {
            var atomClient = new AtomClient();
            retrievedFeed = atomClient.GetFeed(atomUrl + subscriptionId).Result;

            Console.WriteLine(atomClient.GetFeedAsString(atomUrl + subscriptionId).Result);
        }


        [Then(@"I should be able to retrieve the earlier document by issuing a GET to its url")]
        public void ThenIShouldBeAbleToRetrieveTheEarlierDocumentByIssuingAGETToItsUrl()
        {
            var atomClient = new AtomClient();
            var url = retrievedFeed.Links.First(x => x.RelationshipType == "prev-archive").Uri.ToString();
            retrievedFeed = atomClient.GetFeed(url).Result;
            Assert.AreEqual(10, retrievedFeed.Items.Count());
        }

        [Then(@"the earlier document should have a link to the new head document")]
        public void ThenTheEarlierDocumentShouldHaveALinkToTheNewHeadDocument()
        {
            var atomClient = new AtomClient();
            var url = retrievedFeed.Links.First(x => x.RelationshipType == "next-archive").Uri.ToString();
            retrievedFeed = atomClient.GetFeed(url).Result;
            Assert.AreEqual(10, retrievedFeed.Items.Count());
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
            Thread.Sleep(TimeSpan.FromMilliseconds(300));

            var atomClient = new AtomClient();
            retrievedFeed = atomClient.GetFeed(atomUrl + subscriptionId).Result;

            Assert.IsTrue(retrievedFeed.Links.Any(x => x.RelationshipType == "prev-archive"));
        }
    }
}
