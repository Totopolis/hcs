namespace Domain.Shared;

public interface IDomainEventsAccessor
{
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}
