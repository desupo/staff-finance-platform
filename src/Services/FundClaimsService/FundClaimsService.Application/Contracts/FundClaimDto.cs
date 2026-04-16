using FundClaimsService.Domain;

namespace FundClaimsService.Application.Contracts;

public sealed record FundClaimDto(
    Guid Id,
    string StaffId,
    string StaffName,
    decimal Amount,
    string ExpenseType,
    string Description,
    FundClaimStatus Status,
    string? ReviewerId,
    string? DecisionComment,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
