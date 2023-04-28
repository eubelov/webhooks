namespace Webhooks.BW.Producers;

internal sealed class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMqPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Send<T>(T message, CancellationToken cancellationToken)
        where T : class
    {
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}
