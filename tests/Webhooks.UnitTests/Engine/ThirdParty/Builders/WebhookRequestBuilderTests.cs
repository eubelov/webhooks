using Webhooks.Engine.ThirdParty.Builders;

namespace Webhooks.UnitTests.Engine.ThirdParty.Builders;

public sealed class WebhookRequestBuilderTests
{
    private readonly WebhookRequestBuilderImpl _builder = new();
    private readonly WebhookSubscription _subscription;

    public WebhookRequestBuilderTests()
    {
        _subscription = new AutoFaker<WebhookSubscription>()
            .RuleFor(x => x.Url, x => x.Internet.Url())
            .RuleFor(x => x.Token, x => x.Random.String2(10))
            .RuleFor(x => x.CustomerName, _ => _builder.CustomerName)
            .Generate();
    }

    [Fact]
    public void BuildRequest_Should_Return_HttpRequestMessage_With_Correct_Properties()
    {
        var request = _builder.BuildRequest(_subscription, "{}");

        request.Method.Should().Be(HttpMethod.Post);
        request.RequestUri.Should().Be(_subscription.Url);
        request.Content.Should().BeOfType<StringContent>();
        request.Content!.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public void BuildRequest_Should_Add_Auth_Header()
    {
        var request = _builder.BuildRequest(_subscription, "{}");

        request.Headers.Authorization!.ToString().Should().Be($"Bearer {_subscription.Token}");
    }

    [Fact]
    public void BuildRequest_Should_Not_Add_Auth_Header()
    {
        _subscription.Token = null;

        var request = _builder.BuildRequest(_subscription, "{}");

        request.Headers.Authorization.Should().BeNull();
    }

    private sealed class WebhookRequestBuilderImpl : WebhookRequestBuilder
    {
        public override string CustomerName => "Test";
    }
}
