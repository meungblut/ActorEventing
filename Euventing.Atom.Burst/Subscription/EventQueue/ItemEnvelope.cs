using System;

namespace Eventing.Atom.Burst.Subscription.EventQueue
{
    public class ItemEnvelope<T>
    {
        public ItemEnvelope(long itemSequenceNumber, DateTime raisedDate, T itemToStore)
        {
            ItemSequenceNumber = itemSequenceNumber;
            RaisedDate = raisedDate;
            ItemToStore = itemToStore;
        }

        public long ItemSequenceNumber { get; }
        public DateTime RaisedDate { get; }
        public T ItemToStore { get; }
    }
}