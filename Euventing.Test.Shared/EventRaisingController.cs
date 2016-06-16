using System.Net;
using System.Net.Http;
using System.Web.Http;
using Eventing.Core;
using NLog;

namespace Eventing.Test.Shared
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

        [HttpPut]
        [Route("events/multi/{numberOfEvents}")]
        public HttpResponseMessage PutMany([FromUri] string numberOfEvents)
        {
            for (int i = 0; i < int.Parse(numberOfEvents); i++)
            {
                LogManager.GetLogger("Log").Info("EventRaisingController publishing event " + i);
                publisher.PublishMessage(new DummyEvent(i.ToString()));
            }


            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            return response;
        }
    }
}
