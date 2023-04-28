using System.Diagnostics;
using MapsterMapper;
using Webhooks.Commands;
using Webhooks.Commands.Workman;
using Webhooks.Engine.Contacts.Magnit;

namespace Webhooks.Engine.Builders;

internal abstract class WebhookRequestBuilder : IWebhookRequestBuilder
{
    private readonly IMapper _mapper;

    protected WebhookRequestBuilder(IMapper mapper)
    {
        _mapper = mapper;
    }

    public abstract string CustomerName { get; }

    public HttpRequestMessage BuildRequest<T>(WebhookSubscription subscription, T model)
        where T : CommandBase
    {
        Debug.Assert(subscription.CustomerName == CustomerName);
        var request = ConfigureHttpRequest(subscription, model);
        ConfigureAuth(subscription, request);
        return request;
    }

    protected virtual void ConfigureAuth(WebhookSubscription subscription, HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(subscription.Token))
        {
            request.Headers.Add("Authorization", subscription.Token);
        }
    }

    protected virtual HttpRequestMessage ConfigureHttpRequest<TSource>(WebhookSubscription subscription, TSource model)
        where TSource : CommandBase
    {
        var payload = Map(model);
        HttpRequestMessage request = new()
        {
            RequestUri = new(subscription.Url, UriKind.Absolute),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        return request;
    }

    private object Map<T>(T source)
        where T : CommandBase
    {
        return source switch
        {
            NotifyCreated => _mapper.Map<WorkmanCreated>(source),
            NotifyModerationCompleted => _mapper.Map<WorkmanModerationCompleted>(source),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}
