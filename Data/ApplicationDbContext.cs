using Gambling_my_beloved.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<StockOwnership> StockOwnerships { get; set; }
    public DbSet<StockEvent> StockEvents { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

}