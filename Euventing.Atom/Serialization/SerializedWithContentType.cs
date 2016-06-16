namespace Eventing.Atom.Serialization
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
