using System;

namespace Eventing.Atom.Document.Actors
{
    public class FeedAlreadyCreatedException : Exception
    {
        public FeedAlreadyCreatedException(string id) : base(id)
        {
        }
    }
}