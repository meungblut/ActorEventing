using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Api.Messages
{
    public class SubscribeMessage
    {
        public string SubscriptionId { get; set; }
        public string NotificationChannel { get; set; }
    }
}
