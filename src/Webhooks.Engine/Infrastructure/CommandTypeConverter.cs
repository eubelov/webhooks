namespace Webhooks.Engine.Infrastructure;

internal sealed class StatusConverter : ValueConverter<CommandType, string>
{
    public StatusConverter()
        : base(
            x => x.ToString(),
            x => (CommandType)Enum.Parse(typeof(CommandType), x))
    {
    }
}
