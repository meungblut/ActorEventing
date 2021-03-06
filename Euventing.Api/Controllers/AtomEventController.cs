﻿using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Eventing.Atom;
using Eventing.Atom.Document;
using Eventing.Atom.Serialization;
using Eventing.Core.Messages;

namespace Eventing.Api.Controllers
{
    public class AtomEventController : ApiController
    {
        private readonly IAtomDocumentRetriever atomDocumentRetriever;

        public AtomEventController(IAtomDocumentRetriever retriever)
        {
            atomDocumentRetriever = retriever;
        }

        [HttpGet]
        [Route("events/atom/feed/{subscriptionId}")]
        public async Task<HttpResponseMessage> GetFeed([FromUri] string subscriptionId)
        {
            var subscription = new SubscriptionId(subscriptionId);
            AtomDocument document = await atomDocumentRetriever.GetHeadDocument(subscription);
            return SerialiseDocumentToResonse(document);
        }

        [HttpGet]
        [Route("events/atom/document/{documentId}")]
        public async Task<HttpResponseMessage> GetDocument([FromUri] string documentId)
        {
            AtomDocument document = await atomDocumentRetriever.GetDocument(new DocumentId(documentId));
            return SerialiseDocumentToResonse(document);
        }

        private static HttpResponseMessage SerialiseDocumentToResonse(AtomDocument document)
        {
            var serialiser = new AtomDocumentSerialiser();
            var content = new StringContent(serialiser.Serialise(document, "http://localhost:3600/events/atom/document/"),
                Encoding.UTF8, "application/atom+xml");
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            return response;
        }
    }
}
