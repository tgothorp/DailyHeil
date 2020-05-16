using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;

namespace DailyHeil.Functions
{
    public static class Constants
    {
        public static readonly Func<ExecutionContext, string> TemplateDirectory = executionContext => $"{executionContext.FunctionAppDirectory}\\Content";

        public static Dictionary<Pages, string> Templates = new Dictionary<Pages, string>
        {
            {Pages.Index, "Index.cshtml"},
            {Pages.Faq, "Faq.cshtml"},
            {Pages.History, "History.cshtml"}
        };
        
        public enum Pages
        {
            Index,
            Faq,
            History,
        }
    }
}