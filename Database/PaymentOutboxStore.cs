using Dapper;
using NanoPaymentSystem.Application.NotificationHandler;
using NanoPaymentSystem.Application.Repository;
using NanoPaymentSystem.Domain;
using NanoPaymentSystem.Domain.LifecycleData;
using NanoPaymentSystem.Domain.Lifecycles;
using Newtonsoft.Json;
using Npgsql;

namespace NanoPaymentSystem.Database;

public class PaymentOutboxStore : IPaymentOutboxStore
{
    private readonly string _connectionString;

    public PaymentOutboxStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task Add(PaymentOutboxItem item, CancellationToken cancellationToken)
    {
        const string sql = "INSERT INTO payment_outbox VALUES(@id, @payment_id, @type, @data, @try_count)";

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(sql, new
        {
            id = item.Id,
            payment_id = item.Event.PaymentId,
            type = item.Event.Status,
            data = JsonConvert.SerializeObject(item.Event.Data),
            try_count = item.TryCount,
        });
    }

    public async Task<IEnumerable<PaymentOutboxItem>> GetAll(CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM payment_outbox";

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        var paymentOutboxItems = await connection.QueryAsync<PaymentOutboxDao>(sql);

        var result = new List<PaymentOutboxItem>();

        foreach (var dao in paymentOutboxItems)
        {
            ILifecycleData data = (PaymentStatus)dao.Type switch
            {
                PaymentStatus.New => JsonConvert.DeserializeObject<PaymentCreatedData>(dao.Data)!,
                PaymentStatus.Processing => JsonConvert.DeserializeObject<PaymentProcessingStartedData>(dao.Data)!,
                PaymentStatus.Authorized => JsonConvert.DeserializeObject<PaymentAuthorizedData>(dao.Data)!,
                PaymentStatus.Rejected => JsonConvert.DeserializeObject<PaymentRejectedData>(dao.Data)!,
                PaymentStatus.Cancelled => JsonConvert.DeserializeObject<PaymentCanceledData>(dao.Data)!,
                PaymentStatus.CancelRequested => JsonConvert.DeserializeObject<PaymentRequestCancelData>(dao.Data)!,
                _ => throw new ArgumentOutOfRangeException()
            };

            var integrationEvent = new IntegrationEvent(dao.PaymentId, (PaymentStatus)dao.Type, data);
            var paymentOutboxItem = new PaymentOutboxItem(dao.Id, integrationEvent, dao.TryCount);
            result.Add(paymentOutboxItem);
        }

        return result;
    }

    public async Task IncreaseTryCount(IEnumerable<Guid> itemIds, CancellationToken cancellationToken)
    {
        const string sql = "UPDATE payment_outbox SET try_count = try_count + 1 WHERE id IN (SELECT * FROM unnest(@idList))";
        
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(sql, new { idList = itemIds.AsEnumerable() });
    }

    public async Task Remove(IEnumerable<Guid> itemIds, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM payment_outbox WHERE id IN (SELECT * FROM unnest(@idList))";

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(sql, new { idList = itemIds });
    }
}