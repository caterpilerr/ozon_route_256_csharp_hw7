namespace NanoPaymentSystem.Database;

internal sealed class PaymentOutboxDao
{
    public Guid Id { get; set; }
    
    public Guid PaymentId { get; set; }

    public int Type { get; set; }

    public string Data { get; set; } = string.Empty;
    
    public int TryCount { get; set; }
}