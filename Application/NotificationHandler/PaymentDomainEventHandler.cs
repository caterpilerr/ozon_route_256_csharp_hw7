using MediatR;
using NanoPaymentSystem.Application.Repository;
using NanoPaymentSystem.Domain.DomainEvents;

namespace NanoPaymentSystem.Application.NotificationHandler;

internal class PaymentDomainEventHandler :
    INotificationHandler<PaymentCreatedEvent>,
    INotificationHandler<PaymentProcessingStartedEvent>,
    INotificationHandler<PaymentAuthorizedEvent>,
    INotificationHandler<PaymentRejectedEvent>,
    INotificationHandler<PaymentCancelledEvent>
{
    private readonly IPaymentOutboxStore _paymentOutboxStore;

    public PaymentDomainEventHandler(IPaymentOutboxStore paymentOutboxStore)
    {
        _paymentOutboxStore = paymentOutboxStore;
    }

    /// <inheritdoc />
    public Task Handle(PaymentCreatedEvent notification, CancellationToken cancellationToken)
        => _paymentOutboxStore.Add(
            new PaymentOutboxItem(notification.PaymentId, notification.PaymentStatus, notification.Event),
            cancellationToken);


    /// <inheritdoc />
    public Task Handle(PaymentProcessingStartedEvent notification, CancellationToken cancellationToken)
        => _paymentOutboxStore.Add(
            new PaymentOutboxItem(notification.PaymentId, notification.PaymentStatus, notification.Event),
            cancellationToken);

    /// <inheritdoc />
    public Task Handle(PaymentAuthorizedEvent notification, CancellationToken cancellationToken)
        => _paymentOutboxStore.Add(
            new PaymentOutboxItem(notification.PaymentId, notification.PaymentStatus, notification.Event),
            cancellationToken);

    /// <inheritdoc />
    public Task Handle(PaymentRejectedEvent notification, CancellationToken cancellationToken)
        => _paymentOutboxStore.Add(
            new PaymentOutboxItem(notification.PaymentId, notification.PaymentStatus, notification.Event),
            cancellationToken);

    /// <inheritdoc />
    public Task Handle(PaymentCancelledEvent notification, CancellationToken cancellationToken)
        => _paymentOutboxStore.Add(
            new PaymentOutboxItem(notification.PaymentId, notification.PaymentStatus, notification.Event),
            cancellationToken);
}