namespace Webhooks.Engine.Requests;

internal sealed class ProcessCommandRequestHandler : IRequestHandler<ProcessCommandRequest, List<WebhookSchedule>>
{
    private readonly WebhooksContext _context;
    private readonly ILogger<ProcessCommandRequestHandler> _logger;
    private readonly IEnumerable<IWebhookPayloadMapper> _mappers;

    public ProcessCommandRequestHandler(
        WebhooksContext context,
        IEnumerable<IWebhookPayloadMapper> mappers,
        ILogger<ProcessCommandRequestHandler> logger)
    {
        _context = context;
        _mappers = mappers;
        _logger = logger;
    }

    public async ValueTask<List<WebhookSchedule>> Handle(ProcessCommandRequest request, CancellationToken token)
    {
        var command = request.Command;
        _logger.LogTrace("Processing command {CommandType}", command.CommandType);
        var subscribers = await GetSubscribers(command.CommandType, token);
        return ScheduleWebhooks(subscribers, command);
    }

    private List<WebhookSchedule> ScheduleWebhooks(List<WebhookSubscription> subscribers, CommandBase command)
    {
        _logger.LogTrace("Scheduling hook for {SubscribersCount} subscriber(s)", subscribers.Count);

        return subscribers.Select(
            x =>
            {
                var payload = GetRequestPayload(x, command);
                var payloadJson = JsonSerializer.Serialize(payload);
                return new WebhookSchedule(x.Id, payloadJson);
            }).ToList();
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
