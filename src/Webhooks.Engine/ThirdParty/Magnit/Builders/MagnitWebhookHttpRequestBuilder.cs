using MapsterMapper;
using Webhooks.Engine.Constants;
using Webhooks.Engine.ThirdParty.Builders;

namespace Webhooks.Engine.ThirdParty.Magnit.Builders;

internal sealed class MagnitWebhookHttpRequestBuilder : WebhookHttpRequestBuilder
{
    public MagnitWebhookHttpRequestBuilder(IMapper mapper) : base(mapper)
    {
    }

    public override string CustomerName => CustomerNames.Magnit;
}
