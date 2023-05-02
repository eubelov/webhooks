namespace Webhooks.UnitTests.Engine.Requests;

public sealed class ProcessCommandHandlerTests
{
    private readonly IWebhookPayloadMapper _mapper = A.Fake<IWebhookPayloadMapper>();
    private readonly NotifyCreated _command;
    private readonly ProcessCommandRequestHandler _requestHandler;

    public ProcessCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<WebhooksContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var subscription = new AutoFaker<WebhookSubscription>()
            .RuleFor(x => x.IsEnabled, _ => true)
            .RuleFor(x => x.Type, _ => CommandType.WorkmanCreated)
            .Generate();
        _command = new AutoFaker<NotifyCreated>()
            .RuleFor(x => x.CustomerId, subscription.CustomerId)
            .Generate();

        var context = new WebhooksContext(options);
        context.Subscriptions.Add(subscription);
        context.SaveChanges();

        A.CallTo(() => _mapper.CustomerName).Returns(subscription.CustomerName);
        A.CallTo(() => _mapper.Map(A<CommandBase>._)).Returns(new object());

        _requestHandler = new(
            context,
            new[] { _mapper },
            NullLogger<ProcessCommandRequestHandler>.Instance);
    }

    [Fact]
    public async Task Handle_Should_Map_Command_To_Expected_Input_Type()
    {
        await _requestHandler.Handle(new(_command), default);
        A.CallTo(() => _mapper.Map(A<CommandBase>.That.IsEqualTo(_command))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Return_List_Of_Scheduled_Webhooks()
    {
        var result = await _requestHandler.Handle(new(_command), default);
        result.Should().NotBeEmpty();
    }
}
