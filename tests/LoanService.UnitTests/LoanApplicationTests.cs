using FluentAssertions;
using LoanService.Domain;

namespace LoanService.UnitTests;

public sealed class LoanApplicationTests
{
    [Fact]
    public void Constructor_ShouldCreateSubmittedLoanApplication()
    {
        var now = new DateTime(2026, 4, 16, 12, 0, 0, DateTimeKind.Utc);

        var loanApplication = new LoanApplication("STF-001", "Ada Lovelace", 5000, 12, "Laptop purchase", now);

        loanApplication.Status.Should().Be(LoanApplicationStatus.Submitted);
        loanApplication.StaffId.Should().Be("STF-001");
        loanApplication.CreatedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Approve_ShouldMoveToApprovedStatus()
    {
        var loanApplication = new LoanApplication("STF-001", "Ada Lovelace", 5000, 12, "Laptop purchase", DateTime.UtcNow);

        loanApplication.Approve("MGR-001", "Budget available", DateTime.UtcNow);

        loanApplication.Status.Should().Be(LoanApplicationStatus.Approved);
        loanApplication.ReviewerId.Should().Be("MGR-001");
    }

    [Fact]
    public void Reject_AfterApproval_ShouldThrow()
    {
        var loanApplication = new LoanApplication("STF-001", "Ada Lovelace", 5000, 12, "Laptop purchase", DateTime.UtcNow);
        loanApplication.Approve("MGR-001", "Approved", DateTime.UtcNow);

        var action = () => loanApplication.Reject("MGR-002", "Late issue", DateTime.UtcNow);

        action.Should().Throw<InvalidOperationException>();
    }
}
