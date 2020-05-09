using System.Collections.Generic;
using DailyHeil.Functions.Db.Entities;
using DailyHeil.Functions.Infrastructure.SiteScraperFactory.SiteScraper;

namespace DailyHeil.Functions.Infrastructure.SiteScraperFactory
{
    public interface ISiteScraperFactory
    {
        ISiteScraper GetScraper(Newspaper newspaper);
    }

    public class SiteScraperFactory : ISiteScraperFactory
    {
        private readonly Dictionary<Scraper, ISiteScraper> _scrapers;
        
        public SiteScraperFactory()
        {
            _scrapers = new Dictionary<Scraper, ISiteScraper>
            {
                {Scraper.DailyMail, new DailyMailScraper()}
            };
        }
        
        public ISiteScraper GetScraper(Newspaper newspaper)
        {
            return _scrapers[newspaper.Scraper];
        }
    }
}