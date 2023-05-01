namespace Webhooks.Engine.Notifications;

internal sealed record WebhookInvokedNotification(string Url, bool Success, int Attempt, int? StatusCode, string? Error = null)
    : INotification;
