using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingJournal.Domain.Enums;

namespace TradingJournal.Infrastructure.Data.Configurations;

public class TradingJournalConfiguration : IEntityTypeConfiguration<Domain.Models.TradingPlan>
{
    public void Configure(EntityTypeBuilder<Domain.Models.TradingPlan> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasConversion(
            tradingPlanId => tradingPlanId.Value,
            dbId => TradingPlanId.Of(dbId));

        builder.HasMany(t => t.Technics)
            .WithMany(t => t.TradingPlans)
            .UsingEntity<Dictionary<string, object>>(
                "PlansTechnics",
                j => j.HasOne<TradingTechnic>().WithMany().HasForeignKey("TechnicsId").OnDelete(DeleteBehavior.Restrict),
                j => j.HasOne<TradingPlan>().WithMany().HasForeignKey("TradingPlansId").OnDelete(DeleteBehavior.Cascade));

        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.SelectedDays)
            .HasDefaultValue(DaysOfWeek.None);

        builder.HasMany(t => t.Trades)
            .WithOne(t => t.TradingPlan)
            .HasForeignKey(t => t.TradingPlanId);
    }
}