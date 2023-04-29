using AutoBogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Webhooks.Commands.Enums;
using Webhooks.Engine.Entities;
using Webhooks.Engine.Infrastructure;
using Webhooks.Engine.Requests;
using Webhooks.Engine.Services;

namespace Webhooks.UnitTests.Engine.Requests;

public sealed class SendWebhookRequestHandlerTests
{
    private readonly SendWebhookRequestHandler _handler;
    private readonly SendWebhookRequest _request;
    private readonly Mock<IWebhooksSender> _sender = new();
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
        _handler = new(context, _sender.Object);
    }

    [Fact]
    public async Task Should_Send_Webhook()
    {
        await _handler.Handle(_request, default);
        _sender.Verify(
            x => x.Send(It.Is<WebhookSubscription>(s => s.Id == _subscription.Id), _request.PayloadJson, default),
            Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Return_Send_Result(bool sendResult)
    {
        _sender
            .Setup(x => x.Send(It.IsAny<WebhookSubscription>(), It.IsAny<string>(), default))
            .ReturnsAsync(sendResult);

        var result = await _handler.Handle(_request, default);

        result.Should().Be(sendResult);
    }
}
