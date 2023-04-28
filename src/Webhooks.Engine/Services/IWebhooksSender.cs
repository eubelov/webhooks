namespace Webhooks.Engine.Services;

internal interface IWebhooksSender
{
    Task<bool> Send(WebhookSubscription receiver, string payloadJson, CancellationToken token);
}
