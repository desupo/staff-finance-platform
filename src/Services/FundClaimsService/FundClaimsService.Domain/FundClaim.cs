using BuildingBlocks.Domain;

namespace FundClaimsService.Domain;

public sealed class FundClaim : AggregateRoot<Guid>
{
    private FundClaim()
    {
    }

    public FundClaim(
        string staffId,
        string staffName,
        decimal amount,
        string expenseType,
        string description,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(staffId)) throw new ArgumentException("Staff ID is required.", nameof(staffId));
        if (string.IsNullOrWhiteSpace(staffName)) throw new ArgumentException("Staff name is required.", nameof(staffName));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Claim amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(expenseType)) throw new ArgumentException("Expense type is required.", nameof(expenseType));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));

        Id = Guid.NewGuid();
        StaffId = staffId.Trim();
        StaffName = staffName.Trim();
        Amount = amount;
        ExpenseType = expenseType.Trim();
        Description = description.Trim();
        Status = FundClaimStatus.Submitted;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public string StaffId { get; private set; } = string.Empty;
    public string StaffName { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string ExpenseType { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public FundClaimStatus Status { get; private set; }
    public string? ReviewerId { get; private set; }
    public string? DecisionComment { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void StartReview(string reviewerId, DateTime reviewedAtUtc)
    {
        EnsureNotFinalized();

        if (string.IsNullOrWhiteSpace(reviewerId)) throw new ArgumentException("Reviewer ID is required.", nameof(reviewerId));

        ReviewerId = reviewerId.Trim();
        Status = FundClaimStatus.UnderReview;
        UpdatedAtUtc = reviewedAtUtc;
    }

    public void Approve(string reviewerId, string? comment, DateTime decidedAtUtc)
    {
        EnsureNotFinalized();

        if (string.IsNullOrWhiteSpace(reviewerId)) throw new ArgumentException("Reviewer ID is required.", nameof(reviewerId));

        ReviewerId = reviewerId.Trim();
        DecisionComment = comment?.Trim();
        Status = FundClaimStatus.Approved;
        UpdatedAtUtc = decidedAtUtc;
    }

    public void Reject(string reviewerId, string reason, DateTime decidedAtUtc)
    {
        EnsureNotFinalized();

        if (string.IsNullOrWhiteSpace(reviewerId)) throw new ArgumentException("Reviewer ID is required.", nameof(reviewerId));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required.", nameof(reason));

        ReviewerId = reviewerId.Trim();
        DecisionComment = reason.Trim();
        Status = FundClaimStatus.Rejected;
        UpdatedAtUtc = decidedAtUtc;
    }

    private void EnsureNotFinalized()
    {
        if (Status is FundClaimStatus.Approved or FundClaimStatus.Rejected)
        {
            throw new InvalidOperationException("Finalized claims cannot transition again.");
        }
    }
}
