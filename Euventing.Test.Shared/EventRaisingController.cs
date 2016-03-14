using System.Net;
using System.Net.Http;
using System.Web.Http;
using Euventing.Core;
using NLog;

namespace Euventing.Test.Shared
{
    public class EventRaisingController : ApiController
    {
        private readonly IEventPublisher publisher;

        public EventRaisingController(IEventPublisher publisher)
        {
            this.publisher = publisher;
        }

        [HttpPut]
        [Route("events/{eventId}")]
        public HttpResponseMessage Put([FromUri] string eventId)
        {
            LogManager.GetLogger("Log").Info("EventRaisingController publishing event " + eventId);
            publisher.PublishMessage(new DummyEvent(eventId));

            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            return response;
        }
    }
}
