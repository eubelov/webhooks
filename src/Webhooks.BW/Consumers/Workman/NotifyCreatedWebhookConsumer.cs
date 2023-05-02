namespace Webhooks.BW.Consumers.Workman;

internal sealed class NotifyCreatedWebhookConsumer : WebhookConsumerBase<NotifyCreated>
{
    private readonly IMediator _mediator;

    public NotifyCreatedWebhookConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task Consume(ConsumeContext<NotifyCreated> context)
    {
       var schedules = await _mediator.Send(new ProcessCommandRequest(context.Message), context.CancellationToken);
       await ScheduleWebhooks(context, schedules);
    }
}
