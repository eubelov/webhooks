namespace Webhooks.Engine.Infrastructure.MessageBus;

public interface IRabbitMqPublisher
{
    Task Send<T>(T message, CancellationToken token)
        where T : class;
}
