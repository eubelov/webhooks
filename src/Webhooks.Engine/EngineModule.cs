namespace Webhooks.Engine;

public static class EngineModule
{
    public static void RegisterEngineModuleDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureMappings();

        services.AddHttpClient(ConfigurationConstants.HttpClientName);

        services
            .AddMediator(x => x.ServiceLifetime = ServiceLifetime.Transient)
            .AddTransient<IMapper, ServiceMapper>()
            .AddTransient<IWebhooksSender, WebhooksSender>()
            .AddTransient<IWebhookRequestBuilder, MagnitWebhookRequestBuilder>()
            .AddTransient<IWebhookPayloadMapper, MagnitWebhookPayloadMapper>();

        services
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
    }

    private static void ConfigureMappings(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = true;
        TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;
        var config = new TypeAdapterConfig();
        config.MapMagnitContracts();
        services.AddSingleton(config);
    }
}
