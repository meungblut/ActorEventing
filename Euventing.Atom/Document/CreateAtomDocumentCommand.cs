using Akka.Actor;

namespace Euventing.Atom.Document
{
    public class CreateAtomDocumentCommand
    {
        public CreateAtomDocumentCommand(
            string title,
            string author,
            DocumentId documentId, IActorRef actorRef)
        {
            Title = title;
            Author = author;
            DocumentId = documentId;
        }

        public string Title { get; }

        public string Author { get; }

        public DocumentId DocumentId { get; set; }

        public IActorRef SubscriptionActorRef { get; set; }
    }
}