using System.Collections.Generic;
using System.Linq;

namespace Euventing.Atom.Burst.Subscription.EventQueue
{
    public class SubscriptionQueue<T>
    {
        private readonly List<ItemEnvelope<T>> items;

        public SubscriptionQueue()
        {
            items = new List<ItemEnvelope<T>>();
        }

        public void Add(T atomEntry, long lastSequenceNr)
        {
            items.Add(new ItemEnvelope<T>(lastSequenceNr, atomEntry));
        }

        public List<ItemEnvelope<T>> Get(int maxEventsToSend, long lastNumberProcessed)
        {
            return items.Where(x => x.ItemSequenceNumber > lastNumberProcessed).Take(maxEventsToSend).ToList();
        }
    }
}
