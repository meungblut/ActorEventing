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
        private readonly SingleShardedSubscriptionManager eventSingleShardedSubscriber;
        private JsonSerializerSettings jsonSerializerSettings;

        public EventSubscriptionController(SingleShardedSubscriptionManager eventSingleShardedSubscriptionManager)
        {
            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            eventSingleShardedSubscriber = eventSingleShardedSubscriptionManager;
        }

        [HttpPut]
        [Route("subscriptions")]
        public HttpResponseMessage Put(SubscribeMessage subscribeMessage)
        {
            var subscriptionMessage = new SubscriptionMessage(new AtomNotificationChannel(), new SubscriptionId(subscribeMessage.SubscriptionId), new AllEventMatcher());
            eventSingleShardedSubscriber.CreateSubscription(subscriptionMessage);

            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Content = new StringContent(JsonConvert.SerializeObject(string.Empty, jsonSerializerSettings), Encoding.UTF8, "application/vnd.tesco.eventSubscription+json");
            return response;
        }

        [HttpGet]
        [Route("subscriptions/{subscriptionId}")]
        public async Task<HttpResponseMessage> Get([FromUri] string subscriptionId)
        {
            var subscription = await eventSingleShardedSubscriber.GetSubscriptionDetails(new SubscriptionQuery(new SubscriptionId(subscriptionId)));

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
            var subscription = await eventSingleShardedSubscriber.GetSubscriptionDetails(new SubscriptionQuery(new SubscriptionId(subscriptionId)));

            if (subscription is NullSubscription)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            eventSingleShardedSubscriber.DeleteSubscription(new DeleteSubscriptionMessage(new SubscriptionId(subscriptionId)));

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(subscription, jsonSerializerSettings), Encoding.UTF8, "application/vnd.tesco.eventSubscription+json");
            return response;
        }
    }
}
