using BuildingBlocks.Services;
using BuildingBlocks.Utilities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TradingJournal.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(ICurrentSessionProvider currentSessionProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context as ApplicationDbContext);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context as ApplicationDbContext);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(ApplicationDbContext? context)
    {
        if (context == null) return;

        // مدیریت موجودیت‌ها برای اضافه یا ویرایش
        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentSessionProvider.GetUserId();
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified ||
                entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedBy = currentSessionProvider.GetUserId();
                entry.Entity.LastModified = DateTime.UtcNow;
            }
        }

        // فراخوانی فانکشن حذف فایل‌ها برای کاربران حذف ‌شده
        DeleteUserFiles(context);
    }


    private void DeleteUserFiles(ApplicationDbContext context)
    {
        var deletedUsers = context.ChangeTracker.Entries<User>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        if (deletedUsers.Any())
        {
            var imageUpload = new ImageUpload("TechnicImages");
            foreach (var user in deletedUsers)
            {
                var tradingTechnics = context.TradingTechnics //.AsNoTracking()
                    .Include(t => t.Images)
                    .Where(t => t.UserId == user.Id)
                    .ToList();

                foreach (var tradingTechnic in tradingTechnics)
                {
                    foreach (var image in tradingTechnic.Images)
                    {
                        imageUpload.RemoveImage(image.Path);
                    }
                    
                    // حذف رکوردهای PlansTechnics قبل از TradingTechnic
                    // حذف رکوردهای PlansTechnics با استفاده از Guid
                    var plansTechnics = context.Set<Dictionary<string, object>>("PlansTechnics")
                        .Where(pt => (Guid)pt["TechnicsId"] == tradingTechnic.Id.Value) // تبدیل به Guid
                        .ToList();
                    context.Set<Dictionary<string, object>>("PlansTechnics").RemoveRange(plansTechnics);

                    // حالا TradingTechnic رو حذف کن (اگه Cascade نیست)
                    //context.TradingTechnics.Remove(technic);
                }
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}