using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure;
using FundClaimsService.Application.Ports;
using FundClaimsService.Application.Services;
using FundClaimsService.Infrastructure.Messaging;
using FundClaimsService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FundClaimsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFundClaimsService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FundClaimsMongoOptions>(configuration.GetSection(FundClaimsMongoOptions.SectionName));
        services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.SectionName));

        var mongoOptions = configuration.GetSection(FundClaimsMongoOptions.SectionName).Get<FundClaimsMongoOptions>()
            ?? new FundClaimsMongoOptions();

        services.AddDbContext<FundClaimsDbContext>(options =>
        {
            var client = new MongoClient(mongoOptions.ConnectionString);
            options.UseMongoDB(client, mongoOptions.DatabaseName);
        });

        services.AddScoped<IFundClaimRepository, FundClaimRepository>();
        services.AddScoped<FundClaimService>();
        services.AddSingleton<IClock, UtcClock>();
        services.AddSingleton<IIntegrationEventPublisher, AzureServiceBusPublisher>();

        return services;
    }
}
