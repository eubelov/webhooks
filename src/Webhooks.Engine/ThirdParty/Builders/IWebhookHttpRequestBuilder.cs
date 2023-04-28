using Webhooks.Commands;

namespace Webhooks.Engine.ThirdParty.Builders;

internal interface IWebhookHttpRequestBuilder
{
    string CustomerName { get; }

    object? BuildPayload<T>(T source)
        where T : CommandBase;

    HttpRequestMessage BuildRequest(WebhookSubscription subscription, string payload);
}
