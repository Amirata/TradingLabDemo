using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradingJournal.Infrastructure.Data.Configurations;
public class TradingTechnicImageConfiguration : IEntityTypeConfiguration<TradingTechnicImage>
{
    public void Configure(EntityTypeBuilder<TradingTechnicImage> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Path).IsRequired().HasMaxLength(200);
    }
}
