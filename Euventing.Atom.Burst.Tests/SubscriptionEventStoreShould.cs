using System.Linq;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Burst.Subscription.EventQueue;
using NUnit.Framework;

namespace Euventing.Atom.Burst.Tests
{
    public class SubscriptionEventStoreShould
    {
        private SubscriptionEventStore<string> _queue;

        [SetUp]
        public void ConfigureQueue()
        {
            _queue = new SubscriptionEventStore<string>();
        }

        [Test]
        public void AllowMeToSaveAndRetrieveEvents()
        {
            _queue.Add("hi", 1);

            var retrievedItems = _queue.Get(1);
            Assert.AreEqual("hi", retrievedItems.Events.First());
        }

        [Test]
        public void ReturnMaximumAvailableEventsWhenAskedForMoreEventsThanAreAvailabe()
        {
            _queue.Add("hi", 1);

            var retrievedItems = _queue.Get(2);

            Assert.AreEqual(1, retrievedItems.Events.Count());
        }

        [Test]
        public void ReturnExpectedMinPersistenceNumberHeldForHAndledEvents()
        {
            AddEntries(4);
            var retrievedItems = _queue.Get(2);

            _queue.ConfirmHandled(retrievedItems.EventBatchId);

            Assert.AreEqual(2, _queue.LowestCurrentlyHeldPersistenceId);
        }

        [Test]
        public void ReturnCorrectNumberOfEventsInBatch()
        {
            AddEntries(4);
            var retrievedItems = _queue.Get(2);

            Assert.AreEqual(2, retrievedItems.EventCount);
        }

        [Test]
        public void WaitUntilLowestBatchIsConfirmedBeforeUpdatingMinimumPersistenceIdHeld()
        {
            AddEntries(10);

            var batch1 = _queue.Get(2);
            var batch2 = _queue.Get(2);
            var batch3 = _queue.Get(2);
            var batch4 = _queue.Get(2);

            _queue.ConfirmHandled(batch3.EventBatchId);

            Assert.AreEqual(0, _queue.LowestCurrentlyHeldPersistenceId);

            _queue.ConfirmHandled(batch1.EventBatchId);

            Assert.AreEqual(2, _queue.LowestCurrentlyHeldPersistenceId);

            _queue.ConfirmHandled(batch2.EventBatchId);
            _queue.ConfirmHandled(batch4.EventBatchId);

            Assert.AreEqual(8, _queue.LowestCurrentlyHeldPersistenceId);
        }

        [Test]
        public void ThrowExceptionIfSamePersistenceIdIsAddedTwice()
        {
            _queue.Add("hi", 1);

            Assert.Throws<PersistenceIdAlreadyAddedException>(() => _queue.Add("hi", 1));
        }

        [Test]
        public void ReturnExpectedBatches()
        {
            _queue.Add("hia", 1);
            _queue.Add("hib", 2);
            _queue.Add("hic", 3);
            _queue.Add("hid", 4);

            var retrievedItems = _queue.Get(1);
            Assert.AreEqual("hia", retrievedItems.Events.First());

            retrievedItems = _queue.Get(3);
            Assert.AreEqual("hib", retrievedItems.Events.ElementAt(0));
            Assert.AreEqual("hic", retrievedItems.Events.ElementAt(1));
            Assert.AreEqual("hid", retrievedItems.Events.ElementAt(2));
        }

        [Test]
        public void ReturnExpectedQueueLengths()
        {
            Assert.AreEqual(0, _queue.EventsInQueue);

            AddEntries(4);

            Assert.AreEqual(4, _queue.EventsInQueue);

            var retrievedItems = _queue.Get(1);

            Assert.AreEqual(3, _queue.EventsInQueue);

            retrievedItems = _queue.Get(2);

            Assert.AreEqual(1, _queue.EventsInQueue);

            retrievedItems = _queue.Get(1);

            Assert.AreEqual(0, _queue.EventsInQueue);

        }

        private void AddEntries(int numberToAdd)
        {
            for (int i = 1; i <= numberToAdd; i++)
            {
                _queue.Add("hi" + i.ToString(), i);
            }
        }

        //TODO: Add timeout to event batches to return them to pending batch
    }
}
