using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Atom.Document;

namespace Euventing.Atom.Test
{
    public class DummyAtomDocumentSettings : IAtomDocumentSettings
    {
        public DummyAtomDocumentSettings(int numberOfEventsPerDocument)
        {
            NumberOfEventsPerDocument = numberOfEventsPerDocument;
        }

        public int NumberOfEventsPerDocument { get; }
    }
}
