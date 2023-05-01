using Webhooks.Commands;

namespace Webhooks.Engine.ThirdParty.Mappers;

internal interface IWebhookPayloadMapper
{
    string CustomerName { get; }

    object Map<T>(T source)
        where T : CommandBase;
}
