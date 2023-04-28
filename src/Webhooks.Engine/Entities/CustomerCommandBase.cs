namespace Webhooks.Commands;

public abstract record CustomerCommandBase : CommandBase
{
    public long CustomerId { get; init; }
}
