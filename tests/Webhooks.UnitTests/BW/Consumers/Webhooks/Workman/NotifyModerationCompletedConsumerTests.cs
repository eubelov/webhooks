using MassTransit;
using Webhooks.BW.Commands;
using Webhooks.BW.Consumers.Webhooks.Workman;
using Webhooks.Engine.Models;

namespace Webhooks.UnitTests.BW.Consumers.Webhooks.Workman;

public sealed class NotifyModerationCompletedConsumerTests : ConsumerTestBase
{
    [Fact]
    public async Task Consume_Should_Send_ProcessCommandRequest_To_Mediator()
    {
        var message = new AutoFaker<NotifyModerationCompleted>().Generate();
        var consumer = new NotifyModerationCompletedConsumer(Mediator);
        var context = A.Fake<ConsumeContext<NotifyModerationCompleted>>();
        A.CallTo(() => context.Message).Returns(message);

        await consumer.Consume(context);

        A.CallTo(() => Mediator.Send(A<ProcessCommandRequest>.That.Matches(r => r.Command == message), default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Consume_Should_Schedule_Webhooks()
    {
        var message = new AutoFaker<NotifyModerationCompleted>().Generate();
        var consumer = new NotifyModerationCompletedConsumer(Mediator);
        var contextMock = A.Fake<ConsumeContext<NotifyModerationCompleted>>();
        A.CallTo(() => contextMock.Message).Returns(message);
        A.CallTo(() => Mediator.Send(A<ProcessCommandRequest>.That.Matches(r => r.Command == message), default))
            .Returns(new List<WebhookSchedule> { new AutoFaker<WebhookSchedule>().Generate() });

        await consumer.Consume(contextMock);

        A.CallTo(() => contextMock.Publish(A<ScheduleWebhookCommand>._, default)).MustHaveHappenedOnceExactly();
    }
}
