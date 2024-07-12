namespace Gambling_my_beloved.Models;

public class StockBinding
{
    public int Id { get; set; }
    
    public int StockId { get; set; }
    public Stock Stock { get; set; }
    
    public BindingType Type { get; set; }
    
    public string BindTarget { get; set; }
    public double Multiplier { get; set; }
}

public enum BindingType
{
    RealStock,
    InGameStock,
    InGameStockChange
}