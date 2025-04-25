using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingJournal.Domain.Enums;

namespace TradingJournal.Infrastructure.Data.Configurations;
public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id).HasConversion(
            tradeId => tradeId.Value,
            dbId => TradeId.Of(dbId));
        
        // builder.Property(t => t.TradingPlanId).HasConversion(
        //     tradingPlanId => tradingPlanId.Value,
        //     dbId => TradingPlanId.Of(dbId));

        builder.Property(t => t.Symbol).HasDefaultValue(Symbols.EurUsd);
        
        builder.Property(t => t.PositionType).HasDefaultValue(PositionType.Long);
    }
}
