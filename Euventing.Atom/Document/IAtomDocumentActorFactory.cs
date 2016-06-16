using Akka.Actor;

namespace Eventing.Atom.Document
{
    public interface IAtomDocumentActorFactory
    {
        IActorRef GetActorRef();
    }
}