namespace Webhooks.Engine.Messages;

public sealed record SendWebhookCommand(Guid SubscriptionId, string PayloadJson);
