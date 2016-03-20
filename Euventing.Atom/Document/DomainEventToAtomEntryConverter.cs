using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Atom.Serialization;
using Euventing.Core.Messages;

namespace Euventing.Atom.Document
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
