using LoanService.Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace LoanService.Infrastructure.Persistence;

public sealed class LoanDbContext(DbContextOptions<LoanDbContext> options) : DbContext(options)
{
    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoanApplication>(builder =>
        {
            builder.ToCollection("loanApplications");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.StaffId).IsRequired();
            builder.Property(x => x.StaffName).IsRequired();
            builder.Property(x => x.PrincipalAmount);
            builder.Property(x => x.RepaymentMonths);
            builder.Property(x => x.Purpose).IsRequired();
            builder.Property(x => x.Status);
            builder.Property(x => x.ReviewerId);
            builder.Property(x => x.DecisionComment);
            builder.Property(x => x.CreatedAtUtc);
            builder.Property(x => x.UpdatedAtUtc);
        });
    }
}
