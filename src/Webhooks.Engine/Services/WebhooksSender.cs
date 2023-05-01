namespace Webhooks.Engine.Services;

internal sealed class WebhooksSender : IWebhooksSender
{
    private static readonly TimeSpan[] Retries =
    {
        TimeSpan.FromSeconds(0),
        TimeSpan.FromSeconds(0),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(8),
        TimeSpan.FromSeconds(16),
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhooksSender> _logger;
    private readonly IMediator _mediator;
    private readonly List<IWebhookRequestBuilder> _requestBuilders;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public WebhooksSender(
        IHttpClientFactory httpClientFactory,
        IMediator mediator,
        IEnumerable<IWebhookRequestBuilder> requestBuilders,
        ILogger<WebhooksSender> logger)
    {
        _httpClientFactory = httpClientFactory;
        _mediator = mediator;
        _requestBuilders = requestBuilders.ToList();
        _logger = logger;
        _retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(
                Retries,
                async (exception, _, retryCount, context) =>
                {
                    var url = (string)context["Url"];
                    var type = (CommandType)context["Type"];
                    var error = exception.Exception.Message;
                    var statusCode = (int?)exception.Result?.StatusCode;

                    await mediator.Publish(new WebhookInvokedNotification(url, false, retryCount, statusCode, error));

                    _logger.LogError(
                        exception.Exception,
                        "Sending hook to {DestinationUrl} of type {WebhookType} failed with error {ErrorDescription} and status code {StatusCode}",
                        url, type,
                        error,
                        statusCode);
                });
    }

    public async Task<bool> Send(WebhookSubscription receiver, string payloadJson, CancellationToken token)
    {
        var client = _httpClientFactory.CreateClient("Default");
        return await Send(receiver, payloadJson, client, token);
    }

    private async Task<bool> Send(
        WebhookSubscription subscription,
        string payloadJson,
        HttpMessageInvoker client,
        CancellationToken token)
    {
        var attempts = 0;

        HttpRequestMessage BuildHttpRequest()
        {
            var builder = _requestBuilders.Single(x =>
                x.CustomerName.Equals(subscription.CustomerName, StringComparison.OrdinalIgnoreCase));

            return builder.BuildRequest(subscription, payloadJson);
        }

        async Task<HttpResponseMessage> SendRequest(Context _)
        {
            _logger.LogTrace(
                "Sending hook to {DestinationUrl} of type {WebhookType}",
                subscription.Url,
                subscription.Type);

            attempts++;
            return await client.SendAsync(BuildHttpRequest(), token);
        }

        var context = new Context { ["Url"] = subscription.Url, ["Type"] = subscription.Type };
        var result = await _retryPolicy.ExecuteAndCaptureAsync(SendRequest, context);
        var isSuccessful = result.Outcome is OutcomeType.Successful && result.Result.IsSuccessStatusCode;
        var statusCode = (int?)result.Result?.StatusCode;
        var error = result.FinalException?.Message ?? result.Result?.ReasonPhrase;
        await _mediator.Publish(
            new WebhookInvokedNotification(subscription.Url, isSuccessful, attempts, statusCode, error),
            token);
        return isSuccessful;
    }
}
