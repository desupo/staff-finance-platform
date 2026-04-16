using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public sealed class LayeringTests
{
    [Fact]
    public void LoanDomain_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(LoanService.Domain.LoanApplication).Assembly)
            .ShouldNot()
            .HaveDependencyOn("LoanService.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void FundClaimsDomain_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(FundClaimsService.Domain.FundClaim).Assembly)
            .ShouldNot()
            .HaveDependencyOn("FundClaimsService.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
