using Webhooks.Engine.ThirdParty.Magnit.Builders;

namespace Webhooks.UnitTests.Engine.ThirdParty.Magnit;

public sealed class MagnitWebhookRequestBuilderTests
{
    private readonly MagnitWebhookRequestBuilder _builder = new();

    [Fact]
    public void Should_Have_Expected_CustomerName()
    {
        _builder.CustomerName.Should().Be("Magnit");
    }
}
