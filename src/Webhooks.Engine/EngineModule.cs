using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webhooks.Engine.Notifications;
using Webhooks.Engine.ThirdParty.Builders;
using Webhooks.Engine.ThirdParty.Magnit.Builders;
using Webhooks.Engine.ThirdParty.Magnit.Mappers;

namespace Webhooks.Engine;

public static class EngineModule
{
    public static void RegisterEngineModuleDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = true;
        TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;

        services.AddHttpClient("default");
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<WebhookInvokedNotification>());
        services.AddScoped<IWebhooksSender, WebhooksSender>();
        services.AddScoped<IWebhookHttpRequestBuilder, MagnitWebhookHttpRequestBuilder>();

        services.AddEntityFrameworkNpgsql()
            .AddDbContext<WebhooksContext>(
                options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString("PostgresConnection"),
                        x =>
                        {
                            x.MigrationsAssembly("Webhooks.Host");
                            x.MigrationsHistoryTable("__EFMigrationsHistory", "webhooks");
                            x.EnableRetryOnFailure(5);
                        });
                });

        var config = new TypeAdapterConfig();
        config.MapMagnitContracts();
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
