using Webhooks.Commands.Workman;

namespace Webhooks.BW.Consumers.Workman;

internal sealed class NotifyModerationCompletedConsumer : IConsumer<NotifyModerationCompleted>
{
    private readonly IMediator _mediator;

    public NotifyModerationCompletedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<NotifyModerationCompleted> context)
    {
        await _mediator.Send(new ProcessIntegrationEventRequest(context.Message), context.CancellationToken);
    }
}
