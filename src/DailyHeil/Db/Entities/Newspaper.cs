namespace DailyHeil.Functions.Db.Entities
{
    public class Newspaper : BaseEntity
    {
        public string Name { get; set; }

        public Scraper Scraper { get; set; }
    }

    public enum Scraper
    {
        DailyMail,
    }
}