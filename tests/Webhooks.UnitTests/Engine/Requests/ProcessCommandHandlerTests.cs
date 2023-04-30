using Webhooks.Engine.Ports;

namespace Webhooks.UnitTests.Engine.Requests;

public sealed class ProcessCommandHandlerTests
{
    private readonly Mock<IWebhookPayloadMapper> _mapper = new();
    private readonly NotifyCreated _command;
    private readonly ProcessCommandRequestHandler _requestHandler;
    private readonly Mock<IWebhookScheduler> _webhookScheduler = new();
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

        _mapper.SetupGet(x => x.CustomerName).Returns(_subscription.CustomerName);
        _mapper.Setup(x => x.Map(It.IsAny<CommandBase>())).Returns(new object());

        _requestHandler = new(
            context,
            _webhookScheduler.Object,
            new[] { _mapper.Object },
            NullLogger<ProcessCommandRequestHandler>.Instance);
    }

    [Fact]
    public async Task Should_Map_Command_To_Expected_Input_Type()
    {
        await _requestHandler.Handle(new(_command), default);
        _mapper.Verify(x => x.Map<CommandBase>(_command), Times.Once);
    }

    [Fact]
    public async Task Should_Send_SendWebhookCommand()
    {
        await _requestHandler.Handle(new(_command), default);
        _webhookScheduler
            .Verify(
                x => x.ScheduleSend(
                    _subscription.Id,
                    It.IsAny<string>(),
                    default),
                Times.Once);
    }

    [Fact]
    public async Task Should_Not_Send_SendWebhookCommand_If_Subscription_Not_Found()
    {
        await _requestHandler.Handle(new(new AutoFaker<NotifyModerationCompleted>()), default);
        _mapper.VerifyNoOtherCalls();
        _webhookScheduler.Verify(
            x => x.ScheduleSend(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                default),
            Times.Never);
    }
}
