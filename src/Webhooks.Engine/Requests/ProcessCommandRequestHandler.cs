namespace Webhooks.Engine.Requests;

internal sealed class ProcessCommandRequestHandler : IRequestHandler<ProcessCommandRequest>
{
    private readonly WebhooksContext _context;
    private readonly ILogger<ProcessCommandRequestHandler> _logger;
    private readonly IWebhookScheduler _webhookScheduler;
    private readonly IEnumerable<IWebhookPayloadMapper> _mappers;

    public ProcessCommandRequestHandler(
        WebhooksContext context,
        IWebhookScheduler webhookScheduler,
        IEnumerable<IWebhookPayloadMapper> mappers,
        ILogger<ProcessCommandRequestHandler> logger)
    {
        _context = context;
        _mappers = mappers;
        _logger = logger;
        _webhookScheduler = webhookScheduler;
    }

    public async ValueTask<Unit> Handle(ProcessCommandRequest request, CancellationToken token)
    {
        var command = request.Command;
        _logger.LogTrace("Processing command {CommandType}", command.CommandType);
        var subscribers = await GetSubscribers(command.CommandType, token);
        await ScheduleWebhooks(subscribers, command, token);

        return Unit.Value;
    }

    private async Task ScheduleWebhooks(
        List<WebhookSubscription> subscribers,
        CommandBase command,
        CancellationToken token)
    {
        _logger.LogTrace("Scheduling hook for {SubscribersCount} subscriber(s)", subscribers.Count);

        var sendTasks = subscribers.Select(
            x =>
            {
                var payload = GetRequestPayload(x, command);
                var payloadJson = JsonSerializer.Serialize(payload);
                return _webhookScheduler.ScheduleSend(x.Id, payloadJson, token);
            });

        await Task.WhenAll(sendTasks);
    }

    private async Task<List<WebhookSubscription>> GetSubscribers(
        CommandType commandType,
        CancellationToken token)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .Where(x => x.IsEnabled && x.Type == commandType)
            .ToListAsync(token);
    }

    private object GetRequestPayload(WebhookSubscription subscription, CommandBase command)
    {
        var builder = _mappers.Single(x => x.CustomerName.EqualsIgnoreCase(subscription.CustomerName));
        return builder.Map(command);
    }
}
