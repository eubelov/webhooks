namespace Webhooks.Commands;

public abstract record CommandBase
{
    public Guid Id { get; init; }

    public abstract CommandType CommandType { get; }
}
