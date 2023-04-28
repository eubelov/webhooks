namespace Webhooks.Engine.Infrastructure;

internal sealed class WebhooksContext : DbContext
{
    private const string Schema = "webhooks";

    public WebhooksContext(DbContextOptions<WebhooksContext> options)
        : base(options)
    {
    }

    public DbSet<WebhookSubscription> Subscriptions { get; set; } = default!;
    public DbSet<WebhookInvocation> Invocations { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        base.OnModelCreating(modelBuilder);
    }
}
