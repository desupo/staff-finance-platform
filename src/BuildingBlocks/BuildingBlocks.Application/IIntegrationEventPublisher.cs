namespace BuildingBlocks.Application;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TMessage>(string subject, TMessage message, CancellationToken cancellationToken = default);
}
