using FundClaimsService.Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FundClaimsService.Infrastructure.Persistence;

public sealed class FundClaimsDbContext(DbContextOptions<FundClaimsDbContext> options) : DbContext(options)
{
    public DbSet<FundClaim> FundClaims => Set<FundClaim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FundClaim>(builder =>
        {
            builder.ToCollection("fundClaims");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.StaffId).IsRequired();
            builder.Property(x => x.StaffName).IsRequired();
            builder.Property(x => x.Amount);
            builder.Property(x => x.ExpenseType).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Status);
            builder.Property(x => x.ReviewerId);
            builder.Property(x => x.DecisionComment);
            builder.Property(x => x.CreatedAtUtc);
            builder.Property(x => x.UpdatedAtUtc);
        });
    }
}
