namespace Webhooks.Engine.ThirdParty.Magnit.Mappers;

internal sealed class MagnitWebhookPayloadMapper : IWebhookPayloadMapper
{
    private readonly IMapper _mapper;

    public MagnitWebhookPayloadMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public string CustomerName => CustomerNames.Magnit;

    public object Map<T>(T source)
        where T : CommandBase
    {
        return source switch
        {
            NotifyCreated => _mapper.Map<WorkmanCreated>(source),
            NotifyModerationCompleted => _mapper.Map<WorkmanModerationCompleted>(source),
            _ => throw new ArgumentOutOfRangeException(
                nameof(source),
                $"Unsupported command type - {source.GetType().Name}"),
        };
    }
}
