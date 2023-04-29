namespace Webhooks.Engine.ThirdParty.Builders;

internal interface IWebhookRequestBuilder
{
    string CustomerName { get; }

    HttpRequestMessage BuildRequest(WebhookSubscription subscription, string payload);
}
