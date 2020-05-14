using System;

namespace DailyHeil.Functions.Infrastructure
{
    public class FunctionConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string HomePage { get; set; }

        public FunctionConfiguration()
        {
            DatabaseConnectionString = GetRequiredEnvironmentVariable("DailyHeilConnectionString");
            HomePage = GetRequiredEnvironmentVariable("HomePageUrl");
        }

        private static string GetRequiredEnvironmentVariable(string key)
        {
            var envVar = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(envVar))
            {
                throw new Exception("Required environment variable not set: " + key);
            }

            return envVar;
        }
    }
}