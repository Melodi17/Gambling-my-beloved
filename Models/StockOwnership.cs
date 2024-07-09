namespace Gambling_my_beloved.Models;

public class StockOwnership
{
    public int Id { get; set; }
    
    public int StockId { get; set; }
    public Stock Stock { get; set; }
    
    public int AccountId { get; set; }
    public ApplicationUser Account { get; set; }
    
    public virtual List<Transaction> Transactions { get; set; }
    
    public int GetQuantity()
    {
        int quantity = 0;
        
        foreach (Transaction transaction in this.Transactions)
            quantity += transaction.Type == TransactionType.Buy 
                ? transaction.Quantity 
                : -transaction.Quantity;
        
        return quantity;
    }
    
    public decimal GetTotalInvestment()
    {
        decimal totalInvestment = 0;
        
        foreach (Transaction transaction in this.Transactions)
            totalInvestment += transaction.Type == TransactionType.Buy 
                ? transaction.Amount 
                : -transaction.Amount;
        
        return totalInvestment;
    }
    
    public decimal GetCurrentStockValue() => this.GetQuantity() * this.Stock.UnitPrice;
}