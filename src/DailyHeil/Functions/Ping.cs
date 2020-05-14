using System;
using System.Net;
using System.Threading.Tasks;
using DailyHeil.Functions.Infrastructure;
using Microsoft.Azure.WebJobs;
using Serilog;

namespace DailyHeil.Functions.Functions
{
    public class Ping
    {
        private readonly FunctionConfiguration _functionConfiguration;
        
        private static readonly ILogger _logger = Log.ForContext<SiteScraper>();

        public Ping(FunctionConfiguration functionConfiguration)
        {
            _functionConfiguration = functionConfiguration;
        }
        
        /// <summary>
        /// Pings the homepage every 2 mins, this is essentially a hack to keep the razor engine alive for as long as possible
        ///
        /// CRON 0 */2 * * * * = Every 2 mins
        /// </summary>
        [FunctionName("Ping")]
        public async Task RunAsync([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(_functionConfiguration.HomePage);
            request.Method = "GET";
            request.Timeout = 30000;
            
            try
            {
                // As this is a hack we're not too worried about exceptions so we'll just log them
                _logger.Information($"Attempting to ping homepage");
                var response = await request.GetResponseAsync();
                _logger.Information($"Homepage pinged successfully");

            } catch (Exception exception)
            {
                _logger.Information($"Exception when attempting to ping homepage: {exception.GetType().Name} : {exception.Message}");
                
            }
            
        }
    }
}