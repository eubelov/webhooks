using MassTransit;
using Webhooks.BW.Adapters;
using Webhooks.BW.Commands;

namespace Webhooks.UnitTests.BW.Adapters;

public sealed class WebhookSchedulerTests
{
    private readonly IPublishEndpoint _publishEndpoint = A.Fake<IPublishEndpoint>();
    private readonly WebhookScheduler _scheduler;

    public WebhookSchedulerTests()
    {
        _scheduler = new(_publishEndpoint);
    }

    [Fact]
    public async Task ScheduleSend_Should_Publish_ScheduleWebhookCommand()
    {
        var id = Guid.NewGuid();
        const string json = "{}";
        await _scheduler.ScheduleSend(id, json, default);
        A.CallTo(() =>
                _publishEndpoint.Publish(
                    A<ScheduleWebhookCommand>.That.Matches(x => x.SubscriptionId == id && x.PayloadJson == json), default))
            .MustHaveHappenedOnceExactly();
    }
}
