namespace Webhooks.UnitTests.Engine.ThirdParty.Magnit;

public sealed class MagnitWebhookPayloadMapperTests
{
    private readonly MagnitWebhookPayloadMapper _payloadMapper;

    public MagnitWebhookPayloadMapperTests()
    {
        var config = new TypeAdapterConfig { RequireExplicitMapping = true, RequireDestinationMemberSource = true };
        config.MapMagnitContracts();
        _payloadMapper = new(new Mapper(config));
    }

    [Fact]
    public void Map_Should_Have_Expected_CustomerName()
    {
        _payloadMapper.CustomerName.Should().Be("Magnit");
    }

    [Fact]
    public void Map_Should_Map_NotifyWorkmanCreated()
    {
        var command = new AutoFaker<NotifyCreated>().Generate();
        var result = _payloadMapper.Map(command);
        result.Should().BeEquivalentTo(command, x => x.ExcludingMissingMembers());
    }

    [Fact]
    public void Map_Should_Map_NotifyWorkmanModerationCompletedCreated()
    {
        var command = new AutoFaker<NotifyModerationCompleted>().Generate();
        var result = _payloadMapper.Map(command);
        result.Should().BeEquivalentTo(command, x => x.ExcludingMissingMembers());
    }

    [Fact]
    public void Map_Should_Throw_If_Unsupported_Type()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _payloadMapper.Map(new UnsupportedCommand()));
    }

    private sealed record UnsupportedCommand : CommandBase
    {
        public override CommandType CommandType => CommandType.Unknown;
    }
}
