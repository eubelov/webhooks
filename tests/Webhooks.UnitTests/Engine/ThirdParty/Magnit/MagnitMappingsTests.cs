namespace Webhooks.UnitTests.Engine.ThirdParty.Magnit;

public sealed class MagnitMappingsTests
{
    private readonly Mapper _mapper;

    public MagnitMappingsTests()
    {
        var config = new TypeAdapterConfig { RequireExplicitMapping = true, RequireDestinationMemberSource = true };
        config.MapMagnitContracts();
        _mapper = new(config);
    }

    [Fact]
    public void Map_Should_Map_NotifyWorkmanCreated()
    {
        var command = new AutoFaker<NotifyCreated>().Generate();
        var result = _mapper.Map<WorkmanCreated>(command);
        result.Should().BeEquivalentTo(command, x => x.ExcludingMissingMembers());
    }

    [Fact]
    public void Map_Should_Map_NotifyWorkmanModerationCompletedCreated()
    {
        var command = new AutoFaker<NotifyModerationCompleted>().Generate();
        var result = _mapper.Map<WorkmanModerationCompleted>(command);
        result.Should().BeEquivalentTo(command, x => x.ExcludingMissingMembers());
    }

    [Fact]
    public void Map_Should_Return_Null_For_Unknown_Map()
    {
        var command = new AutoFaker<NotifyModerationCompleted>().Generate();
        var result = _mapper.Map<WorkmanModerationCompleted>(command);
        result.Should().BeEquivalentTo(command, x => x.ExcludingMissingMembers());
    }
}
