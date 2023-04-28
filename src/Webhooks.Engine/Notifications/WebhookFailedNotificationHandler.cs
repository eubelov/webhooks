namespace Webhooks.Engine.Notifications;

internal sealed class WebhookFailedNotificationHandler : INotificationHandler<WebhookInvokedNotification>
{
    private readonly WebhooksContext _context;

    public WebhookFailedNotificationHandler(WebhooksContext context)
    {
        _context = context;
    }

    public async Task Handle(WebhookInvokedNotification notification, CancellationToken cancellationToken)
    {
        _context.Invocations.Add(
            new()
            {
                Attempt = notification.Attempt,
                ErrorDescription = notification.Error,
                IsSuccess = notification.Success,
                InvokedAtUtc = DateTime.UtcNow,
                Url = notification.Url
            });
        await _context.SaveChangesAsync(cancellationToken);
    }
}
