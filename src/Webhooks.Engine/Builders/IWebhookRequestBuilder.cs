using Webhooks.Commands;

namespace Webhooks.Engine.Builders;

internal interface IWebhookRequestBuilder
{
    string CustomerName { get; }

    HttpRequestMessage BuildRequest<T>(WebhookSubscription subscription, T model)
        where T : CommandBase;
}
