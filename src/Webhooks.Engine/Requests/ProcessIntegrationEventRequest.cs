using Webhooks.Commands;

namespace Webhooks.Engine.Requests;

public sealed record ProcessIntegrationEventRequest(CommandBase Command) : IRequest;
