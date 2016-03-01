using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Euventing.Atom;
using Euventing.Atom.Document;
using Euventing.Atom.Serialization;
using Euventing.Core.Messages;

namespace Euventing.Api.Controllers
{
    public class AtomEventController : ApiController
    {
        private readonly AtomDocumentRetriever atomDocumentRetriever;

        public AtomEventController(AtomDocumentRetriever retriever)
        {
            atomDocumentRetriever = retriever;
        }

        [HttpGet]
        [Route("events/atom/feed/{subscriptionId}")]
        public async Task<HttpResponseMessage> GetFeed([FromUri] string subscriptionId)
        {
            var subscription = new SubscriptionId(subscriptionId);
            var document = await atomDocumentRetriever.GetHeadDocument(subscription);
            var serialiser = new AtomDocumentSerialiser();
            StringContent content = new StringContent(serialiser.Serialise(document, "http://localhost:3600/events/atom/document/"), Encoding.UTF8, "application/atom+xml");
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = content;
            return response;
        }

        [HttpGet]
        [Route("events/atom/document/{documentId}")]
        public async Task<HttpResponseMessage> GetDocument([FromUri] string documentId)
        {
            var document = await atomDocumentRetriever.GetDocument(new DocumentId(documentId));
            var serialiser = new AtomDocumentSerialiser();
            StringContent content = new StringContent(serialiser.Serialise(document, "http://localhost:3600/atom/"), Encoding.UTF8, "application/atom+xml");
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = content;
            return response;
        }

    }
}
