﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class AtomDocumentSettings : IAtomDocumentSettings
    {
        public int NumberOfEventsPerDocument { get { return 10; } }
    }
}