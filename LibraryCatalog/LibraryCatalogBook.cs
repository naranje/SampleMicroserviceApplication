using System;

namespace LibraryCatalog
{
    public class LibraryCatalogBook
    {
        public LibraryCatalogBook(Guid Id, string Title, string Summary)
        {
            this.Id = Id;
            this.Title = Title;
            this.Summary = Summary;
        }

        public LibraryCatalogBook(Guid Id, string Title, string Summary, string SummaryHtml, string Authors, string Url, string SmallImageUrl, string MediumImageUrl, string LargeImageUrl, string Isbn, string Published, string Publisher, string Binding)
        {
            this.Id = Id;
            this.Title = Title;
            this.Summary = Summary;
            this.SummaryHtml = SummaryHtml;
            this.Authors = Authors;
            this.Url = Url;
            this.SmallImageUrl = SmallImageUrl;
            this.MediumImageUrl = MediumImageUrl;
            this.LargeImageUrl = LargeImageUrl;
            this.Isbn = Isbn;
            this.Published = Published;
            this.Publisher = Publisher;
            this.Binding = Binding;
        }

        public Guid Id { get; }
        public string Title { get; }
        public string Summary { get; }
        public string SummaryHtml { get; }
        public string Authors { get; }
        public string Url { get; }
        public string SmallImageUrl { get; }
        public string MediumImageUrl { get; }
        public string LargeImageUrl { get; }
        public string Isbn { get; }
        public string Published { get; }
        public string Publisher { get; }
        public string Binding { get; }

    }

}