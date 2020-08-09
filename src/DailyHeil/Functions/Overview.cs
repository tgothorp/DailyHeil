using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DailyHeil.Functions.Content.Models;
using DailyHeil.Functions.Infrastructure.DailyHeilContextFactory;
using DailyHeil.Functions.Infrastructure.RazorEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DailyHeil.Functions.Functions
{
    public class Overview
    {
        private readonly IRazorEngine _razorEngine;
        private readonly IDailyHeilContextFactory _contextFactory;

        public Overview(IRazorEngine razorEngine, IDailyHeilContextFactory contextFactory)
        {
            _razorEngine = razorEngine;
            _contextFactory = contextFactory;
        }
        
        [FunctionName("Overview")]
        public async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest httpRequest,
            ExecutionContext executionContext)
        {
            using (var context = _contextFactory.Create())
            {
                var today = DateTime.Today;
                var startOfYear = new DateTime(today.Year, 1, 1);
                var daysSinceStartOfYear = Math.Abs((today - startOfYear).Days);

                var totalMentionsThisYear = await context.Articles
                    .Where(x => x.MentionsHitler)
                    .Where(x => x.PublishedDate > startOfYear)
                    .CountAsync();

                var averageMentionsPerDay = Math.Round((decimal)totalMentionsThisYear / (decimal)daysSinceStartOfYear, 2);

                // This will be null if they haven't mentioned since the start of the year
                var mostMentionsInOneDay = await context.Articles
                    .Where(x => x.MentionsHitler)
                    .Where(x => x.PublishedDate > startOfYear)
                    .GroupBy(x => new
                    {
                        x.PublishedDate.Date,
                    })
                    .Select(x => new
                    {
                        Date = x.Key,
                        Count = x.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefaultAsync();
                
                var response = new HttpResponseMessage
                {
                    Content = new StringContent(await _razorEngine.GetWebPage(executionContext, Constants.Pages.Overview, new OverviewModel
                    {
                        AverageMentions = averageMentionsPerDay,
                        TotalMentions = totalMentionsThisYear,
                        MostMentionedCount = mostMentionsInOneDay.Count
                    }))
                };
                
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
        }
    }
}