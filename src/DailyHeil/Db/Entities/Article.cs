using System;
using DailyHeil.Functions.Infrastructure.RssReader;

namespace DailyHeil.Functions.Db.Entities
{
    public class Article : BaseEntity
    {
        protected Article() { }
        
        public Article(RssItem rssItem,
            Newspaper newspaper)
        {
            Title = rssItem.Title;
            Url = rssItem.Url;
            PublishedDate = rssItem.PublishDate;
            Hash = rssItem.Hash;
            Newspaper = newspaper;
        }

        public string Title { get; protected set; }
        public string Url { get; protected set; }

        public DateTime PublishedDate { get; protected set; }

        public string Hash { get; set; }

        public Newspaper Newspaper { get; protected set; }
        public long NewspaperId { get; protected set; }

        public bool Scraped { get; protected set; }
        public bool MentionsHitler { get; protected set; }


        public void MarkAsScraped(bool mentionsHitler)
        {
            Scraped = true;
            MentionsHitler = mentionsHitler;
        }
    }
}