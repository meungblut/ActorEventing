using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Atom.Serialization;
using Euventing.Atom.Serialization.Tesco.Eventing.ExternalPublishing.Protocol;
using NUnit.Framework;

namespace Euventing.Atom.Test
{
    public class JsonEventSerialisationShould
    {
        [Test]
        public void SerializeADummyEvent()
        {
            var serializer = new JsonEventSerialisation();
            var data = serializer.GetContentWithContentType(new DummyDomainEvent(Guid.NewGuid().ToString()));
        }
    }
}
