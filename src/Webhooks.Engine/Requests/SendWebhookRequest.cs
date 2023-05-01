namespace Webhooks.Engine.Requests;

public sealed record SendWebhookRequest(Guid SubscriptionId, string PayloadJson) : IRequest<bool>;
