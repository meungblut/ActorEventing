namespace Eventing.Core.Messages
{
    public class UserId
    {
        public UserId(string uuid)
        {
            Id = uuid;
        }

        public string Id { get; }
    }
}
