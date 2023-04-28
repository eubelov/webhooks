namespace Webhooks.BW.Settings;

internal sealed class RabbitSettings
{
    public string Uri { get; set; } = default!;

    public string VirtualHost { get; set; } = default!;

    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;

    public ushort Port { get; set; }
}
