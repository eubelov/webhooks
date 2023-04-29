using AutoBogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Webhooks.Commands;
using Webhooks.Commands.Enums;
using Webhooks.Commands.Workman;
using Webhooks.Engine.Entities;
using Webhooks.Engine.Infrastructure;
using Webhooks.Engine.Infrastructure.MessageBus;
using Webhooks.Engine.Requests;
using Webhooks.Engine.ThirdParty.Builders;

namespace Webhooks.UnitTests.Engine.Requests;

public sealed class ProcessCommandHandlerTests
{
    private readonly Mock<IWebhookHttpRequestBuilder> _builder = new();
    private readonly NotifyCreated _command;
    private readonly ProcessCommandHandler _handler;
    private readonly Mock<IRabbitMqPublisher> _rabbitMqPublisher = new();
    private readonly WebhookSubscription _subscription;

    public ProcessCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<WebhooksContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _subscription = new AutoFaker<WebhookSubscription>()
            .RuleFor(x => x.IsEnabled, _ => true)
            .RuleFor(x => x.Type, _ => CommandType.WorkmanCreated)
            .Generate();
        _command = new AutoFaker<NotifyCreated>()
            .RuleFor(x => x.CustomerId, _subscription.CustomerId)
            .Generate();

        var context = new WebhooksContext(options);
        context.Subscriptions.Add(_subscription);
        context.SaveChanges();

        _builder.SetupGet(x => x.CustomerName).Returns(_subscription.CustomerName);
        _builder.Setup(x => x.BuildPayload(It.IsAny<CommandBase>())).Returns(new object());

        _handler = new(
            context,
            _rabbitMqPublisher.Object,
            new[] { _builder.Object },
            NullLogger<ProcessCommandHandler>.Instance);
    }

    [Fact]
    public async Task Should_Map_Command_To_Expected_Input_Type()
    {
        await _handler.Handle(new(_command), default);
        _builder.Verify(x => x.BuildPayload<CommandBase>(_command), Times.Once);
    }

    [Fact]
    public async Task Should_Send_SendWebhookCommand()
    {
        await _handler.Handle(new(_command), default);
        _rabbitMqPublisher
            .Verify(
                x => x.Send(It.Is<SendWebhookCommand>(c => c.SubscriptionId == _subscription.Id), default),
                Times.Once);
    }

    [Fact]
    public async Task Should_Not_Send_SendWebhookCommand_If_Subscription_Not_Found()
    {
        await _handler.Handle(new(new AutoFaker<NotifyModerationCompleted>()), default);
        _builder.VerifyNoOtherCalls();
        _rabbitMqPublisher
            .Verify(
                x => x.Send(It.Is<SendWebhookCommand>(c => c.SubscriptionId == _subscription.Id), default),
                Times.Never);
    }
}
