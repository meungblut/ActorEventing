namespace Euventing.Atom.Burst.Subscription.EventQueue
{
    public class ItemEnvelope<T>
    {
        public ItemEnvelope(long itemSequenceNumber, T itemToStore)
        {
            ItemSequenceNumber = itemSequenceNumber;
            ItemToStore = itemToStore;
        }

        public long ItemSequenceNumber { get; }

        public T ItemToStore { get; }
    }
}