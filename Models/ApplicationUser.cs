using Microsoft.AspNetCore.Identity;

namespace Gambling_my_beloved.Models;

public class ApplicationUser : IdentityUser
{
    public const decimal InitialBalance = 1000;
    
    public decimal Balance { get; set; }
    public List<StockOwnership> Stocks { get; set; }
}