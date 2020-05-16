using System;
using System.Collections.Generic;
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
using Microsoft.EntityFrameworkCore;

namespace DailyHeil.Functions.Functions
{
     public class History
    {
        private readonly IDailyHeilContextFactory _contextFactory;
        private readonly IRazorEngine _razorEngine;

        public History(IDailyHeilContextFactory contextFactory, IRazorEngine razorEngine)
        {
            _contextFactory = contextFactory;
            _razorEngine = razorEngine;
        }
        
        [FunctionName("History")]
        public async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest httpRequest, ExecutionContext executionContext)
        {
            using (var context = _contextFactory.Create())
            {
                var fifteenDaysAgo = DateTime.Today.AddDays(-15);

                var hitlerMentionsInLastFifteenDays = await context.Articles
                    .Where(x => x.MentionsHitler == true)
                    .Where(x => x.PublishedDate >= fifteenDaysAgo)
                    .GroupBy(x => x.PublishedDate.Date)
                    .Select(x => new
                    {
                        Date = x.Key,
                        Count = x.Count()
                    }).ToListAsync();

                var model = new HistoryModel();
                
                while (fifteenDaysAgo < DateTime.Today)
                {
                    var hitlerMentionsForDay = hitlerMentionsInLastFifteenDays
                        .SingleOrDefault(x => x.Date == fifteenDaysAgo.Date);
                    
                    model.Labels.Add(fifteenDaysAgo.ToString("dd/MM/yyyy"));
                    model.Data.Add(hitlerMentionsForDay?.Count ?? 0);

                    fifteenDaysAgo = fifteenDaysAgo.AddDays(1);
                }
                
                var response = new HttpResponseMessage
                {
                    Content = new StringContent(await _razorEngine.GetWebPage(executionContext, Constants.Pages.History, model))
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
        }


    }
}