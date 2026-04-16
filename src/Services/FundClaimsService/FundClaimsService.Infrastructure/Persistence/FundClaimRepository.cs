using FundClaimsService.Application.Ports;
using FundClaimsService.Domain;
using Microsoft.EntityFrameworkCore;

namespace FundClaimsService.Infrastructure.Persistence;

public sealed class FundClaimRepository(FundClaimsDbContext dbContext) : IFundClaimRepository
{
    public async Task AddAsync(FundClaim fundClaim, CancellationToken cancellationToken = default)
        => await dbContext.FundClaims.AddAsync(fundClaim, cancellationToken);

    public Task<FundClaim?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.FundClaims.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<FundClaim>> ListAsync(CancellationToken cancellationToken = default)
        => await dbContext.FundClaims.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
