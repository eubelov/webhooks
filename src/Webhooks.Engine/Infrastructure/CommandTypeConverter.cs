using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Webhooks.Engine.Infrastructure;

internal sealed class StatusConverter : ValueConverter<CommandType, string>
{
    public StatusConverter() : base(
        v => v.ToString(),
        v => (CommandType)Enum.Parse(typeof(CommandType), v))
    {
    }
}
