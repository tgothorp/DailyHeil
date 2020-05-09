using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyHeil.Functions.Db.Entities;
using DailyHeil.Functions.Infrastructure.DailyHeilContextFactory;
using Microsoft.Azure.WebJobs;
using DailyHeil.Functions.Infrastructure.RssReader;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DailyHeil.Functions.Functions
{
    /// <summary>
    /// Reads RSS feeds and records articles
    ///
    /// CRON 0 */5 * * * * = Every 5 mins
    /// </summary>
    public class FeedReader
    {
        private static readonly ILogger _logger = Log.ForContext<FeedReader>();
        
        private readonly IRssReader _rssReader;
        private readonly IDailyHeilContextFactory _contextFactory;

        public FeedReader(IRssReader rssReader, IDailyHeilContextFactory contextFactory)
        {
            _rssReader = rssReader;
            _contextFactory = contextFactory;
        }
        
        [FunctionName("FeedReader")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            List<long> feedIdsToRead;

            using (var context = _contextFactory.Create())
            {
                feedIdsToRead = await context.Feeds
                    .AsNoTracking()
                    .Include(x => x.Newspaper)
                    .Select(x => x.Id)
                    .ToListAsync();
            }

            foreach (var feed in feedIdsToRead)
            {
                await ReadFeed(feed);
            }
        }

        private async Task ReadFeed(long feedId)
        {
            using (var context = _contextFactory.Create())
            {
                var feed = await context.Feeds
                    .Include(x => x.Newspaper)
                    .SingleAsync(x => x.Id == feedId);
                
                _logger.Information($"Reading feed {feed.FeedUrl} for newspaper {feed.Newspaper.Name}");

                var articlesFromFeed = await _rssReader.ReadFeed(feed.FeedUrl);
                var articleHashes = articlesFromFeed.Select(x => x.Hash);

                var existingArticles = await context.Articles
                    .Where(x => articleHashes.Contains(x.Hash))
                    .ToListAsync();

                var articlesToAdd = new List<Article>();
                foreach (var rssItem in articlesFromFeed)
                {
                    if (string.IsNullOrEmpty(rssItem.Title) || string.IsNullOrEmpty(rssItem.Url) || string.IsNullOrEmpty(rssItem.Hash))
                    {
                        _logger.Information($"");
                        continue;
                    }
                    
                    if (existingArticles.Any(x => x.Hash == rssItem.Hash))
                    {
                        _logger.Information($"");
                        continue;
                    }
                    
                    articlesToAdd.Add(new Article(rssItem, feed.Newspaper));
                }
                
                _logger.Information($"{articlesFromFeed.Count} articles were retrieved from feed {feed.FeedUrl} for newspaper {feed.Newspaper}. Of those {articlesToAdd.Count} are new and will be added to the database");
                context.Articles.AddRange(articlesToAdd);
                await context.SaveChangesAsync();
                
                _logger.Information($"Finished reading feed {feed.FeedUrl} for newspaper {feed.Newspaper.Name}");
            }
        }
    }
}