namespace Eventing.Atom.Document
{
    public class FeedId
    {
        public FeedId(string uuid)
        {
            Id = uuid;
        }

        public string Id { get; }

        public static implicit operator FeedId(string input)
        {
            return new FeedId(input);
        }

        public static implicit operator string(FeedId input)
        {
            return input.Id;
        }
    }
}
