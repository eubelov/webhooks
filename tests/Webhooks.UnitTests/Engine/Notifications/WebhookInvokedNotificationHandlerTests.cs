using AutoBogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Webhooks.Engine.Infrastructure;
using Webhooks.Engine.Notifications;

namespace Webhooks.UnitTests.Engine.Notifications;

public sealed class WebhookInvokedNotificationHandlerTests
{
    [Fact]
    public async Task Handle_Should_Add_Invocation_To_DbSet_And_SaveChanges()
    {
        var options = new DbContextOptionsBuilder<WebhooksContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new WebhooksContext(options);
        var notification = new AutoFaker<WebhookInvokedNotification>().Generate();
        var handler = new WebhookInvokedNotificationHandler(context);

        await handler.Handle(notification, CancellationToken.None);

        context.Invocations.Should()
            .ContainSingle(
                x =>
                    x.Attempt == notification.Attempt &&
                    x.ErrorDescription == notification.Error &&
                    x.IsSuccess == notification.Success &&
                    x.InvokedAtUtc != default &&
                    x.Url == notification.Url);
    }
}
