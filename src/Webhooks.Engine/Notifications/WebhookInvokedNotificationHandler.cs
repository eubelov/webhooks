namespace Webhooks.Engine.Notifications;

internal sealed class WebhookInvokedNotificationHandler : INotificationHandler<WebhookInvokedNotification>
{
    private readonly WebhooksContext _context;

    public WebhookInvokedNotificationHandler(WebhooksContext context)
    {
        _context = context;
    }

    public async ValueTask Handle(WebhookInvokedNotification notification, CancellationToken token)
    {
        _context.Invocations.Add(
            new()
            {
                Attempt = notification.Attempt,
                ErrorDescription = notification.Error,
                IsSuccess = notification.Success,
                InvokedAtUtc = DateTime.UtcNow,
                Url = notification.Url,
                StatusCode = notification.StatusCode,
            });
        await _context.SaveChangesAsync(token);
    }
}
