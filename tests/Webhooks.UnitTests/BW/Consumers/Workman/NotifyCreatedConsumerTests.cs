using FakeItEasy;
using MassTransit;
using Webhooks.BW.Consumers.Workman;

namespace Webhooks.UnitTests.BW.Consumers.Workman;

public sealed class NotifyCreatedConsumerTests : ConsumerTestBase
{
    [Fact]
    public async Task Consume_Should_Send_ProcessCommandRequest_To_Mediator()
    {
        var message = new AutoFaker<NotifyCreated>().Generate();
        var consumer = new NotifyCreatedConsumer(Mediator);
        var contextMock = A.Fake<ConsumeContext<NotifyCreated>>();
        A.CallTo(() => contextMock.Message).Returns(message);

        await consumer.Consume(contextMock);

        A.CallTo(() => Mediator.Send(A<ProcessCommandRequest>.That.Matches(r => r.Command == message), default))
            .MustHaveHappenedOnceExactly();
    }
}
