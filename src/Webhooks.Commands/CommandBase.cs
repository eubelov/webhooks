using System.Text.Json.Serialization;
using Webhooks.Commands.Workman;

namespace Webhooks.Commands;

[JsonDerivedType(typeof(NotifyCreated), typeDiscriminator: nameof(CommandType.WorkmanCreated))]
[JsonDerivedType(typeof(NotifyModerationCompleted), typeDiscriminator: nameof(CommandType.WorkmanModerationCompleted))]
public abstract record CommandBase
{
    public Guid Id { get; init; }

    public abstract CommandType CommandType { get; }
}
