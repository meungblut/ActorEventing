using System.Net;
using System.Net.Http;
using System.Web.Http;
using Euventing.Core;

namespace Euventing.Test.Shared
{
    public class EventRaisingController : ApiController
    {
        private readonly EventPublisher publisher;

        public EventRaisingController(EventPublisher publisher)
        {
            this.publisher = publisher;
        }

        [HttpPut]
        [Route("events/{eventId}")]
        public HttpResponseMessage Put([FromUri] string eventId)
        {
            publisher.PublishMessage(new DummyEvent(eventId));

            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            return response;
        }
    }
}
