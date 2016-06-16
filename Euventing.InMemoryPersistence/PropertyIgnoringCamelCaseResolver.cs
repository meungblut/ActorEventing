using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Eventing.InMemoryPersistence
{
    public class PropertyIgnoringCamelCaseResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member,
                                            MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(Akka.Actor.Nobody) && property.PropertyName == "provider" ||
                property.DeclaringType == typeof(Akka.Actor.FutureActorRef) && property.PropertyName == "provider" ||
                property.DeclaringType == typeof(Akka.Routing.NoRouter) && property.PropertyName == "routerDispatcher")
            {
                property.ShouldSerialize = p => false;
            }

            if (property.PropertyName == "sender")
                //&& property.DeclaringType.GetInterfaces().Contains(typeof(Akka.Actor.IActorRef)))
            {
                Console.WriteLine(property.PropertyType.ToString());
                property.ShouldSerialize = p => false;
            }


            return property;
        }
    }
}
