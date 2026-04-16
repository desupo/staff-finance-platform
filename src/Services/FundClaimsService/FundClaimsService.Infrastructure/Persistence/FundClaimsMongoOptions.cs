namespace FundClaimsService.Infrastructure.Persistence;

public sealed class FundClaimsMongoOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "fund-claims-service";
}
