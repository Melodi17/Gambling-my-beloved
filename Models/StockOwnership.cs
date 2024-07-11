using System.ComponentModel.DataAnnotations.Schema;

namespace Gambling_my_beloved.Models;

public class StockOwnership
{
    public int Id { get; set; }
    
    public int StockId { get; set; }
    public Stock Stock { get; set; }
    
    public string AccountId { get; set; }
    [ForeignKey("AccountId")]
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
        
        // Store a list of stocks currently owned induvidually
        List<(decimal cost, int amount)> ownedStocks = new();
        
        foreach (Transaction transaction in this.Transactions
                     .OrderBy(transaction => transaction.Date))
        {
            if (transaction.Type == TransactionType.Buy)
            {
                totalInvestment += transaction.Amount;
                decimal singleCost = transaction.Amount / transaction.Quantity;
                ownedStocks.Add((singleCost, transaction.Quantity));
            }
            else
            {
                int quantity = transaction.Quantity;
                
                while (quantity > 0)
                {
                    (decimal cost, int amount) = ownedStocks[0];
                    
                    if (amount <= quantity)
                    {
                        totalInvestment -= cost * amount;
                        quantity -= amount;
                        ownedStocks.RemoveAt(0);
                    }
                    else
                    {
                        totalInvestment -= cost * quantity;
                        ownedStocks[0] = (cost, amount - quantity);
                        quantity = 0;
                    }
                }
            }
        }
        
        return totalInvestment;
    }
    
    public decimal GetCurrentStockValue() => this.GetQuantity() * this.Stock.UnitPrice;
}