namespace Webhooks.Engine.ThirdParty.Magnit.Builders;

internal sealed class MagnitWebhookRequestBuilder : WebhookRequestBuilder
{
    public override string CustomerName => CustomerNames.Magnit;
}
