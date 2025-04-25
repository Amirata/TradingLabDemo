using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradingJournal.Infrastructure.Data.Configurations;

public class TradingTechnicConfiguration : IEntityTypeConfiguration<TradingTechnic>
{
    public void Configure(EntityTypeBuilder<TradingTechnic> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(t => t.Id).HasConversion(
            tradingTechnicId => tradingTechnicId.Value,
            dbId => TradingTechnicId.Of(dbId));

        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();

        builder.Property(t => t.Description).HasMaxLength(5000).IsRequired();

        builder.HasMany(t => t.Images)
            .WithOne()
            .HasForeignKey(p => p.TradingTechnicId);
    }
}