using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class DocumentId
    {
        public DocumentId(string uuid)
        {
            Id = uuid;
        }

        public string Id { get; }
    }
}
