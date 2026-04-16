using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BuildingBlocks.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LoanService.Infrastructure.Messaging;

public sealed class AzureServiceBusPublisher : IIntegrationEventPublisher, IAsyncDisposable
{
    private readonly ILogger<AzureServiceBusPublisher> _logger;
    private readonly ServiceBusOptions _options;
    private readonly ServiceBusClient? _client;

    public AzureServiceBusPublisher(IOptions<ServiceBusOptions> options, ILogger<AzureServiceBusPublisher> logger)
    {
        _logger = logger;
        _options = options.Value;

        if (!string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            _client = new ServiceBusClient(_options.ConnectionString);
        }
    }

    public async Task PublishAsync<TMessage>(string subject, TMessage message, CancellationToken cancellationToken = default)
    {
        if (_client is null)
        {
            _logger.LogInformation("Service Bus is not configured. Skipping publication for subject {Subject}.", subject);
            return;
        }

        await using var sender = _client.CreateSender(_options.QueueName);
        var busMessage = new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(message))
        {
            Subject = subject
        };

        await sender.SendMessageAsync(busMessage, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
        }
    }
}
