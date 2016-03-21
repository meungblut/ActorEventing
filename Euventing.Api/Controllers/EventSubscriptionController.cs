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
using Euventing.Core.Subscriptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Euventing.Api.Controllers
{
    public class EventSubscriptionController : ApiController
    {
        private readonly ISubscriptionManager eventShardedSubscriber;
        private JsonSerializerSettings jsonSerializerSettings;

        public EventSubscriptionController(ISubscriptionManager eventShardedSubscriptionManager)
        {
            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            eventShardedSubscriber = eventShardedSubscriptionManager;
        }

        [HttpPut]
        [Route("subscriptions")]
        public HttpResponseMessage Put(SubscribeMessage subscribeMessage)
        {
            var subscriptionMessage = new SubscriptionMessage(new AtomNotificationChannel(), new SubscriptionId(subscribeMessage.SubscriptionId), new AllEventMatcher());
            eventShardedSubscriber.CreateSubscription(subscriptionMessage);

            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Content = new StringContent(JsonConvert.SerializeObject(string.Empty, jsonSerializerSettings), Encoding.UTF8, "application/vnd.tesco.eventSubscription+json");
            return response;
        }

        [HttpGet]
        [Route("subscriptions/{subscriptionId}")]
        public async Task<HttpResponseMessage> Get([FromUri] string subscriptionId)
        {
            var subscription = await eventShardedSubscriber.GetSubscriptionDetails(new SubscriptionQuery(new SubscriptionId(subscriptionId)));

            if (subscription is NullSubscription)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(subscription, jsonSerializerSettings), Encoding.UTF8, "application/vnd.tesco.eventSubscription+json");
            return response;
        }

        [HttpDelete]
        [Route("subscriptions/{subscriptionId}")]
        public async Task<HttpResponseMessage> Delete([FromUri] string subscriptionId)
        {
            var subscription = await eventShardedSubscriber.GetSubscriptionDetails(new SubscriptionQuery(new SubscriptionId(subscriptionId)));

            if (subscription is NullSubscription)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            eventShardedSubscriber.DeleteSubscription(new DeleteSubscriptionMessage(new SubscriptionId(subscriptionId)));

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(subscription, jsonSerializerSettings), Encoding.UTF8, "application/vnd.tesco.eventSubscription+json");
            return response;
        }
    }
}
