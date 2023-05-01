using FakeItEasy;
using MediatR;

namespace Webhooks.UnitTests.BW.Consumers;

public abstract class ConsumerTestBase
{
    protected ConsumerTestBase()
    {
        Mediator = A.Fake<IMediator>();
    }

    protected IMediator Mediator { get; }
}
