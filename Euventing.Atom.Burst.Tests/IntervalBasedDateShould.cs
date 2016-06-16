using System;
using NUnit.Framework;

namespace Eventing.Atom.Burst.Tests
{
    public class IntervalBasedDateShould
    {
        [Test]
        public void ReturnExpectedStartDate()
        {
            Date.CurrentDate = () => new DateTime(2016, 01, 01, 12, 12, 22);
            var intervalDate = new IntervalBasedDate(TimeSpan.FromSeconds(10));
            Assert.AreEqual(new DateTime(2016, 01, 01, 12, 12, 20), intervalDate.Current);
        }

        [Test]
        public void ReturnNewIntervalWhenDateRunsOver()
        {
            Date.CurrentDate = () => new DateTime(2016, 01, 01, 12, 12, 22);
            var intervalDate = new IntervalBasedDate(TimeSpan.FromSeconds(10));
            Date.CurrentDate = () => new DateTime(2016, 01, 01, 12, 12, 32);
            Assert.AreEqual(new DateTime(2016, 01, 01, 12, 12, 30), intervalDate.Current);
        }
    }
}
