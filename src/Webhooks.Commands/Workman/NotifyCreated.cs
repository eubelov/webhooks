namespace Webhooks.Commands.Workman;

public sealed record NotifyCreated(string Inn, string Phone) : CustomerCommandBase
{
    public override CommandType CommandType => CommandType.WorkmanCreated;
}