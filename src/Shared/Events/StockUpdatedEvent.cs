namespace Shared.Events;

public class StockUpdatedEvent
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public int QuantityChange { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty; // "sale", "restock", "adjustment"
}
