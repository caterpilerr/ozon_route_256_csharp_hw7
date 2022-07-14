using NanoPaymentSystem.Domain;
using NanoPaymentSystem.Domain.Lifecycles;

namespace NanoPaymentSystem.Application.NotificationHandler;

public class PaymentOutboxItem
{
   public PaymentOutboxItem(Guid paymentId, PaymentStatus status, ILifecycleData data)
   {
      Id = Guid.NewGuid();
      Event = new IntegrationEvent(paymentId, status, data);
      TryCount = 0;
   }
   
   public PaymentOutboxItem(Guid id, IntegrationEvent @event, int tryCount)
   {
      Id = id;
      Event = @event;
      TryCount = tryCount;
   }
   
   public Guid Id { get; }
   
   public IntegrationEvent Event { get;}
   
   public int TryCount { get; set; }
}