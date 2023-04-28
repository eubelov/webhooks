namespace Webhooks.BW.Consumers.Workman;

internal sealed class NotifyCreatedConsumer : IConsumer<NotifyCreated>
{
    private readonly IMediator _mediator;

    public NotifyCreatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<NotifyCreated> context)
    {
        await _mediator.Send(new ProcessCommandRequest(context.Message), context.CancellationToken);
    }
}
