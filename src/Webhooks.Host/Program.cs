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
app.Run();
