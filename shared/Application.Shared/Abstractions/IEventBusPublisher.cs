using Domain.Shared;

namespace Application.Shared.Abstractions;

public interface IEventBusPublisher
{
    Task Publish<T>(T message, CancellationToken ct)
        where T : class;

    Task PublishDomainEvent(IDomainEvent domainEvent, CancellationToken ct);

    // Task PublishIntegrationEvent();
}
