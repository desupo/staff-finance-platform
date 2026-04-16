using LoanService.Application.Ports;
using LoanService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Infrastructure.Persistence;

public sealed class LoanApplicationRepository(LoanDbContext dbContext) : ILoanApplicationRepository
{
    public async Task AddAsync(LoanApplication loanApplication, CancellationToken cancellationToken = default)
        => await dbContext.LoanApplications.AddAsync(loanApplication, cancellationToken);

    public Task<LoanApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.LoanApplications.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LoanApplication>> ListAsync(CancellationToken cancellationToken = default)
        => await dbContext.LoanApplications.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
