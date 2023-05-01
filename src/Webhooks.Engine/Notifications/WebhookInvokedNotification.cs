namespace Webhooks.Engine.Notifications;

public sealed record WebhookInvokedNotification(string Url, bool Success, int Attempt, int? StatusCode, string? Error = null)
    : INotification;
