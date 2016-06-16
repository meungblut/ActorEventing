using System;
using System.Collections.Generic;
using System.Linq;
using Eventing.Atom.Document;

namespace Eventing.Atom.Burst.Subscription.EventQueue
{
    public class SubscriptionQueue<T>
    {
        private readonly List<ItemEnvelope<T>> items;

        private Dictionary<FeedId, long> LastNumbers = new Dictionary<FeedId, long>();

        public SubscriptionQueue()
        {
            items = new List<ItemEnvelope<T>>();
        }

        public void Add(T atomEntry, long sequenceNumber)
        {
            items.Add(new ItemEnvelope<T>(sequenceNumber, DateTime.Now,  atomEntry));
        }

        public void RemoveItemsWithIndexLowerThan(long sequenceNumber)
        {
            items.RemoveAll(x => x.ItemSequenceNumber < sequenceNumber);
        }

        public List<ItemEnvelope<T>> Get(int maxEventsToSend, long lastNumberProcessed, FeedId requester)
        {
            return items.Where(x => x.ItemSequenceNumber > lastNumberProcessed).Take(maxEventsToSend).ToList();
        }

        public List<ItemEnvelope<T>> Get(int maxEventsToSend, DateTime earliestEventToSend)
        {
            return items.Where(x => x.RaisedDate > earliestEventToSend).Take(maxEventsToSend).ToList();
        }
    }
}
