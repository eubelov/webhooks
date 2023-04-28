namespace Webhooks.Commands;

public abstract record CustomerCommandBase : CommandBase
{
    public Guid CustomerId { get; init; }
}
