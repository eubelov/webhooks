namespace Webhooks.Engine.Infrastructure.MessageBus;

public sealed record SendWebhookCommand(Guid SubscriptionId, string PayloadJson);
