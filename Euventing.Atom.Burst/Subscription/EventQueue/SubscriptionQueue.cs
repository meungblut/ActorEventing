﻿using System.Collections.Generic;
using System.Linq;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst.Subscription.EventQueue
{
    public class SubscriptionQueue<T>
    {
        private readonly List<ItemEnvelope<T>> items;

        private Dictionary<FeedId, long> LastNumbers = new Dictionary<FeedId, long>();

        public SubscriptionQueue()
        {
            items = new List<ItemEnvelope<T>>();
        }

        public void Add(T atomEntry, long lastSequenceNr)
        {
            items.Add(new ItemEnvelope<T>(lastSequenceNr, atomEntry));
        }

        public void RemoveItemsWithIndexLowerThan(long sequenceNumber)
        {
            items.RemoveAll(x => x.ItemSequenceNumber < sequenceNumber);
        }

        public List<ItemEnvelope<T>> Get(int maxEventsToSend, long lastNumberProcessed, FeedId requester)
        {
            return items.Where(x => x.ItemSequenceNumber > lastNumberProcessed).Take(maxEventsToSend).ToList();
        }
    }
}