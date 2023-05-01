namespace Webhooks.Engine.Entities;

internal sealed class WebhookSubscription
{
    public Guid Id { get; set; }
    public CommandType Type { get; set; }
    public string Url { get; set; } = default!;
    public string? Token { get; set; }
    public Guid CustomerId { get; set; }
    public bool IsEnabled { get; set; }
    public string CustomerName { get; set; } = default!;
}
