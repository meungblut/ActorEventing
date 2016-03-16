using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class ConfigurableAtomDocumentSettings : IAtomDocumentSettings
    {
        public ConfigurableAtomDocumentSettings(int numberOfEventsPerDocument)
        {
            NumberOfEventsPerDocument = numberOfEventsPerDocument;
        }

        public int NumberOfEventsPerDocument { get; }
    }
}
