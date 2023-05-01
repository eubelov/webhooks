namespace Webhooks.Commands;

public abstract record CommandBase
{
    public Guid CorrelationId { get; init; }

    public abstract CommandType CommandType { get; }
}
