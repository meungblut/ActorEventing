using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Euventing.Api.Messages;
using Euventing.Atom;
using Euventing.Core;
using Euventing.Core.EventMatching;
using Euventing.Core.Messages;
using Newtonsoft.Json;

namespace Euventing.Api.Controllers
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
        public HttpResponseMessage Put(SubscribeMessage subscribeMessage)
        {
            var subscriptionMessage = new SubscriptionMessage(new AtomNotificationChannel(), new SubscriptionId(subscribeMessage.SubscriptionId), new AllEventMatcher());
            eventSubscriber.CreateSubscription(subscriptionMessage);

            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Content = new StringContent(JsonConvert.SerializeObject(string.Empty), Encoding.UTF8, "application/vnd.tesco.eventSubscription+json");
            return response;
        }

        [HttpGet]
        [Route("events/{subscriptionId}")]
        public async Task<HttpResponseMessage> Get([FromUri] string subscriptionId)
        {
            var subscription = await eventSubscriber.GetSubscriptionDetails(new SubscriptionQuery(new SubscriptionId(subscriptionId)));

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(subscription), Encoding.UTF8, "application/vnd.tesco.eventSubscription+json");
            return response;
        }
    }
}
