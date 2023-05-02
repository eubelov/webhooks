namespace Webhooks.BW.Commands;

internal sealed record ScheduleWebhookCommand(Guid SubscriptionId, string PayloadJson, int Attempt = 1);