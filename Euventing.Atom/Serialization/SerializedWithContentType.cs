using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Serialization
{
    public class SerialisedWithContentType
    {
        public SerialisedWithContentType(string content, string contentType)
        {
            this.Content = content;
            this.ContentType = contentType;
        }

        public string ContentType { get; private set; }
        public string Content { get; private set; }
    }
}
