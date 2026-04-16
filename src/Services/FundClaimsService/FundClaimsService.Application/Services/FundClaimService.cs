using BuildingBlocks.Application;
using FundClaimsService.Application.Contracts;
using FundClaimsService.Application.Ports;
using FundClaimsService.Domain;

namespace FundClaimsService.Application.Services;

public sealed class FundClaimService
{
    private readonly IClock _clock;
    private readonly IFundClaimRepository _repository;
    private readonly IIntegrationEventPublisher _publisher;

    public FundClaimService(IClock clock, IFundClaimRepository repository, IIntegrationEventPublisher publisher)
    {
        _clock = clock;
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<FundClaimDto> CreateAsync(CreateFundClaimRequest request, CancellationToken cancellationToken = default)
    {
        var fundClaim = new FundClaim(
            request.StaffId,
            request.StaffName,
            request.Amount,
            request.ExpenseType,
            request.Description,
            _clock.UtcNow);

        await _repository.AddAsync(fundClaim, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        await _publisher.PublishAsync(
            "fundclaim.submitted",
            new
            {
                fundClaim.Id,
                fundClaim.StaffId,
                fundClaim.StaffName,
                fundClaim.Amount,
                fundClaim.Status,
                fundClaim.CreatedAtUtc
            },
            cancellationToken);

        return fundClaim.ToDto();
    }

    public async Task<FundClaimDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fundClaim = await _repository.GetByIdAsync(id, cancellationToken);
        return fundClaim?.ToDto();
    }

    public async Task<IReadOnlyList<FundClaimDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var claims = await _repository.ListAsync(cancellationToken);
        return claims.Select(x => x.ToDto()).ToList();
    }

    public async Task<FundClaimDto> ReviewAsync(Guid id, ReviewFundClaimRequest request, CancellationToken cancellationToken = default)
    {
        var fundClaim = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Fund claim '{id}' was not found.");

        switch (request.Decision)
        {
            case FundClaimStatus.UnderReview:
                fundClaim.StartReview(request.ReviewerId, _clock.UtcNow);
                break;
            case FundClaimStatus.Approved:
                fundClaim.Approve(request.ReviewerId, request.Comment, _clock.UtcNow);
                break;
            case FundClaimStatus.Rejected:
                fundClaim.Reject(request.ReviewerId, request.Comment ?? "Rejected during review.", _clock.UtcNow);
                break;
            default:
                throw new InvalidOperationException("Only UnderReview, Approved, and Rejected decisions are supported.");
        }

        await _repository.SaveChangesAsync(cancellationToken);
        await _publisher.PublishAsync(
            "fundclaim.status.changed",
            new
            {
                fundClaim.Id,
                fundClaim.StaffId,
                fundClaim.Status,
                fundClaim.ReviewerId,
                fundClaim.DecisionComment,
                fundClaim.UpdatedAtUtc
            },
            cancellationToken);

        return fundClaim.ToDto();
    }
}
