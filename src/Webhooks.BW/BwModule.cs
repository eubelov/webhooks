using Webhooks.BW.Adapters;

namespace Webhooks.BW;

public static class BwModule
{
    public static void RegisterBwModuleDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IWebhookScheduler, WebhookScheduler>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<NotifyCreatedConsumer>();
            x.AddConsumer<NotifyModerationCompletedConsumer>();
            x.AddConsumer<ScheduleWebhookCommandConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                var settings = configuration.GetSection(nameof(RabbitSettings)).Get<RabbitSettings>()!;
                cfg.Host(settings.Uri, settings.Port, settings.VirtualHost, c =>
                {
                    c.Username(settings.Username);
                    c.Password(settings.Password);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    }
}
