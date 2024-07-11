using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Gambling_my_beloved.Models;

public class ApplicationUser : IdentityUser
{
    public const decimal InitialBalance = 1000;
    
    [DataType(DataType.Currency)]
    public decimal Balance { get; set; } = InitialBalance;
    public List<StockOwnership> Stocks { get; set; }

    public decimal GetTotalInvestment() => this.Stocks.Sum(x => x.GetTotalInvestment());

    public decimal GetTotalValue() => this.Stocks.Sum(x => x.GetCurrentStockValue());
    
    public decimal GetNetWorth() => this.Balance + this.GetTotalValue();
    
    public decimal GetTotalProfit() => this.GetTotalValue() - this.GetTotalInvestment();
    public decimal GetTotalProfitPercentage() => this.Stocks.Sum(x => x.GetTotalYieldPercentage());
    public List<Stock> GetOwnedStocks() => this.Stocks.Where(x => x.GetQuantity() > 0).Select(x => x.Stock).ToList();
}