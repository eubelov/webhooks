
namespace Webhooks.Engine.ThirdParty.Magnit.Mappers;

internal static class MagnitMappings
{
    public static void MapMagnitContracts(this TypeAdapterConfig config)
    {
        config.NewConfig<NotifyCreated, WorkmanCreated>()
            .Map(src => src.Inn, dest => dest.Inn)
            .Map(src => src.Phone, dest => dest.Phone);

        config.NewConfig<NotifyModerationCompleted, WorkmanModerationCompleted>()
            .Map(src => src.Inn, dest => dest.Inn)
            .Map(src => src.Phone, dest => dest.Phone);
    }
}
