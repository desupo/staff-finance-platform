using LoanService.Domain;

namespace LoanService.Application.Ports;

public interface ILoanApplicationRepository
{
    Task AddAsync(LoanApplication loanApplication, CancellationToken cancellationToken = default);
    Task<LoanApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanApplication>> ListAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
