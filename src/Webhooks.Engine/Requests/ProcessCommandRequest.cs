using Webhooks.Engine.Models;

namespace Webhooks.Engine.Requests;

public sealed record ProcessCommandRequest(CommandBase Command) : IRequest<List<WebhookSchedule>>;
