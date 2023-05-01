using MassTransit;
using Webhooks.BW.Consumers.Workman;

namespace Webhooks.UnitTests.BW.Consumers.Workman;

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
}
