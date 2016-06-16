using System;
using Eventing.Atom.Serialization;
using Eventing.Core.Messages;

namespace Eventing.Atom.Document
{
    public class DomainEventToAtomEntryConverter
    {
        public AtomEntry ConvertDomainEventToAtomEntry(DomainEvent eventToAdd)
        {
            var atomEntry = new AtomEntry();
            var serializer = new JsonEventSerialisation();
            var content = serializer.GetContentWithContentType(eventToAdd);
            atomEntry.Content = content.Content;
            atomEntry.Id = eventToAdd.Id;
            atomEntry.Updated = DateTime.Now;
            atomEntry.Title = content.ContentType;
            return atomEntry;
        }
    }
}
