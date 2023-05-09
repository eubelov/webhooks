namespace Webhooks.BW.Consumers;

internal sealed class ScheduleWebhookConsumer : IConsumer<ScheduleWebhookCommand>
{
    private readonly IMediator _mediator;

    public ScheduleWebhookConsumer(IMediator mediator)
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
