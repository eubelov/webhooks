using FakeItEasy;

namespace Webhooks.UnitTests.Engine.Requests;

public sealed class SendWebhookRequestHandlerTests
{
    private readonly SendWebhookRequestHandler _handler;
    private readonly SendWebhookRequest _request;
    private readonly IWebhooksSender _sender = A.Fake<IWebhooksSender>();
    private readonly WebhookSubscription _subscription;

    public SendWebhookRequestHandlerTests()
    {
        var options = new DbContextOptionsBuilder<WebhooksContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _subscription = new AutoFaker<WebhookSubscription>()
            .RuleFor(x => x.IsEnabled, _ => true)
            .RuleFor(x => x.Type, _ => CommandType.WorkmanCreated)
            .Generate();
        _request = new AutoFaker<SendWebhookRequest>()
            .RuleFor(x => x.SubscriptionId, _ => _subscription.Id)
            .Generate();

        var context = new WebhooksContext(options);
        context.Subscriptions.Add(_subscription);
        context.SaveChanges();
        _handler = new(context, _sender);
    }

    [Fact]
    public async Task Handle_Should_Send_Webhook()
    {
        await _handler.Handle(_request, default);
        A.CallTo(() => _sender.Send(
                A<WebhookSubscription>.That.Matches(s => s.Id == _subscription.Id), A<string>._, default))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_Should_Return_Send_Result(bool sendResult)
    {
        A.CallTo(() => _sender.Send(A<WebhookSubscription>._, A<string>._, default)).Returns(sendResult);

        var result = await _handler.Handle(_request, default);

        result.Should().Be(sendResult);
    }
}
