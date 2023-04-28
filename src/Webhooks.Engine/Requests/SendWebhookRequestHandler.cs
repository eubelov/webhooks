using Webhooks.Commands;
using Webhooks.Engine.Exceptions;

namespace Webhooks.Engine.Requests;

internal sealed class SendWebhookRequestHandler : IRequestHandler<SendWebhookRequest, bool>
{
    private readonly WebhooksContext _context;
    private readonly IWebhooksSender _webhooksSender;
    private readonly ILogger<SendWebhookRequestHandler> _logger;

    public SendWebhookRequestHandler(
        WebhooksContext context,
        IWebhooksSender webhooksSender,
        ILogger<SendWebhookRequestHandler> logger)
    {
        _context = context;
        _webhooksSender = webhooksSender;
        _logger = logger;
    }


    public async Task<bool> Handle(SendWebhookRequest request, CancellationToken cancellationToken)
    {
        var subscription = await GetSubscription(request.SubscriptionId, cancellationToken);
        var @event = JsonSerializer.Deserialize<CommandBase>(request.PayloadJson);
        if (@event == null)
        {
            _logger.LogError("Could not deserialize event. Raw data: {EventJson}", request.PayloadJson);
            throw new InvalidEventException(request.PayloadJson);
        }

        return await _webhooksSender.SendOne(subscription, @event, cancellationToken);
    }

    private async Task<WebhookSubscription> GetSubscription(Guid subscriptionId, CancellationToken cancellationToken)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .SingleAsync(x => x.Id == subscriptionId, cancellationToken);
    }
}
