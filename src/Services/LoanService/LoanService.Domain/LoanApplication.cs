using BuildingBlocks.Domain;

namespace LoanService.Domain;

public sealed class LoanApplication : AggregateRoot<Guid>
{
    private LoanApplication()
    {
    }

    public LoanApplication(
        string staffId,
        string staffName,
        decimal principalAmount,
        int repaymentMonths,
        string purpose,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(staffId)) throw new ArgumentException("Staff ID is required.", nameof(staffId));
        if (string.IsNullOrWhiteSpace(staffName)) throw new ArgumentException("Staff name is required.", nameof(staffName));
        if (principalAmount <= 0) throw new ArgumentOutOfRangeException(nameof(principalAmount), "Principal must be greater than zero.");
        if (repaymentMonths <= 0) throw new ArgumentOutOfRangeException(nameof(repaymentMonths), "Repayment months must be greater than zero.");
        if (string.IsNullOrWhiteSpace(purpose)) throw new ArgumentException("Purpose is required.", nameof(purpose));

        Id = Guid.NewGuid();
        StaffId = staffId.Trim();
        StaffName = staffName.Trim();
        PrincipalAmount = principalAmount;
        RepaymentMonths = repaymentMonths;
        Purpose = purpose.Trim();
        Status = LoanApplicationStatus.Submitted;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public string StaffId { get; private set; } = string.Empty;
    public string StaffName { get; private set; } = string.Empty;
    public decimal PrincipalAmount { get; private set; }
    public int RepaymentMonths { get; private set; }
    public string Purpose { get; private set; } = string.Empty;
    public LoanApplicationStatus Status { get; private set; }
    public string? ReviewerId { get; private set; }
    public string? DecisionComment { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void StartReview(string reviewerId, DateTime reviewedAtUtc)
    {
        EnsureNotFinalized();

        if (string.IsNullOrWhiteSpace(reviewerId)) throw new ArgumentException("Reviewer ID is required.", nameof(reviewerId));

        ReviewerId = reviewerId.Trim();
        Status = LoanApplicationStatus.UnderReview;
        UpdatedAtUtc = reviewedAtUtc;
    }

    public void Approve(string reviewerId, string? comment, DateTime decidedAtUtc)
    {
        EnsureNotFinalized();

        if (string.IsNullOrWhiteSpace(reviewerId)) throw new ArgumentException("Reviewer ID is required.", nameof(reviewerId));

        ReviewerId = reviewerId.Trim();
        DecisionComment = comment?.Trim();
        Status = LoanApplicationStatus.Approved;
        UpdatedAtUtc = decidedAtUtc;
    }

    public void Reject(string reviewerId, string reason, DateTime decidedAtUtc)
    {
        EnsureNotFinalized();

        if (string.IsNullOrWhiteSpace(reviewerId)) throw new ArgumentException("Reviewer ID is required.", nameof(reviewerId));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Rejection reason is required.", nameof(reason));

        ReviewerId = reviewerId.Trim();
        DecisionComment = reason.Trim();
        Status = LoanApplicationStatus.Rejected;
        UpdatedAtUtc = decidedAtUtc;
    }

    private void EnsureNotFinalized()
    {
        if (Status is LoanApplicationStatus.Approved or LoanApplicationStatus.Rejected)
        {
            throw new InvalidOperationException("Finalized applications cannot transition again.");
        }
    }
}
