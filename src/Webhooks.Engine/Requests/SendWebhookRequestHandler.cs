namespace Webhooks.Engine.Requests;

internal sealed class SendWebhookRequestHandler : IRequestHandler<SendWebhookRequest, bool>
{
    private readonly WebhooksContext _context;
    private readonly IWebhooksSender _webhooksSender;

    public SendWebhookRequestHandler(WebhooksContext context, IWebhooksSender webhooksSender)
    {
        _context = context;
        _webhooksSender = webhooksSender;
    }

    public async ValueTask<bool> Handle(SendWebhookRequest request, CancellationToken token)
    {
        var subscription = await GetSubscription(request.SubscriptionId, token);
        return await _webhooksSender.Send(subscription, request.PayloadJson, token);
    }

    private async Task<WebhookSubscription> GetSubscription(Guid subscriptionId, CancellationToken token)
    {
        return await _context.Subscriptions
            .AsNoTracking()
            .SingleAsync(x => x.Id == subscriptionId, token);
    }
}
