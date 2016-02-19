using Akka.Actor;

namespace Euventing.Atom.Document
{
    public interface IAtomDocumentActorBuilder
    {
        IActorRef GetActorRef();
    }
}