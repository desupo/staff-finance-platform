using LoanService.Domain;

namespace LoanService.Application.Contracts;

public sealed record LoanApplicationDto(
    Guid Id,
    string StaffId,
    string StaffName,
    decimal PrincipalAmount,
    int RepaymentMonths,
    string Purpose,
    LoanApplicationStatus Status,
    string? ReviewerId,
    string? DecisionComment,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
