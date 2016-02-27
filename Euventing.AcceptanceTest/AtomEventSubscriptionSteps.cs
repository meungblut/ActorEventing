﻿using System;
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
            Assert.AreEqual(expectedBody, body, body);
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
        public void GivenIHaveSubscribedToAnAtomFeedWithASubscriptionIdOf(string subscriptionId)
        {
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
            Thread.Sleep(TimeSpan.FromSeconds(1));

            var atomDocument = GetDocumentStream(atomUrl).Result;

            using (var xmlReader = XmlReader.Create(atomDocument))
            {
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                if (feed != null)
                {
                    Assert.AreEqual(numberOfEventsExpected, feed.Items.Count());
                }
            }
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

            var atomDocument = GetAtomDocument(atomUrl);

            Assert.IsTrue(atomDocument.Contains("prev-archive"));
        }
    }
}