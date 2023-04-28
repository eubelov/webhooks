using Mapster;
using Webhooks.Commands.Workman;
using Webhooks.Engine.ThirdParty.Magnit.Contracts;

namespace Webhooks.Engine.ThirdParty.Magnit.Mappers;

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
