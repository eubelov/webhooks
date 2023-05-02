namespace Webhooks.Engine.Models;

public sealed record WebhookSchedule(Guid SubscriptionId, string PayloadJson);
