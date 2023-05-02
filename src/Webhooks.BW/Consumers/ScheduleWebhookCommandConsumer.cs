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
        var request = new SendWebhookRequest(subscriptionId, payload);
        var result = await _mediator.Send(request, context.CancellationToken);
        if (!result)
        {
            throw new Exception("Failed to send webhook");
        }
    }
}
