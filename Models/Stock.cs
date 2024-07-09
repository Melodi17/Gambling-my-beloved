using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace Gambling_my_beloved.Models;

public class Stock
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    
    
    public int CompanyId { get; set; }
    public Company Company { get; set; }
    
    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }
    
    public decimal PriceChange => this.GetPriceChange();
    public virtual List<PricePeriod> PriceHistory { get; set; }
    
    public Stock(string symbol, Company company, decimal unitPrice)
    {
        this.Symbol = symbol;
        this.Company = company;
        this.UnitPrice = unitPrice;
    }

    public Stock()
    {
    }

    public void UpdatePrice(decimal newPrice)
    {
        this.UnitPrice = newPrice;
        this.PriceHistory.Add(new()
        {
            StockId = this.Id,
            Date = DateTime.Now,
            Price = newPrice
        });
    }
    
    public decimal GetPriceChange()
    {
        if (this.PriceHistory == null)
            return 0;
        
        if (this.PriceHistory.Count < 2)
            return 0;
        
        decimal lastPrice = this.PriceHistory[^2].Price;
        decimal currentPrice = this.PriceHistory[^1].Price;
        
        return Math.Round((currentPrice - lastPrice) / lastPrice * 100, 2);
    }
}

public class PricePeriod
{
    public int Id { get; set; }
    
    public int StockId { get; set; }
    public Stock Stock { get; set; }
    
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
}