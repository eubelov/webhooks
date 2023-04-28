using Webhooks.Commands.Enums;

namespace Webhooks.Engine.Models;

internal sealed record WebhookData(CommandType Type, object Data);
