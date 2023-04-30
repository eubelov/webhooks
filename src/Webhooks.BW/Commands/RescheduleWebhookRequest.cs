namespace Webhooks.BW.Commands;

internal sealed record RescheduleWebhookRequest(long Attempt, SendWebhookCommand Command) : IRequest<bool>;
