using MassTransit;
using Serilog;
using Webhooks.BW;
using Webhooks.Commands.Workman;
using Webhooks.Engine;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
    lc.WriteTo.Console();
#if DEBUG
    lc.WriteTo.Seq("http://localhost:5341");
#endif
    lc.Enrich.WithCorrelationId();
});


builder.Services.RegisterEngineModuleDependencies(builder.Configuration);
builder.Services.RegisterBwModuleDependencies(builder.Configuration);

var app = builder.Build();
var sp = app.Services.CreateScope()
    .ServiceProvider
    .GetRequiredService<IPublishEndpoint>();

for (int i = 0; i < 1; i++)
{
    await sp.Publish(new NotifyCreated("333333333", "79117984018"),
        x => x.CorrelationId = Guid.NewGuid());
    await sp.Publish(new NotifyModerationCompleted("333333333", "79117984018"),
        x => x.CorrelationId = Guid.NewGuid());
}

app.Run();
