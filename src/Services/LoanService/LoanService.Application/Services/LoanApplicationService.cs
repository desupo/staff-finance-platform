using BuildingBlocks.Application;
using LoanService.Application.Contracts;
using LoanService.Application.Ports;
using LoanService.Domain;

namespace LoanService.Application.Services;

public sealed class LoanApplicationService
{
    private readonly IClock _clock;
    private readonly ILoanApplicationRepository _repository;
    private readonly IIntegrationEventPublisher _publisher;

    public LoanApplicationService(IClock clock, ILoanApplicationRepository repository, IIntegrationEventPublisher publisher)
    {
        _clock = clock;
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<LoanApplicationDto> CreateAsync(CreateLoanApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var loanApplication = new LoanApplication(
            request.StaffId,
            request.StaffName,
            request.PrincipalAmount,
            request.RepaymentMonths,
            request.Purpose,
            _clock.UtcNow);

        await _repository.AddAsync(loanApplication, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        await _publisher.PublishAsync(
            "loan.application.submitted",
            new
            {
                loanApplication.Id,
                loanApplication.StaffId,
                loanApplication.StaffName,
                loanApplication.Status,
                loanApplication.CreatedAtUtc
            },
            cancellationToken);

        return loanApplication.ToDto();
    }

    public async Task<LoanApplicationDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loanApplication = await _repository.GetByIdAsync(id, cancellationToken);
        return loanApplication?.ToDto();
    }

    public async Task<IReadOnlyList<LoanApplicationDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var applications = await _repository.ListAsync(cancellationToken);
        return applications.Select(x => x.ToDto()).ToList();
    }

    public async Task<LoanApplicationDto> ReviewAsync(Guid id, ReviewLoanApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var loanApplication = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Loan application '{id}' was not found.");

        switch (request.Decision)
        {
            case LoanApplicationStatus.UnderReview:
                loanApplication.StartReview(request.ReviewerId, _clock.UtcNow);
                break;
            case LoanApplicationStatus.Approved:
                loanApplication.Approve(request.ReviewerId, request.Comment, _clock.UtcNow);
                break;
            case LoanApplicationStatus.Rejected:
                loanApplication.Reject(request.ReviewerId, request.Comment ?? "Rejected during review.", _clock.UtcNow);
                break;
            default:
                throw new InvalidOperationException("Only UnderReview, Approved, and Rejected decisions are supported.");
        }

        await _repository.SaveChangesAsync(cancellationToken);
        await _publisher.PublishAsync(
            "loan.application.status.changed",
            new
            {
                loanApplication.Id,
                loanApplication.StaffId,
                loanApplication.Status,
                loanApplication.ReviewerId,
                loanApplication.DecisionComment,
                loanApplication.UpdatedAtUtc
            },
            cancellationToken);

        return loanApplication.ToDto();
    }
}
