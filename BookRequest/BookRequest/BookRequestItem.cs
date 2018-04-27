using System;

namespace BookRequest.BookRequest
{
    public class BookRequestItem
    {
        public int BookRequestId { get; set; }
        public Guid BookCatalogId { get; }
        public string Title { get; }

        public BookRequestItem(int BookRequestId, Guid BookCatalogId, string Title)
        {
            this.BookRequestId = BookRequestId;
            this.BookCatalogId = BookCatalogId;
            this.Title = Title;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var that = obj as BookRequestItem;
            return BookCatalogId.Equals(that.BookCatalogId);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return BookCatalogId.GetHashCode();
        }
    }
}