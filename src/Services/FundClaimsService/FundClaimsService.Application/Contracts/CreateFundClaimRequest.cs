namespace FundClaimsService.Application.Contracts;

public sealed record CreateFundClaimRequest(
    string StaffId,
    string StaffName,
    decimal Amount,
    string ExpenseType,
    string Description);
