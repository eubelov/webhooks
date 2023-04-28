using Webhooks.Engine.Infrastructure.MessageBus;

namespace Webhooks.BW.Consumers;

internal sealed class SendWebhookCommandConsumer : IConsumer<SendWebhookCommand>
{
    private readonly IMediator _mediator;

    public SendWebhookCommandConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendWebhookCommand> context)
    {
        var (subscriptionId, payload) = context.Message;
        await _mediator.Send(new SendWebhookRequest(subscriptionId, payload), context.CancellationToken);
    }
}
