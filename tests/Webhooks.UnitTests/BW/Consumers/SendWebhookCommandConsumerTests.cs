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
        var consumer = new ScheduleWebhookConsumer(Mediator);
        var context = A.Fake<ConsumeContext<ScheduleWebhookCommand>>();
        A.CallTo(() => context.Message).Returns(command);
        A.CallTo(() => Mediator.Send(A<SendWebhookRequest>._, default)).Returns(true);

        await consumer.Consume(context);

        A.CallTo(() =>
                Mediator.Send(
                    A<SendWebhookRequest>.That.Matches(r =>
                        r.SubscriptionId == command.SubscriptionId && r.PayloadJson == command.PayloadJson), default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Consume_Should_Throw_If_False_Returned_From_Mediator()
    {
        var command = new AutoFaker<ScheduleWebhookCommand>().Generate();
        var consumer = new ScheduleWebhookConsumer(Mediator);
        var context = A.Fake<ConsumeContext<ScheduleWebhookCommand>>();
        A.CallTo(() => context.Message).Returns(command);
        A.CallTo(() => Mediator.Send(A<SendWebhookRequest>._, default)).Returns(false);

        await Assert.ThrowsAnyAsync<Exception>(() => consumer.Consume(context));
    }
}
