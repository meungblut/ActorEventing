using System;
using Euventing.Atom.Burst.Subscription.EventQueue;
using NUnit.Framework;

namespace Euventing.Atom.Burst.Tests
{
    public class LowestItemTrackerShould
    {
        private LowestItemTracker tracker;

        [SetUp]
        public void Setup()
        {
            tracker = new LowestItemTracker();
        }

        [Test]
        public void ReturnTheHighestValueWhenThereIsOnlyOneMember()
        {
            var code = Guid.NewGuid().ToString();
            tracker.AddEntry(22, code);

            Assert.AreEqual(22, tracker.GetLowestValue());
        }

        [Test]
        public void ReturnTheLowestOfTwoValues()
        {
            var code = Guid.NewGuid().ToString();
            tracker.AddEntry(42, Guid.NewGuid().ToString());
            tracker.AddEntry(40, Guid.NewGuid().ToString());

            Assert.AreEqual(40, tracker.GetLowestValue());
        }

        [Test]
        public void UpdateTheLowestOfTwoValues()
        {
            var code = Guid.NewGuid().ToString();
            var code1 = Guid.NewGuid().ToString();
            tracker.AddEntry(42, code);
            tracker.AddEntry(40, code1);

            Assert.AreEqual(40, tracker.GetLowestValue());

            tracker.AddEntry(50, code1);

            Assert.AreEqual(42, tracker.GetLowestValue());
        }

        [Test]
        public void UpdateTheLowestValueWhenAPollerStopsUpdating()
        {
            var code = Guid.NewGuid().ToString();
            var code1 = Guid.NewGuid().ToString();
            tracker.AddEntry(42, code);
            tracker.AddEntry(40, code1);

            Assert.AreEqual(40, tracker.GetLowestValue());

            tracker.RemoveListener(code1);

            Assert.AreEqual(42, tracker.GetLowestValue());
        }
    }
}
