namespace ICMarketsTest.Core.Events;

/// <summary>
/// Publishes domain events.
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken);
}
