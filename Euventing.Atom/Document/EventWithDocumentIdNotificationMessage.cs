using System;
using Eventing.Core.Messages;

namespace Eventing.Atom.Document
{
    public class EventWithDocumentIdNotificationMessage
    {
        public EventWithDocumentIdNotificationMessage(DocumentId atomDocumentId, DomainEvent domainEvent)
        {
            AtomDocumentId = atomDocumentId;
            DomainEvent = domainEvent;
            CreatedDateTime = DateTime.Now;
        }

        public DocumentId AtomDocumentId { get; }
        public DomainEvent DomainEvent { get; }
        public DateTime CreatedDateTime { get; set; }
    }
}
