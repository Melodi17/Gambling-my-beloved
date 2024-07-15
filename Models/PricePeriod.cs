using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved.Models;

[Index(nameof(Date), AllDescending = true)]
public class PricePeriod
{
    public int Id { get; set; }
    
    public int StockId { get; set; }
    public Stock Stock { get; set; }
    
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
}