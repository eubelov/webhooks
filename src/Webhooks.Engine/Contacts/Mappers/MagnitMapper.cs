using Mapster;
using Webhooks.Commands.Workman;
using Webhooks.Engine.Contacts.Magnit;

namespace Webhooks.Engine.Contacts.Mappers;

internal static class MagnitMapper
{
    public static void MapMagnitContracts(this TypeAdapterConfig config)
    {
        config.NewConfig<NotifyCreated, WorkmanCreated>()
            .Map(src => src.Inn, dest => dest.Inn)
            .Map(src => src.Phone, dest => dest.Phone);

        config.NewConfig<WorkmanModerationCompleted, WorkmanModerationCompleted>()
            .Map(src => src.Inn, dest => dest.Inn)
            .Map(src => src.Phone, dest => dest.Phone);
    }
}
