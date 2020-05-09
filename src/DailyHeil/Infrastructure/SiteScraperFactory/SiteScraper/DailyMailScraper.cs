using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrapySharp.Html;
using ScrapySharp.Network;
using Serilog;

namespace DailyHeil.Functions.Infrastructure.SiteScraperFactory.SiteScraper
{
    public class DailyMailScraper : ISiteScraper
    {
        private static readonly ILogger _logger = Log.ForContext<DailyMailScraper>();
        
        public async Task<bool> ScrapeUrl(string url)
        {
            try
            {
                var browser = new ScrapingBrowser();
                var webpage = await browser.NavigateToPageAsync(new Uri(url));

                var article = webpage.Find("div", By.Class("article-text")).FirstOrDefault();
            
                //var articleMetaData = article.ChildNodes.Where(x => x.Name == "p");
                var articleBodyNode = article.ChildNodes.FirstOrDefault(x => x.GetAttributeValue("itemprop", "") != "" && x.Name == "div"); 
                if (articleBodyNode != null)
                {
                    var articleNodes = articleBodyNode.ChildNodes.Where(x => x.Name == "p");
                    var articleContent = new StringBuilder();
                    foreach (var articleNode in articleNodes)
                    {
                        articleContent.AppendLine(articleNode.InnerText);
                    }

                    var fullArticle = articleContent.ToString().ToLower();
                    return fullArticle.Contains("hitler");
                }
                else
                {
                    _logger.Information($"Could not extract article text for article: {url}!");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Error scraping article {url} : {e.Message}");
                return false;
            }
        }
    }
}