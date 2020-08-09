using System;

namespace DailyHeil.Functions.Content.Models
{
    public class OverviewModel
    {
        public long TotalMentions { get; set; }
        public decimal AverageMentions { get; set; }
        public long MostMentionedCount { get; set; }
    }
}