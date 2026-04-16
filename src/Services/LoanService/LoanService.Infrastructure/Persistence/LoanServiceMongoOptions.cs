namespace LoanService.Infrastructure.Persistence;

public sealed class LoanServiceMongoOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "loan-service";
}
