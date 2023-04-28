namespace Webhooks.Engine.Entities;

public sealed class WebhookInvocation
{
    public Guid Id { get; set; }
    public DateTime InvokedAtUtc { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorDescription { get; set; }
    public int Attempt { get; set; }
    public string Url { get; set; } = string.Empty;
}
