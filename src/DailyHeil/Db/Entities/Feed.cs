namespace DailyHeil.Functions.Db.Entities
{
    public class Feed : BaseEntity
    {
        public string FeedUrl { get; set; }
        
        public Newspaper Newspaper { get; set; }
        public long NewspaperId { get; set; }
    }
}