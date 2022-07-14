using Microsoft.Extensions.Hosting;
using NanoPaymentSystem.Application.Repository;

namespace NanoPaymentSystem.Application.NotificationHandler;

public class PaymentOutboxWorker : BackgroundService
{
    private const int _pollingDelay = 10000;
    private const int _tryLimit = 7;
    private readonly IPaymentOutboxStore _paymentOutboxStore;
    private readonly IMessageBroker _messageBroker;

    public PaymentOutboxWorker(
        IPaymentOutboxStore paymentOutboxStore,
        IMessageBroker messageBroker)
    {
        _paymentOutboxStore = paymentOutboxStore;
        _messageBroker = messageBroker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var events = await _paymentOutboxStore.GetAll(stoppingToken);
            var messagesIdsToRemove = new List<Guid>();
            var failedMessagesIds = new List<Guid>();
            foreach (var item in events)
            {
                if (item.TryCount > _tryLimit)
                {
                    messagesIdsToRemove.Add(item.Id);
                    continue;
                }

                try
                {
                    await _messageBroker.Publish(item.Event, stoppingToken);
                    messagesIdsToRemove.Add(item.Id);
                }
                catch (Exception)
                {
                    item.TryCount++;
                    failedMessagesIds.Add(item.Id);
                }
            }

            if (failedMessagesIds.Count > 0)
            {
                await _paymentOutboxStore.IncreaseTryCount(failedMessagesIds, stoppingToken);
            }

            if (messagesIdsToRemove.Count > 0)
            {
                await _paymentOutboxStore.Remove(messagesIdsToRemove, stoppingToken);
            }

            await Task.Delay(_pollingDelay, stoppingToken); 
        }
    }
}