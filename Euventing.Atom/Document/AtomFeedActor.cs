﻿using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;

namespace Euventing.Atom.Document
{
    public class AtomFeedActor : PersistentActor
    {
        private readonly IAtomDocumentActorBuilder builder;
        private FeedId atomFeedId;
        private DocumentId currentFeedHeadDocument;
        private DocumentId lastHeadDocument;
        private string feedTitle;
        private string feedAuthor;
        private int numberOfEventsInCurrentHeadDocument;

        public override string PersistenceId { get; }

        public AtomFeedActor(IAtomDocumentActorBuilder builder)
        {
            Console.WriteLine("Feed actor path is " + Self.Path);
            this.builder = builder;
            PersistenceId = "AtomFeedActor|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        protected override bool ReceiveRecover(object message)
        {
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
            {
                Console.WriteLine("Received null message");
                return true;
            }

            Console.WriteLine("AtomFeedActor ReceiveCommand " + message.GetType());


            try
            {
                ((dynamic)this).Process((dynamic)message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing " + message.GetType() + e.ToString());
            }

            return true;
        }

        private void Process(string message)
        {
            Console.WriteLine(message);
        }

        private void Process(AtomFeedCreationCommand creationCommand)
        {
            var documentId = new DocumentId(Guid.NewGuid().ToString());

            var atomFeedCreated = new AtomFeedCreated(documentId, feedTitle, feedAuthor);

            Persist(atomFeedCreated, MutateInternalState);

            var atomDocument = builder.GetActorRef();
            atomDocument.Tell(new CreateAtomDocumentCommand(
                creationCommand.Title, creationCommand.Author, creationCommand.FeedId, currentFeedHeadDocument, creationCommand.EarlierEventsDocumentId), Self);
        }

        private void Process(EventWithSubscriptionNotificationMessage message)
        {
            var notificationMessage = new EventWithDocumentIdNotificationMessage(currentFeedHeadDocument, message.EventToNotify);
            var atomDocument = builder.GetActorRef();
            atomDocument.Tell(notificationMessage, Self);

            numberOfEventsInCurrentHeadDocument = numberOfEventsInCurrentHeadDocument + 1;
            
            Console.WriteLine("Adding event {0} to doc {1}", numberOfEventsInCurrentHeadDocument, currentFeedHeadDocument.Id);

            if (numberOfEventsInCurrentHeadDocument >= 150)
            {
                var newDocumentId = new DocumentId(Guid.NewGuid().ToString());
                
                atomDocument.Tell(new CreateAtomDocumentCommand(
                    feedTitle, feedAuthor, atomFeedId, newDocumentId, currentFeedHeadDocument), Self);
                lastHeadDocument = currentFeedHeadDocument;
                currentFeedHeadDocument = newDocumentId;

                atomDocument.Tell(new NewDocumentAddedEvent(newDocumentId));

                numberOfEventsInCurrentHeadDocument = 0;
            }
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadIdRequest)
        {
            Sender.Tell(currentFeedHeadDocument, Self);
        }

        private void Process(GetHeadDocumentForFeedRequest getHeadRequest)
        {
            Console.WriteLine("Getting document with id " + currentFeedHeadDocument.Id);

            var atomDocument =
                builder.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(currentFeedHeadDocument)).Result;
            Sender.Tell(atomDocument, Self);
        }

        private void MutateInternalState(AtomFeedDocumentHeadChanged headChanged)
        {
            currentFeedHeadDocument = headChanged.DocumentId;
        }

        private void MutateInternalState(AtomFeedCreated atomFeedCreated)
        {
            currentFeedHeadDocument = atomFeedCreated.DocumentId;
            feedTitle = atomFeedCreated.FeedTitle;
            feedAuthor = atomFeedCreated.FeedAuthor;
        }
    }
}
