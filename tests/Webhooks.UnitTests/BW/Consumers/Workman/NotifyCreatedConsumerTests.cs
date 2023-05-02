using MassTransit;
using Webhooks.BW.Commands;
using Webhooks.BW.Consumers.Workman;
using Webhooks.Engine.Models;

namespace Webhooks.UnitTests.BW.Consumers.Workman;

public sealed class NotifyCreatedConsumerTests : ConsumerTestBase
{
    [Fact]
    public async Task Consume_Should_Send_ProcessCommandRequest_To_Mediator()
    {
        var message = new AutoFaker<NotifyCreated>().Generate();
        var consumer = new NotifyCreatedWebhookConsumer(Mediator);
        var contextMock = A.Fake<ConsumeContext<NotifyCreated>>();
        A.CallTo(() => contextMock.Message).Returns(message);

        await consumer.Consume(contextMock);

        A.CallTo(() => Mediator.Send(A<ProcessCommandRequest>.That.Matches(r => r.Command == message), default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Consume_Should_Schedule_Webhooks()
    {
        var message = new AutoFaker<NotifyCreated>().Generate();
        var consumer = new NotifyCreatedWebhookConsumer(Mediator);
        var contextMock = A.Fake<ConsumeContext<NotifyCreated>>();
        A.CallTo(() => contextMock.Message).Returns(message);
        A.CallTo(() => Mediator.Send(A<ProcessCommandRequest>.That.Matches(r => r.Command == message), default))
            .Returns(new List<WebhookSchedule> { new AutoFaker<WebhookSchedule>().Generate() });

        await consumer.Consume(contextMock);

        A.CallTo(() => contextMock.Publish(A<ScheduleWebhookCommand>._, default)).MustHaveHappenedOnceExactly();
    }
}
