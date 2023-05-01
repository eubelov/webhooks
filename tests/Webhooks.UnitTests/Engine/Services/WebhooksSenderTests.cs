namespace Webhooks.UnitTests.Engine.Services;

public sealed class WebhooksSenderTests
{
    private const string ThirdPartyUri = "https://api.com";
    private const string JsonContentType = "application/json";
    private const string JsonPayload = "{'foo':'bar'}";

    private readonly IMediator _mediator;
    private readonly WebhooksSender _webhooksSender;
    private readonly IWebhookRequestBuilder _builder;
    private readonly WebhookSubscription _subscription;
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;

    public WebhooksSenderTests()
    {
        _mediator = A.Fake<IMediator>();
        _builder = A.Fake<IWebhookRequestBuilder>();
        _subscription = new AutoFaker<WebhookSubscription>()
            .RuleFor(x => x.Url, _ => ThirdPartyUri)
            .Generate();

        A.CallTo(() => _builder.CustomerName)
            .Returns(_subscription.CustomerName);
        A.CallTo(() => _builder.BuildRequest(_subscription, A<string>._))
            .Returns(new HttpRequestMessage(HttpMethod.Post, ThirdPartyUri));

        _mockHttpMessageHandler = new MockHttpMessageHandler();

        var httpClientFactory = A.Fake<IHttpClientFactory>();
        A.CallTo(() => httpClientFactory.CreateClient("Default")).Returns(new HttpClient(_mockHttpMessageHandler));

        _webhooksSender = new WebhooksSender(
            httpClientFactory,
            _mediator,
            new List<IWebhookRequestBuilder> { _builder },
            NullLogger<WebhooksSender>.Instance);
    }

    [Fact]
    public async Task Send_Should_Build_Http_Request()
    {
        _mockHttpMessageHandler
            .Expect(ThirdPartyUri)
            .Respond(JsonContentType, "{}");

        await _webhooksSender.Send(_subscription, JsonContentType, default);
        _mockHttpMessageHandler.VerifyNoOutstandingRequest();
        A.CallTo(() => _builder.BuildRequest(_subscription, A<string>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Send_Should_Log_Successful_Invocation()
    {
        _mockHttpMessageHandler
            .Expect(ThirdPartyUri)
            .Respond(JsonContentType, "{}");

        await _webhooksSender.Send(_subscription, JsonPayload, default);

        A.CallTo(
                () => _mediator.Publish(
                    A<WebhookInvokedNotification>.That.Matches(
                        x => x.Success &&
                             x.Url == ThirdPartyUri
                             && x.Attempt == 1
                             && x.StatusCode == (int)HttpStatusCode.OK
                    ),
                    default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Send_Should_Retry_Failed_Request()
    {
        _mockHttpMessageHandler
            .Expect(ThirdPartyUri)
            .Respond(HttpStatusCode.ServiceUnavailable, JsonContentType, "{}");

        _mockHttpMessageHandler
            .Expect(ThirdPartyUri)
            .Respond(HttpStatusCode.OK, JsonContentType, "{}");

        await _webhooksSender.Send(_subscription, JsonPayload, default);
        _mockHttpMessageHandler.VerifyNoOutstandingRequest();
        A.CallTo(() => _builder.BuildRequest(_subscription, A<string>._)).MustHaveHappened(2, Times.Exactly);
    }

    [Fact]
    public async Task Send_Should_Log_Failed_Request()
    {
        _mockHttpMessageHandler
            .Expect(ThirdPartyUri)
            .Respond(HttpStatusCode.ServiceUnavailable, JsonContentType, "{}");

        _mockHttpMessageHandler
            .Expect(ThirdPartyUri)
            .Respond(HttpStatusCode.OK, JsonContentType, "{}");

        await _webhooksSender.Send(_subscription, JsonPayload, default);

        A.CallTo(
            () => _mediator.Publish(
                A<WebhookInvokedNotification>.That.Matches(
                    x => !x.Success &&
                         x.Url == ThirdPartyUri
                         && x.Attempt == 1
                         && x.StatusCode == (int)HttpStatusCode.ServiceUnavailable
                ),
                default))
            .MustHaveHappenedOnceExactly();
    }
}
