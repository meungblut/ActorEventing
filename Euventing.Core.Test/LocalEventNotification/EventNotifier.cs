using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Euventing.Core.Messages;

namespace Euventing.Core.Test.LocalEventNotification
{
    public class EventNotifier
    {
        public static DomainEvent EventNotifiedWith { get; private set; }

        public static ManualResetEvent EventReceived { get; private set; }

        public void Notify(DomainEvent @event)
        {
            EventNotifiedWith = @event;
            EventReceived.Set();
        }
    }
}
