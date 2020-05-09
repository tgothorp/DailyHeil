using System.Threading.Tasks;
using DailyHeil.Functions.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace DailyHeil.Functions.Functions
{
    public class Redirector
    {
        private readonly FunctionConfiguration _functionConfiguration;

        public Redirector(FunctionConfiguration functionConfiguration)
        {
            _functionConfiguration = functionConfiguration;
        }
        
        [FunctionName("Redirector")]
        public IActionResult RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest httpRequest)
        {
            string path = (string) httpRequest.RouteValues["path"];
            
            if (string.IsNullOrEmpty(path) || path.ToLower() == "index")
                return new RedirectResult($"{_functionConfiguration.Domain}/Index", true);
            
            if (path.ToLower() == "faq")
                return new RedirectResult($"{_functionConfiguration.Domain}/Faq", true);
            
            return new RedirectResult($"{_functionConfiguration.Domain}/Index", true);
        }
    }
}