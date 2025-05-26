namespace Hcs.Domain.Abstractions;

public interface IUnitOfWorks
{
    Task SaveChanges(CancellationToken ct);
}
