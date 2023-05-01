using MassTransit;
using Webhooks.BW;
using Webhooks.Commands.Workman;
using Webhooks.Engine;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.RegisterEngineModuleDependencies(builder.Configuration);
builder.Services.RegisterBwModuleDependencies(builder.Configuration);

var app = builder.Build();
var sp = app.Services.CreateScope()
    .ServiceProvider
    .GetRequiredService<IPublishEndpoint>();

for (int i = 0; i < 100; i++)
{
    await sp.Publish(new NotifyCreated("333333333", "79117984018"));
    await sp.Publish(new NotifyModerationCompleted("333333333", "79117984018"));
}

app.Run();
