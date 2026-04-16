using LoanService.Domain;

namespace LoanService.Application.Contracts;

public sealed record ReviewLoanApplicationRequest(
    string ReviewerId,
    LoanApplicationStatus Decision,
    string? Comment);
