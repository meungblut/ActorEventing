using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Core.Messages;

namespace Euventing.Atom.Document
{
    public class EventWithDocumentIdNotificationMessage
    {
        public EventWithDocumentIdNotificationMessage(DocumentId atomDocumentId, DomainEvent domainEvent)
        {
            AtomDocumentId = atomDocumentId;
            DomainEvent = domainEvent;
        }

        public DocumentId AtomDocumentId { get; }
        public DomainEvent DomainEvent { get; }
    }
}
