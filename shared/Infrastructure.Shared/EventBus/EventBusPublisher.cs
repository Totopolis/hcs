using Application.Shared.Abstractions;
using Domain.Shared;
using MassTransit;

namespace Infrastructure.Shared.EventBus;

internal class EventBusPublisher : IEventBusPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventBusPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Publish<T>(T message, CancellationToken ct) where T : class
    {
        await _publishEndpoint.Publish(message, ct);
    }

    public async Task PublishDomainEvent(IDomainEvent domainEvent, CancellationToken ct)
    {
        var domainEventType = domainEvent.GetType();

        await _publishEndpoint.Publish(
            message: domainEvent,
            messageType: domainEventType,
            callback: ctx =>
            {
                /*if (_chatScope.IsInitialized)
                {
                    ctx.CorrelationId = _chatScope.CorrelationId;
                    ctx.ConversationId = _chatScope.CorrelationId;
                }*/
            });
    }
}
