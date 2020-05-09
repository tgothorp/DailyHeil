using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyHeil.Functions.Infrastructure.DailyHeilContextFactory;
using DailyHeil.Functions.Infrastructure.SiteScraperFactory;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DailyHeil.Functions.Functions
{
    /// <summary>
    /// Scrapes articles for mentions of hitler
    ///
    /// CRON 0 */10 * * * * = Every 10 mins
    /// </summary>
    public class SiteScraper
    {
        private readonly ISiteScraperFactory _siteScraperFactory;
        private readonly IDailyHeilContextFactory _contextFactory;
        
        private static readonly ILogger _logger = Log.ForContext<SiteScraper>();

        public SiteScraper(ISiteScraperFactory siteScraperFactory, IDailyHeilContextFactory contextFactory)
        {
            _siteScraperFactory = siteScraperFactory;
            _contextFactory = contextFactory;
        }
        
        [FunctionName("SiteScraper")]
        public async Task RunAsync([TimerTrigger("0 */10 * * * *")] TimerInfo myTimer)
        {
            List<long> articleIds;

            using (var context = _contextFactory.Create())
            {
                articleIds = await context.Articles
                    .AsNoTracking()
                    .Where(x => x.Scraped == false)
                    .Select(x => x.Id)
                    .ToListAsync();
            }
            
            _logger.Information($"Found {articleIds.Count} articles to scrape");

            var batches = articleIds.Select((id, index) => new
            {
                Id = id,
                Index = index
            }).GroupBy(x => x.Index / 10);

            foreach (var batch in batches)
            {
                var articleIdsInBatch = batch.Select(x => x.Id)
                    .ToList();

                await ScrapeArticles(articleIdsInBatch);
            }
            
            _logger.Information($"Finished scraping articles");
        }

        private async Task ScrapeArticles(List<long> articleIds)
        {
            using (var context = _contextFactory.Create())
            {
                var articles = await context.Articles
                    .Include(x => x.Newspaper)
                    .Where(x => articleIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var article in articles)
                {
                    _logger.Information($"Scraping article {article.Title} ({article.Url}) for Newspaper {article.Newspaper.Name}");
                    
                    var scraper = _siteScraperFactory.GetScraper(article.Newspaper);
                    article.MarkAsScraped(await scraper.ScrapeUrl(article.Url));
                }

                await context.SaveChangesAsync();
            }
        }
    }
}