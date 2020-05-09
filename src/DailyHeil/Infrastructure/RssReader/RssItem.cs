using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DailyHeil.Functions.Infrastructure.RssReader
{
    public class RssItem
    {
        public RssItem(string title, string url, DateTime publishDate)
        {
            Title = new string(title.Where(c => !char.IsControl(c)).ToArray()).Trim();
            Url = new string(url.Where(c => !char.IsControl(c)).ToArray()).Trim();
            PublishDate = publishDate;

            Hash = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Url))).Replace("-","");
        }
        
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime PublishDate { get; set; }

        public string Hash { get; set; }
    }
}