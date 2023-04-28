using Webhooks.Engine.Infrastructure.MessageBus;

namespace Webhooks.BW.Requests;

internal sealed record RescheduleWebhookRequest(long Attempt, SendWebhookCommand Command) : IRequest<bool>;
