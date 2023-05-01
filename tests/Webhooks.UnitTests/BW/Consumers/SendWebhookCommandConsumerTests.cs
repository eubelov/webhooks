using FakeItEasy;
using MassTransit;
using Webhooks.BW.Commands;
using Webhooks.BW.Consumers;

namespace Webhooks.UnitTests.BW.Consumers;

public sealed class SendWebhookCommandConsumerTests : ConsumerTestBase
{
    [Fact]
    public async Task Consume_Should_Send_SendWebhookRequest_To_Mediator()
    {
        var command = new AutoFaker<ScheduleWebhookCommand>().Generate();
        var consumer = new ScheduleWebhookCommandConsumer(Mediator);
        var context = A.Fake<ConsumeContext<ScheduleWebhookCommand>>();
        A.CallTo(() => context.Message).Returns(command);

        await consumer.Consume(context);

        A.CallTo(() =>
                Mediator.Send(
                    A<SendWebhookRequest>.That.Matches(r =>
                        r.SubscriptionId == command.SubscriptionId && r.PayloadJson == command.PayloadJson), default))
            .MustHaveHappenedOnceExactly();
    }
}
