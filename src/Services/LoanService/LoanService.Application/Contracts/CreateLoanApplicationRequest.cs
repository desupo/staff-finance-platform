namespace LoanService.Application.Contracts;

public sealed record CreateLoanApplicationRequest(
    string StaffId,
    string StaffName,
    decimal PrincipalAmount,
    int RepaymentMonths,
    string Purpose);
