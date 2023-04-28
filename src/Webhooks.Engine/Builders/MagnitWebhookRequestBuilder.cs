using MapsterMapper;
using Webhooks.Engine.Constants;

namespace Webhooks.Engine.Builders;

internal sealed class MagnitWebhookRequestBuilder : WebhookRequestBuilder
{
    public MagnitWebhookRequestBuilder(IMapper mapper) : base(mapper)
    {
    }

    public override string CustomerName => CustomerNames.Magnit;
}
