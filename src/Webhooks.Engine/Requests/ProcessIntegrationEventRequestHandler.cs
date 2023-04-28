using Webhooks.Commands;
using Webhooks.Commands.Enums;
using Webhooks.Engine.Messages;

namespace Webhooks.Engine.Requests;

internal sealed class ProcessIntegrationEventRequestHandler : IRequestHandler<ProcessIntegrationEventRequest>
{
    private readonly WebhooksContext _context;
    private readonly IRabbitMqPublisher _publisher;
    private readonly ILogger<ProcessIntegrationEventRequestHandler> _logger;

    public ProcessIntegrationEventRequestHandler(
        WebhooksContext context,
        IRabbitMqPublisher publisher,
        ILogger<ProcessIntegrationEventRequestHandler> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Handle(ProcessIntegrationEventRequest request, CancellationToken cancellationToken)
    {
        var @event = request.Command;
        _logger.LogTrace("Processing event {EventType}", @event.CommandType);
        var subscribers = await GetSubscribers(@event.CommandType, cancellationToken);
        await ScheduleWebhooks(subscribers, @event, cancellationToken);
    }

    private async Task ScheduleWebhooks(
        List<WebhookSubscription> subscribers,
        CommandBase @event,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Scheduling hook to {SubscribersCount} subscriber(s)", subscribers.Count);

        var tasks = subscribers.Select(
            x => _publisher.Send(
                new SendWebhookCommand(x.Id, JsonSerializer.Serialize(@event)),
                cancellationToken));
        
        await Task.WhenAll(tasks);
    }

    private async Task<List<WebhookSubscription>> GetSubscribers(
        CommandType commandType,
        CancellationToken cancellationToken)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .Where(x => x.IsEnabled && x.Type == commandType)
            .ToListAsync(cancellationToken);
    }
}
