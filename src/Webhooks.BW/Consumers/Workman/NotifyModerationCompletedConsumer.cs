namespace Webhooks.BW.Consumers.Workman;

internal sealed class NotifyModerationCompletedConsumer : WebhookConsumerBase<NotifyModerationCompleted>
{
    private readonly IMediator _mediator;

    public NotifyModerationCompletedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task Consume(ConsumeContext<NotifyModerationCompleted> context)
    {
        var schedules = await _mediator.Send(new ProcessCommandRequest(context.Message), context.CancellationToken);
        await ScheduleWebhooks(context, schedules);
    }
}
