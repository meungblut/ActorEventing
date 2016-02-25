using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Euventing.Core;
using Euventing.Core.Messages;
using Newtonsoft.Json;

namespace Euventing.Api
{
    public class EventSubscriptionController : ApiController
    {
        private readonly SubscriptionManager eventSubscriber;

        public EventSubscriptionController(SubscriptionManager eventSubscriptionManager)
        {
            eventSubscriber = eventSubscriptionManager;
        }

        [HttpPut]
        [Route("events")]
        public HttpResponseMessage Put(SubscriptionMessage subscriptionMessage)
        {
            eventSubscriber.CreateSubscription(
              subscriptionMessage);

            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Content = new StringContent(JsonConvert.SerializeObject(string.Empty), Encoding.UTF8, "application/vnd.tesco.eventSubscription+json");
            return response;
        }
    }
}
