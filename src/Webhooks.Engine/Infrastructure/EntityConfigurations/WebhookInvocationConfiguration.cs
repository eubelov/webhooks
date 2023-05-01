using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Webhooks.Engine.Infrastructure.EntityConfigurations;

internal sealed class WebhookInvocationConfiguration : IEntityTypeConfiguration<WebhookInvocation>
{
    public void Configure(EntityTypeBuilder<WebhookInvocation> builder)
    {
        builder.Property(x => x.Url)
            .HasMaxLength(250);

        builder.Property(x => x.ErrorDescription)
            .HasMaxLength(2000);
    }
}
