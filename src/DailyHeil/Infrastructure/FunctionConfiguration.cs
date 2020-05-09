using System;

namespace DailyHeil.Functions.Infrastructure
{
    public class FunctionConfiguration
    {
        public string DatabaseConnectionString { get; set; }

        public FunctionConfiguration()
        {
            DatabaseConnectionString = GetRequiredEnvironmentVariable("DailyHeilConnectionString");
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