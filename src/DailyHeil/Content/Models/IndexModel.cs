using System.Collections.Generic;
using DailyHeil.Functions.Db.Entities;

namespace DailyHeil.Functions.Content.Models
{
    public class IndexModel
    {
        public List<Article> Articles { get; set; }
        public int PreviousMention { get; set; }
    }
}