using Polly;
using Polly.Extensions.Http;
using Webhooks.Commands;
using Webhooks.Commands.Enums;
using Webhooks.Engine.Builders;
using Webhooks.Engine.Notifications;

namespace Webhooks.Engine.Services;

internal sealed class WebhooksSender : IWebhooksSender
{
    private static readonly TimeSpan[] Retries =
    {
        TimeSpan.FromSeconds(0),
        // TimeSpan.FromSeconds(0),
        // TimeSpan.FromSeconds(2),
        // TimeSpan.FromSeconds(4),
        // TimeSpan.FromSeconds(8),
        // TimeSpan.FromSeconds(16),
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMediator _mediator;
    private readonly IEnumerable<IWebhookRequestBuilder> _requestBuilders;
    private readonly ILogger<WebhooksSender> _logger;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public WebhooksSender(
        IHttpClientFactory httpClientFactory,
        IMediator mediator,
        IEnumerable<IWebhookRequestBuilder> requestBuilders,
        ILogger<WebhooksSender> logger)
    {
        _httpClientFactory = httpClientFactory;
        _mediator = mediator;
        _requestBuilders = requestBuilders;
        _logger = logger;
        _retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(
                Retries,
                async (exception, _, retryCount, context) =>
                {
                    var url = (string)context["Url"];
                    var type = (CommandType)context["Type"];
                    var errorText = exception.Exception.Message;

                    await mediator.Publish(new WebhookInvokedNotification(url, false, retryCount, errorText));

                    _logger.LogError(
                        exception.Exception,
                        "Sending hook to {DestinationUrl} of type {WebhookType} failed with error {ErrorDescription}",
                        url, type,
                        errorText);
                });
    }

    public async Task<bool> SendOne<T>(WebhookSubscription receiver, T payload, CancellationToken cancellationToken)
        where T : CommandBase
    {
        var client = _httpClientFactory.CreateClient("web");
        return await ExecuteWebhook(receiver, payload, client, cancellationToken);
    }

    private async Task<bool> ExecuteWebhook<T>(
        WebhookSubscription subscription,
        T payload,
        HttpMessageInvoker client,
        CancellationToken cancellationToken)
        where T : CommandBase
    {
        var attempts = 0;
        var context = new Context { ["Url"] = subscription.Url, ["Type"] = subscription.Type };

        async Task<HttpResponseMessage> Execute(Context _)
        {
            attempts++;
            return await SendData(subscription, payload, client, cancellationToken);
        }

        var result = await _retryPolicy.ExecuteAndCaptureAsync(Execute, context);
        if (result.Outcome is OutcomeType.Successful)
        {
            await _mediator.Publish(
                new WebhookInvokedNotification(subscription.Url, true, attempts),
                cancellationToken);
        }

        return result.Outcome is OutcomeType.Successful;
    }

    private async Task<HttpResponseMessage> SendData<T>(
        WebhookSubscription subscription,
        T payload,
        HttpMessageInvoker client,
        CancellationToken cancellationToken)
        where T : CommandBase
    {
        var builder = _requestBuilders.Single(x =>
            x.CustomerName.Equals(subscription.CustomerName, StringComparison.OrdinalIgnoreCase));

        var request = builder.BuildRequest(subscription, payload);

        _logger.LogTrace("Sending hook to {DestinationUrl} of type {WebhookType}", subscription.Url, subscription.Type);
        return await client.SendAsync(request, cancellationToken);
    }
}
