namespace Gambling_my_beloved.Models;

public class Transaction
{
    public int Id { get; set; }
    
    public int StockOwnershipId { get; set; }
    public StockOwnership StockOwnership { get; set; }
    
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    
    public DateTime Date { get; set; }
}