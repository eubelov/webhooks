using System.Diagnostics;
using MapsterMapper;
using Webhooks.Commands;
using Webhooks.Commands.Workman;
using Webhooks.Engine.ThirdParty.Magnit.Contracts;

namespace Webhooks.Engine.ThirdParty.Builders;

internal abstract class WebhookHttpRequestBuilder : IWebhookHttpRequestBuilder
{
    private readonly IMapper _mapper;

    protected WebhookHttpRequestBuilder(IMapper mapper)
    {
        _mapper = mapper;
    }

    public abstract string CustomerName { get; }

    public object? BuildPayload<T>(T source)
        where T : CommandBase
    {
        return source switch
        {
            NotifyCreated => _mapper.Map<WorkmanCreated>(source),
            NotifyModerationCompleted => _mapper.Map<WorkmanModerationCompleted>(source),
            _ => null
        };
    }

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
            request.Headers.Add("Authorization", subscription.Token);
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
