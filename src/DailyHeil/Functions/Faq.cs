using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DailyHeil.Functions.Infrastructure.RazorEngine;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace DailyHeil.Functions.Functions
{
    public class Faq
    {
        private readonly IRazorEngine _razorEngine;

        public Faq(IRazorEngine razorEngine)
        {
            _razorEngine = razorEngine;
        }
        
        [FunctionName("Faq")]
        public async Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest httpRequest, ExecutionContext executionContext)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(await _razorEngine.GetWebPage(executionContext, Constants.Pages.Faq, new { }))
            };
            
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}