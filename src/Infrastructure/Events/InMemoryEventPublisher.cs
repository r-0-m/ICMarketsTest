using ICMarketsTest.Core.Events;
using Microsoft.Extensions.Logging;

namespace ICMarketsTest.Infrastructure.Events;

/// <summary>
/// In-process event publisher (logs only).
/// </summary>
public sealed class InMemoryEventPublisher : IEventPublisher
{
    private readonly ILogger<InMemoryEventPublisher> _logger;

    public InMemoryEventPublisher(ILogger<InMemoryEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event published: {EventType}", typeof(TEvent).Name);
        return Task.CompletedTask;
    }
}
