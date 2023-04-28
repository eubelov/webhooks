namespace Webhooks.Engine.Notifications;

internal sealed record WebhookInvokedNotification(string Url, bool Success, int Attempt, string? Error = null)
    : INotification;
