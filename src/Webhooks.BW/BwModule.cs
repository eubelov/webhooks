using Webhooks.Engine.Infrastructure.MessageBus;

namespace Webhooks.BW;

public static class BwModule
{
    public static void RegisterBwModuleDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        JobStorage.Current = new MemoryStorage();

        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<RabbitSettings>());
        services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();

        services.AddMassTransit(x =>
        {
            var settings = configuration.GetSection(nameof(RabbitSettings)).Get<RabbitSettings>()!;

            x.AddMessageScheduler(new("queue:webhooks-hangfire"));
            x.AddConsumer<NotifyCreatedConsumer>();
            x.AddConsumer<NotifyModerationCompletedConsumer>();
            x.AddConsumer<SendWebhookCommandConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(settings.Uri, settings.Port, settings.VirtualHost, c =>
                {
                    c.Username(settings.Username);
                    c.Password(settings.Password);
                });

                cfg.ConfigureEndpoints(ctx);
                cfg.UseMessageScheduler(new("queue:webhooks-hangfire"));
                cfg.UseHangfireScheduler("webhooks-hangfire");
            });
        });
    }
}
