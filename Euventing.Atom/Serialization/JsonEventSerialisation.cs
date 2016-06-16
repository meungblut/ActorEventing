using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eventing.Atom.Serialization
{
    public class JsonEventSerialisation
    {
        private static Dictionary<string, Type> valuesToDeserialise = new Dictionary<string, Type>();

        public SerialisedWithContentType GetContentWithContentType(object objectToSerialise)
        {
            var messageBody = JsonConvert.SerializeObject(objectToSerialise, Formatting.None);

            return new SerialisedWithContentType(messageBody, GetContentType(objectToSerialise));
        }

        private const string ContentTypePrefix = "application/vnd.";
        private const string ContentTypePostfix = "+json";


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
