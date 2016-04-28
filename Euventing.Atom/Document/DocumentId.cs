using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class DocumentId
    {
        protected bool Equals(DocumentId other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DocumentId) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        private sealed class IdEqualityComparer : IEqualityComparer<DocumentId>
        {
            public bool Equals(DocumentId x, DocumentId y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Id, y.Id);
            }

            public int GetHashCode(DocumentId obj)
            {
                return (obj.Id != null ? obj.Id.GetHashCode() : 0);
            }
        }

        private static readonly IEqualityComparer<DocumentId> IdComparerInstance = new IdEqualityComparer();

        public static IEqualityComparer<DocumentId> IdComparer => IdComparerInstance;

        public DocumentId(string feedId, long documentIndex)
        {
            DocumentIndex = documentIndex;
            FeedId = feedId;
            Id = $"documentId:{feedId}:{documentIndex}";
        }

        public DocumentId(string documentId)
        {
            Id = documentId;
            var data = documentId.Split(new[] {':'});
            FeedId = data[1];
            DocumentIndex = long.Parse(data[2]);
        }

        public static implicit operator DocumentId(string input)
        {
            return new DocumentId(input);
        }

        public static implicit operator string(DocumentId input)
        {
            return input.Id;
        }

        public DocumentId Subtract(long id)
        {
            return new DocumentId(FeedId, DocumentIndex - id);
        }

        public DocumentId Add(long id)
        {
            return new DocumentId(FeedId, DocumentIndex + id);
        }

        public long DocumentIndex { get; }
        public string FeedId { get; }
        public string Id { get; }
    }
}
