using DailyHeil.Functions.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyHeil.Functions.Db.Configuration
{
    public class FeedConfiguration : IEntityTypeConfiguration<Feed>
    {
        public void Configure(EntityTypeBuilder<Feed> builder)
        {
            builder.ToTable("Feed");
            builder.HasKey(x => x.Id);
        }
    }
}