using Webhooks.Commands;

namespace Webhooks.Engine.Services;

internal interface IWebhooksSender
{
    Task<bool> SendOne<T>(WebhookSubscription receiver, T payload, CancellationToken cancellationToken)
        where T : CommandBase;
}
