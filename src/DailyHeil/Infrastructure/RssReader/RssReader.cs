using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader;

namespace DailyHeil.Functions.Infrastructure.RssReader
{
    public interface IRssReader
    {
        Task<List<RssItem>> ReadFeed(string feedUrl);
    }
    
    public class RssReader : IRssReader
    {
        public async Task<List<RssItem>> ReadFeed(string feedUrl)
        {
            var feed = await FeedReader.ReadAsync(feedUrl);

            return feed.Items
                .Select(item => new RssItem(item.Title, item.Link, item.PublishingDate ?? DateTime.Today))
                .ToList();
        }
    }
}