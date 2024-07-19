using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace Gambling_my_beloved.Models;

public class Stock
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    public string Color { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
    
    [Display(Name = "Unit Price")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }
    
    public decimal Volatility { get; set; }
    
    [Display(Name = "Price Change")]
    public decimal PriceChange => this.GetPriceChange();
    
    [Display(Name = "Price Change Percent")]
    public decimal PriceChangePercent => this.GetPriceChangePercent();

    public bool Frozen { get; set; } = false;
    
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

    public void UpdatePrice(decimal newPrice, DateTime time)
    {
        if (newPrice < 0)
            newPrice = Global.Random.Next(1, 10) * 0.01m;
        
        // round to 2 decimal places
        newPrice = Math.Round(newPrice, 3);
        
        this.UnitPrice = newPrice;
        this.PriceHistory.Add(new()
        {
            StockId = this.Id,
            Date = time,
            Price = newPrice
        });
    }
    
    public decimal GetPriceChangePercent(DateTime? date = null)
    {
        if (this.PriceHistory == null)
            return 0;
        
        if (this.PriceHistory.Count < 2)
            return 0;
        
        IEnumerable<PricePeriod> priceHistory = this.PriceHistory.OrderByDescending(p => p.Date);
        
        if (date != null)
            priceHistory = priceHistory.Where(p => p.Date <= date.Value);
        
        decimal lastPrice = priceHistory.Skip(1).First().Price;
        decimal currentPrice = priceHistory.First().Price;

        try
        {
            return Math.Round((currentPrice - lastPrice) / lastPrice * 100, 2);
        }
        catch (DivideByZeroException)
        {
            return 0;
        }
    }
    
    public decimal GetPriceChange(DateTime? date = null)
    {
        if (this.PriceHistory == null)
            return 0;
        
        if (this.PriceHistory.Count < 2)
            return 0;
        
        IEnumerable<PricePeriod> priceHistory = this.PriceHistory.OrderByDescending(p => p.Date);
        
        if (date != null)
            priceHistory = priceHistory.Where(p => p.Date <= date.Value);
        
        decimal lastPrice = priceHistory.Skip(1).First().Price;
        decimal currentPrice = priceHistory.First().Price;
        
        return Math.Round(currentPrice - lastPrice, 2);
    }

    public decimal GetHistoricalPrice(DateTime date)
    {
        return this.PriceHistory
            .First(p => p.Date == date).Price;
    }
}