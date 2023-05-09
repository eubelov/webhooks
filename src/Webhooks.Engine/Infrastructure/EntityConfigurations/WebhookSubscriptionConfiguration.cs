namespace Webhooks.Engine.Infrastructure.EntityConfigurations;

internal sealed class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
{
    public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
    {
        builder.Property(x => x.CustomerName)
            .HasMaxLength(100);

        builder.Property(x => x.Url)
            .HasMaxLength(250);

        builder.Property(x => x.Token)
            .HasMaxLength(1000);

        builder.Property(x => x.Type).HasConversion<StatusConverter>()
            .HasMaxLength(100);
    }
}
