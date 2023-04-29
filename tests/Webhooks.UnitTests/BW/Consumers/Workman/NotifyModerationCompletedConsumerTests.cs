using AutoBogus;
using MassTransit;
using Moq;
using Webhooks.BW.Consumers.Workman;
using Webhooks.Commands.Workman;
using Webhooks.Engine.Requests;

namespace Webhooks.UnitTests.BW.Consumers.Workman;

public sealed class NotifyModerationCompletedConsumerTests : ConsumerTestBase
{
    [Fact]
    public async Task Consume_Should_Send_ProcessCommandRequest_To_Mediator()
    {
        var message = new AutoFaker<NotifyModerationCompleted>().Generate();
        var consumer = new NotifyModerationCompletedConsumer(MediatorMock.Object);
        var contextMock = new Mock<ConsumeContext<NotifyModerationCompleted>>();
        contextMock.SetupGet(c => c.Message).Returns(message);

        await consumer.Consume(contextMock.Object);

        MediatorMock.Verify(m =>
            m.Send(It.Is<ProcessCommandRequest>(r => r.Command == message),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}
