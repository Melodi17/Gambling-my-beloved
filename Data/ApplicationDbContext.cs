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
    public DbSet<StockBinding> StockBindings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Company>()
            .Property(e => e.Industries)
            .HasConversion(
                v => string.Join(",", v.Select(e => e.ToString()).ToArray()),
                v => v.Split(new[] { ',' })
                    .Select(e => Enum.Parse(typeof(Industry), e, true))
                    .Cast<Industry>()
                    .ToList()
            );

        modelBuilder
            .Entity<StockBinding>()
            .Property(e => e.Type)
            .HasConversion(
                v => v.ToString(),
                v => (BindingType)Enum.Parse(typeof(BindingType), v, true)
            );


        base.OnModelCreating(modelBuilder);
    }
}