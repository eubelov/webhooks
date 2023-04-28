namespace Webhooks.Engine.ThirdParty.Magnit.Contracts;

internal sealed class WorkmanModerationCompleted
{
    public string Inn { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}
