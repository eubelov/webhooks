namespace Webhooks.Commands.Workman;

public sealed record NotifyModerationCompleted(string Inn, string Phone) : CustomerCommandBase
{
    public override CommandType CommandType => CommandType.WorkmanModerationCompleted;
}
