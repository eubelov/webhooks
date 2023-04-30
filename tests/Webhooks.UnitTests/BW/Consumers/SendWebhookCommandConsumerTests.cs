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
        var consumer = new ScheduleWebhookCommandConsumer(MediatorMock.Object);
        var contextMock = new Mock<ConsumeContext<ScheduleWebhookCommand>>();
        contextMock.SetupGet(c => c.Message).Returns(command);

        await consumer.Consume(contextMock.Object);

        MediatorMock.Verify(m =>
                m.Send(
                    It.Is<SendWebhookRequest>(
                        r => r.SubscriptionId == command.SubscriptionId && r.PayloadJson == command.PayloadJson),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
