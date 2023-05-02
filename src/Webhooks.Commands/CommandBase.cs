namespace Webhooks.Commands;

public abstract record CommandBase
{
    public abstract CommandType CommandType { get; }
}
