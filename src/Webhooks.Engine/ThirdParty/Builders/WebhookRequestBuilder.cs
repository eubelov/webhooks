using System.Diagnostics;

namespace Webhooks.Engine.ThirdParty.Builders;

internal abstract class WebhookRequestBuilder : IWebhookRequestBuilder
{
    public abstract string CustomerName { get; }

    public HttpRequestMessage BuildRequest(WebhookSubscription subscription, string payloadJson)
    {
        Debug.Assert(subscription.CustomerName == CustomerName);
        var request = CreateHttpRequest(subscription, payloadJson);
        return ConfigureAuth(subscription, request);
    }

    protected virtual HttpRequestMessage ConfigureAuth(WebhookSubscription subscription, HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(subscription.Token))
        {
            request.Headers.Add("Authorization", $"Bearer {subscription.Token}");
        }

        return request;
    }

    protected virtual HttpRequestMessage CreateHttpRequest(WebhookSubscription subscription, string payloadJson)
    {
        return new()
        {
            RequestUri = new(subscription.Url, UriKind.Absolute),
            Method = HttpMethod.Post,
            Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
        };
    }
}
