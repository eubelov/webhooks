using MediatR;

namespace Webhooks.UnitTests.BW.Consumers;

public class ConsumerTestBase
{
    protected ConsumerTestBase()
    {
        MediatorMock = new();
    }

    protected Mock<IMediator> MediatorMock { get; }
}
