using Webhooks.BW.Commands;
using Webhooks.Engine.Ports;

namespace Webhooks.BW.Adapters;

internal sealed class WebhookScheduler : IWebhookScheduler
{
    private readonly IPublishEndpoint _publishEndpoint;

    public WebhookScheduler(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task ScheduleSend(Guid subscriptionId, string payloadJson, CancellationToken token)
    {
        await _publishEndpoint.Publish(new ScheduleWebhookCommand(subscriptionId, payloadJson), token);
    }
}
