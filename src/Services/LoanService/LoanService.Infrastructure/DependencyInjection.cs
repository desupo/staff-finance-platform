using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure;
using LoanService.Application.Ports;
using LoanService.Application.Services;
using LoanService.Infrastructure.Messaging;
using LoanService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace LoanService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLoanService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoanServiceMongoOptions>(configuration.GetSection(LoanServiceMongoOptions.SectionName));
        services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.SectionName));

        var mongoOptions = configuration.GetSection(LoanServiceMongoOptions.SectionName).Get<LoanServiceMongoOptions>()
            ?? new LoanServiceMongoOptions();

        services.AddDbContext<LoanDbContext>(options =>
        {
            var client = new MongoClient(mongoOptions.ConnectionString);
            options.UseMongoDB(client, mongoOptions.DatabaseName);
        });

        services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
        services.AddScoped<LoanApplicationService>();
        services.AddSingleton<IClock, UtcClock>();
        services.AddSingleton<IIntegrationEventPublisher, AzureServiceBusPublisher>();

        return services;
    }
}
