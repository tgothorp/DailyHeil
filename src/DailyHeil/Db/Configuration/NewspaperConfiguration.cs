using DailyHeil.Functions.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DailyHeil.Functions.Db.Configuration
{
    public class NewspaperConfiguration : IEntityTypeConfiguration<Newspaper>
    {
        public void Configure(EntityTypeBuilder<Newspaper> builder)
        {
            builder.ToTable("Newspaper");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Scraper)
                .HasConversion(new EnumToStringConverter<Scraper>());
        }
    }
}