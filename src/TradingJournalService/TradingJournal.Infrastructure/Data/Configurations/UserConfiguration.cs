using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingJournal.Domain.Enums;

namespace TradingJournal.Infrastructure.Data.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id).HasConversion(
            userId => userId.Value,
            dbId => UserId.Of(dbId));

        builder.HasMany(u => u.TradingPlans)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);
        
        builder.HasMany(u => u.TradingTechnic)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);
    }
}
