namespace Webhooks.BW.Consumers.Webhooks.Workman;

internal sealed class NotifyCreatedConsumer : WebhookConsumerBase<NotifyCreated>
{
    private readonly IMediator _mediator;

    public NotifyCreatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task Consume(ConsumeContext<NotifyCreated> context)
    {
       var schedules = await _mediator.Send(new ProcessCommandRequest(context.Message), context.CancellationToken);
       await ScheduleWebhooks(context, schedules);
    }
}
