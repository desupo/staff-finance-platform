using FundClaimsService.Domain;

namespace FundClaimsService.Application.Ports;

public interface IFundClaimRepository
{
    Task AddAsync(FundClaim fundClaim, CancellationToken cancellationToken = default);
    Task<FundClaim?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FundClaim>> ListAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
