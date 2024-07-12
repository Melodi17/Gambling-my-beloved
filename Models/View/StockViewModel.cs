namespace Gambling_my_beloved.Models.View;

public class StockViewModel
{
    public Stock Stock { get; set; }
    public int? Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceChange { get; set; }
    public decimal? PricePercent { get; set; }
    
    public StockViewModel(Stock stock, decimal? price = null, int? quantity = null, decimal? priceChange = null, decimal? pricePercent = null)
    {
        this.Stock = stock;
        this.Price = price ?? stock.UnitPrice;
        this.Quantity = quantity;
        this.PriceChange = priceChange;
        this.PricePercent = pricePercent;
    }

    public StockViewModel(Stock stock, StockEvent stockEvent)
    {
        this.Stock = stock;
        this.Price = stock.GetHistoricalPrice(stockEvent.Date);
        this.PriceChange = stock.GetPriceChange(stockEvent.Date);
        this.PricePercent = stock.GetPriceChangePercent(stockEvent.Date);
    }
}