using NanoPaymentSystem.Application.NotificationHandler;

namespace NanoPaymentSystem.Application.Repository;

public interface IPaymentOutboxStore
{
    public Task Add(PaymentOutboxItem item, CancellationToken cancellationToken);
    public Task<IEnumerable<PaymentOutboxItem>> GetAll(CancellationToken cancellationToken);
    public Task IncreaseTryCount(IEnumerable<Guid> itemIds, CancellationToken cancellationToken);
    public Task Remove(IEnumerable<Guid> itemIds, CancellationToken cancellationToken);
}