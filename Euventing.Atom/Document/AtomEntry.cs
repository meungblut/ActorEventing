using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class AtomEntry
    {
        public DocumentId DocumentId { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public DateTime Updated { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
    }
}
