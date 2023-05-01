namespace Webhooks.BW.Consumers;

internal sealed class ScheduleWebhookCommandConsumer : IConsumer<ScheduleWebhookCommand>
{
    private readonly IMediator _mediator;

    public ScheduleWebhookCommandConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ScheduleWebhookCommand> context)
    {
        var (subscriptionId, payload, _) = context.Message;
        var result = await _mediator.Send(new SendWebhookRequest(subscriptionId, payload), context.CancellationToken);
        if (!result)
        {
            throw new Exception("Failed to send webhook");
        }
    }
}
