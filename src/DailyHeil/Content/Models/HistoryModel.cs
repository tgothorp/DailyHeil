using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace DailyHeil.Functions.Content.Models
{
    public class HistoryModel
    {
        public HistoryModel()
        {
            Labels = new List<string>();
            Data = new List<long>();
        }
        
        public List<string> Labels { get; set; }
        public List<long> Data { get; set; }

        public HtmlString LabelsString => new HtmlString(JsonConvert.SerializeObject(Labels));
        public HtmlString DataString => new HtmlString(JsonConvert.SerializeObject(Data));
    }
}