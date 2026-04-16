using FluentAssertions;
using FundClaimsService.Domain;

namespace FundClaimsService.UnitTests;

public sealed class FundClaimTests
{
    [Fact]
    public void Constructor_ShouldCreateSubmittedFundClaim()
    {
        var now = new DateTime(2026, 4, 16, 12, 0, 0, DateTimeKind.Utc);

        var fundClaim = new FundClaim("STF-002", "Grace Hopper", 250, "Travel", "Taxi reimbursement", now);

        fundClaim.Status.Should().Be(FundClaimStatus.Submitted);
        fundClaim.StaffId.Should().Be("STF-002");
        fundClaim.CreatedAtUtc.Should().Be(now);
    }

    [Fact]
    public void StartReview_ShouldMoveToUnderReview()
    {
        var fundClaim = new FundClaim("STF-002", "Grace Hopper", 250, "Travel", "Taxi reimbursement", DateTime.UtcNow);

        fundClaim.StartReview("FIN-001", DateTime.UtcNow);

        fundClaim.Status.Should().Be(FundClaimStatus.UnderReview);
        fundClaim.ReviewerId.Should().Be("FIN-001");
    }

    [Fact]
    public void Reject_AfterApproval_ShouldThrow()
    {
        var fundClaim = new FundClaim("STF-002", "Grace Hopper", 250, "Travel", "Taxi reimbursement", DateTime.UtcNow);
        fundClaim.Approve("FIN-001", "Approved", DateTime.UtcNow);

        var action = () => fundClaim.Reject("FIN-002", "Should not happen", DateTime.UtcNow);

        action.Should().Throw<InvalidOperationException>();
    }
}
