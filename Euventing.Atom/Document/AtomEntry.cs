using System;

namespace Eventing.Atom.Document
{
    public class AtomEntry
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public DateTime Updated { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
    }
}
