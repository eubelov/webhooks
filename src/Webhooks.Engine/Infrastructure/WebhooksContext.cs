namespace Webhooks.Engine.Infrastructure;

internal sealed class WebhooksContext : DbContext
{
    private const string Schema = "webhooks";

    public WebhooksContext(DbContextOptions<WebhooksContext> options)
        : base(options)
    {
    }

    public DbSet<WebhookSubscription> Subscriptions { get; init; } = default!;
    public DbSet<WebhookInvocation> Invocations { get; init; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebhookInvocationConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
