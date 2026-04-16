using LoanService.Domain;

namespace LoanService.Application.Contracts;

public static class LoanApplicationMappings
{
    public static LoanApplicationDto ToDto(this LoanApplication loanApplication) =>
        new(
            loanApplication.Id,
            loanApplication.StaffId,
            loanApplication.StaffName,
            loanApplication.PrincipalAmount,
            loanApplication.RepaymentMonths,
            loanApplication.Purpose,
            loanApplication.Status,
            loanApplication.ReviewerId,
            loanApplication.DecisionComment,
            loanApplication.CreatedAtUtc,
            loanApplication.UpdatedAtUtc);
}
