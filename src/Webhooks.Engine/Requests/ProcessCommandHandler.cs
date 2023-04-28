using Webhooks.Commands;
using Webhooks.Commands.Enums;
using Webhooks.Engine.Extensions;
using Webhooks.Engine.Infrastructure.MessageBus;
using Webhooks.Engine.ThirdParty.Builders;

namespace Webhooks.Engine.Requests;

internal sealed class ProcessCommandHandler : IRequestHandler<ProcessCommandRequest>
{
    private readonly WebhooksContext _context;
    private readonly ILogger<ProcessCommandHandler> _logger;
    private readonly IRabbitMqPublisher _publisher;
    private readonly IEnumerable<IWebhookHttpRequestBuilder> _requestBuilders;

    public ProcessCommandHandler(
        WebhooksContext context,
        IRabbitMqPublisher publisher,
        IEnumerable<IWebhookHttpRequestBuilder> requestBuilders,
        ILogger<ProcessCommandHandler> logger)
    {
        _context = context;
        _publisher = publisher;
        _requestBuilders = requestBuilders;
        _logger = logger;
    }

    public async Task Handle(ProcessCommandRequest request, CancellationToken token)
    {
        var command = request.Command;
        _logger.LogTrace("Processing command {CommandType}", command.CommandType);
        var subscribers = await GetSubscribers(command.CommandType, token);
        await ScheduleWebhooks(subscribers, command, token);
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
                return _publisher.Send(new SendWebhookCommand(x.Id, payloadJson), token);
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

    private object? GetRequestPayload(WebhookSubscription subscription, CommandBase command)
    {
        var builder = _requestBuilders.Single(x => x.CustomerName.EqualsIgnoreCase(subscription.CustomerName));
        return builder.BuildPayload(command);
    }
}
