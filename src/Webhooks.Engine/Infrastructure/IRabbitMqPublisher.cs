namespace Webhooks.Engine.Infrastructure;

public interface IRabbitMqPublisher
{
    Task Send<T>(T message, CancellationToken cancellationToken)
        where T : class;
}
