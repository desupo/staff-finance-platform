using FundClaimsService.Domain;

namespace FundClaimsService.Application.Contracts;

public static class FundClaimMappings
{
    public static FundClaimDto ToDto(this FundClaim fundClaim) =>
        new(
            fundClaim.Id,
            fundClaim.StaffId,
            fundClaim.StaffName,
            fundClaim.Amount,
            fundClaim.ExpenseType,
            fundClaim.Description,
            fundClaim.Status,
            fundClaim.ReviewerId,
            fundClaim.DecisionComment,
            fundClaim.CreatedAtUtc,
            fundClaim.UpdatedAtUtc);
}
