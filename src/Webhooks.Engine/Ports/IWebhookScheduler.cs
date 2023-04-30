namespace Webhooks.Engine.Ports;

public interface IWebhookScheduler
{
    Task ScheduleSend(Guid subscriptionId, string payloadJson, CancellationToken token);
}
