using Webhooks.Engine.Ports;

namespace Webhooks.UnitTests.Engine.Requests;

public sealed class ProcessCommandHandlerTests
{
    private readonly IWebhookPayloadMapper _mapper = A.Fake<IWebhookPayloadMapper>();
    private readonly NotifyCreated _command;
    private readonly ProcessCommandRequestHandler _requestHandler;
    private readonly IWebhookScheduler _webhookScheduler = A.Fake<IWebhookScheduler>();
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

        A.CallTo(() => _mapper.CustomerName).Returns(_subscription.CustomerName);
        A.CallTo(() => _mapper.Map(A<CommandBase>._)).Returns(new object());

        _requestHandler = new(
            context,
            _webhookScheduler,
            new[]
            {
                _mapper
            },
            NullLogger<ProcessCommandRequestHandler>.Instance);
    }

    [Fact]
    public async Task Handle_Should_Map_Command_To_Expected_Input_Type()
    {
        await _requestHandler.Handle(new(_command), default);
        A.CallTo(() => _mapper.Map(A<CommandBase>.That.IsEqualTo(_command))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Should_Send_SendWebhookCommand()
    {
        await _requestHandler.Handle(new(_command), default);

        A.CallTo(() => _webhookScheduler.ScheduleSend(_subscription.Id, A<string>._, default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Should_Not_Send_SendWebhookCommand_If_Subscription_Not_Found()
    {
        await _requestHandler.Handle(new(new AutoFaker<NotifyModerationCompleted>()), default);
        A.CallTo(() => _mapper.Map(_command)).MustNotHaveHappened();
        A.CallTo(() => _webhookScheduler.ScheduleSend(_subscription.Id, A<string>._, default))
            .MustNotHaveHappened();
    }
}
