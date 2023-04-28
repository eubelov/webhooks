using Webhooks.Engine.Infrastructure.MessageBus;

namespace Webhooks.BW.Producers;

internal sealed class RescheduleWebhookRequestHandler : IRequestHandler<RescheduleWebhookRequest, bool>
{
    private const string AttemptNumberHeader = "x-webhook-attempt";
    private readonly ILogger<RescheduleWebhookRequestHandler> _logger;

    private readonly IMessageScheduler _messageScheduler;

    public RescheduleWebhookRequestHandler(
        IMessageScheduler messageScheduler,
        ILogger<RescheduleWebhookRequestHandler> logger)
    {
        _messageScheduler = messageScheduler;
        _logger = logger;
    }

    public async Task<bool> Handle(RescheduleWebhookRequest request, CancellationToken token)
    {
        var pipe = Pipe.Execute<SendContext<SendWebhookCommand>>(
            ctx => ctx.Headers.Set(AttemptNumberHeader, request.Attempt + 1));

        await _messageScheduler.SchedulePublish(
            TimeSpan.FromSeconds(10),
            request.Command,
            pipe,
            token);

        _logger.LogInformation("Rescheduled hook. Next attempt in ");
        return true;
    }
}
