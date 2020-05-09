using System.Threading.Tasks;

namespace DailyHeil.Functions.Infrastructure.SiteScraperFactory.SiteScraper
{
    public interface ISiteScraper
    {
        public Task<bool> ScrapeUrl(string url);
    }
}