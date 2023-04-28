using Webhooks.Commands;

namespace Webhooks.Engine.Requests;

public sealed record ProcessCommandRequest(CommandBase Command) : IRequest;
