using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DailyHeil.Functions.Content.Models;
using DailyHeil.Functions.Infrastructure.DailyHeilContextFactory;
using DailyHeil.Functions.Infrastructure.RazorEngine;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DailyHeil.Functions.Functions
{
    public class Index
    {
        private readonly IRazorEngine _razorEngine;
        private readonly IDailyHeilContextFactory _contextFactory;

        public Index(IRazorEngine razorEngine, IDailyHeilContextFactory contextFactory)
        {
            _razorEngine = razorEngine;
            _contextFactory = contextFactory;
        }
        
        [FunctionName("Index")]
        public async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest httpRequest,
            ExecutionContext executionContext)
        {
            using (var context = _contextFactory.Create())
            {
                var today = DateTime.Today;

                var articlesMentioningHitler = await context.Articles
                    .Where(x => x.MentionsHitler)
                    .Where(x => x.PublishedDate.Date == today)
                    .ToListAsync();

                var lastMention = await context.Articles
                    .Where(x => x.MentionsHitler)
                    .OrderByDescending(x => x.PublishedDate)
                    .FirstOrDefaultAsync();

                var lastMentionDaysAgo = 0;
                if (lastMention != null)
                {
                    lastMentionDaysAgo = (today - lastMention.PublishedDate.Date).Days;
                }

                var response = new HttpResponseMessage
                {
                    Content = new StringContent(await _razorEngine.GetWebPage(executionContext, Constants.Pages.Index, new IndexModel
                    {
                        Articles = articlesMentioningHitler,
                        PreviousMention = lastMentionDaysAgo
                    }))
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
        }
    }
}