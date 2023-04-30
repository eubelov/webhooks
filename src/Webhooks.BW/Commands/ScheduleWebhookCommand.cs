namespace Webhooks.BW.Commands;

public sealed record ScheduleWebhookCommand(Guid SubscriptionId, string PayloadJson, int Attempt = 1);
