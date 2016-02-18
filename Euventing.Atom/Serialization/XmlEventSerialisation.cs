﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Serialization
{
    namespace Tesco.Eventing.ExternalPublishing.Protocol
    {
        using System.IO;
        using System.Xml;
        using System.Xml.Serialization;

        public class XmlEventSerialisation
        {
            public SerialisedWithContentType GetContentWithContentType(object objectToSerialise)
            {

                    var emptyNamepsaces = new XmlSerializerNamespaces(new[] {XmlQualifiedName.Empty});
                    var serializer = new XmlSerializer(objectToSerialise.GetType());
                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.OmitXmlDeclaration = true;

                    string xmlContent;
                    using (var stream = new StringWriter())
                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        serializer.Serialize(writer, objectToSerialise, emptyNamepsaces);
                        xmlContent = stream.ToString();
                    }

                    return
                        new SerialisedWithContentType(
                            xmlContent,
                            GetContentType(objectToSerialise));

            }

            public object GetObject(string messageToDeserialise)
            {
                throw new System.NotImplementedException();
            }


            private const string ContentTypePrefix = "application/vnd.";
            private const string ContentTypePostfix = "+xml";


            protected string GetContentType(object objectToSerialise)
            {
                var typeNamespace = objectToSerialise.GetType().Namespace;
                var splitNamespaces = typeNamespace.Split('.');
                var firstTwopartsOfNamespace =
                    (splitNamespaces[0] + "." + splitNamespaces[1] + "." + objectToSerialise.GetType().Name).ToLower()
                    + ContentTypePostfix;
                return ContentTypePrefix + firstTwopartsOfNamespace;
            }
        }
    }

}
