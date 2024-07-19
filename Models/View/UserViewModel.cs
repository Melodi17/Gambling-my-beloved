namespace Gambling_my_beloved.Models.View;

public class UserViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    
    public decimal Balance { get; set; }
    public decimal NetWorth { get; set; }
    public int TotalStocks { get; set; }
}