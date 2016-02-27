using Akka.Actor;

namespace Euventing.Atom.Document
{
    public interface IAtomDocumentActorFactory
    {
        IActorRef GetActorRef();
    }
}