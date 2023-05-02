using Webhooks.Engine.Models;

namespace Webhooks.BW.Consumers;

internal abstract class WebhookConsumerBase<T> : IConsumer<T>
    where T : class
{
    public abstract Task Consume(ConsumeContext<T> context);

    protected static async Task ScheduleWebhooks(ConsumeContext<T> context, List<WebhookSchedule> schedules)
    {
        var tasks = schedules.Select(x => context.Publish(new ScheduleWebhookCommand(x.SubscriptionId, x.PayloadJson)));
        await Task.WhenAll(tasks);
    }
}
