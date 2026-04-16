using FundClaimsService.Domain;

namespace FundClaimsService.Application.Contracts;

public sealed record ReviewFundClaimRequest(
    string ReviewerId,
    FundClaimStatus Decision,
    string? Comment);
