using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved.Models;

public class StockEvent
{
    public int Id { get; set; }
    public decimal Weight { get; set; }
    public string Description { get; set; }
    public bool IsPositive { get; set; }
    public StockEventType Type { get; set; }

    public DateTime Date { get; set; }

    public int? CompanyId { get; set; }
    public virtual Company Company { get; set; }

    public Industry? Industry { get; set; } = null;

    private static readonly StockEventType[] CompanyEvents =
    {
        StockEventType.Dividend,
        StockEventType.Split,
        StockEventType.Merger,
        StockEventType.Acquisition,
        StockEventType.Financial,
        StockEventType.IPO,
        StockEventType.CEO,
        StockEventType.Restructure,
        StockEventType.Rumor,
        StockEventType.Hype
    };

    private static readonly StockEventType[] IndustryEvents =
    {
        StockEventType.Demand,
        StockEventType.Supply,
        StockEventType.WorldEvent,
        StockEventType.Regulation,
        StockEventType.InterestRate,
        StockEventType.Inflation
    };

    private static string Effect(string passthrough, Action action)
    {
        action();
        return passthrough;
    }

    private static string DescriptionForCompanyEvent(Company company, StockEventType type, bool isPositive)
        => (type, isPositive) switch
        {
            (StockEventType.Dividend, true) => $"{company} has declared a dividend",
            (StockEventType.Dividend, false) => $"{company} has cut its dividend",

            (StockEventType.Split, true) => $"{company} has split its stock",
            (StockEventType.Split, false) => $"{company} has consolidated its stock",

            (StockEventType.Merger, true) => $"A merger has been successfully completed by {company}",
            (StockEventType.Merger, false) => $"A merger attempt by {company} has failed",

            (StockEventType.Acquisition, true) => $"Another company has been acquired by {company}",
            (StockEventType.Acquisition, false) => $"{company} has been taken over by another company",

            (StockEventType.Financial, true) => $"Bankruptcy has been declared by {company}",
            (StockEventType.Financial, false) => $"Record profits have been reported by {company}",

            (StockEventType.IPO, true) => $"The public market now includes {company}",
            (StockEventType.IPO, false) => $"{company} has transitioned to a private entity",

            (StockEventType.CEO, true) => Effect($"A new CEO has been welcomed at {company}",
                () => company.CEO = Company.GenerateCEOName()),
            (StockEventType.CEO, false) => Effect($"The CEO of {company}, {company.CEO} has been fired",
                () => company.CEO = Company.GenerateCEOName()),

            (StockEventType.Restructure, true) => $"{company} has announced a restructure",
            (StockEventType.Restructure, false) => $"Layoffs have been announced by {company}",

            (StockEventType.Rumor, true) => $"Some promising rumors about {company} have been circulating",
            (StockEventType.Rumor, false) => $"It has been rumored that {company} is in trouble",

            (StockEventType.Hype, true) => $"There's a lot of positive buzz around {company}",
            (StockEventType.Hype, false) => $"Interest in {company} is waning",
            
            _ => "Unknown company event"
        };

    private static string DescriptionForIndustryEvent(Industry industry, StockEventType type, bool isPositive)
        => (type, isPositive) switch
        {
            (StockEventType.Demand, true) => $"Demand for {industry.ToFriendlyString()} is on the rise",
            (StockEventType.Demand, false) => $"{industry.ToFriendlyString()}'s demand is falling",
                
            (StockEventType.Supply, true) => $"The market is flooded with {industry.ToFriendlyString()} products",
            (StockEventType.Supply, false) => $"Supply shortages are affecting {industry.ToFriendlyString()}",
                
            (StockEventType.WorldEvent, true) => $"A natural disaster has led to a global shortage of {industry.ToFriendlyString()} products",
            (StockEventType.WorldEvent, false) => $"After a natural disaster, {industry.ToFriendlyString()} is a low priority",
                
            (StockEventType.Regulation, true) => $"New regulations have been passed that favor {industry.ToFriendlyString()}",
            (StockEventType.Regulation, false) => $"{industry.ToFriendlyString()} is facing increased regulation and scrutiny",
                
            (StockEventType.InterestRate, true) => $"Interest rates have been lowered, benefiting {industry.ToFriendlyString()}",
            (StockEventType.InterestRate, false) => $"Interest rates are rising, negatively impacting {industry.ToFriendlyString()}",
                
            (StockEventType.Inflation, true) => $"Inflation is driving up prices for {industry.ToFriendlyString()}",
            (StockEventType.Inflation, false) => $"Inflation is increasing the costs of {industry.ToFriendlyString()} production",
            
            _ => "Unknown industry event"
        };

    public static StockEvent GenerateRandomEventForCompany(DbSet<Company> companies)
    {
        Random random = Global.Random;
        StockEvent stockEvent = new();

        stockEvent.Date = DateTime.Now;
        stockEvent.Company = companies.AsEnumerable().OrderBy(x => Guid.NewGuid()).First();
        stockEvent.Type = CompanyEvents[random.Next(CompanyEvents.Length)];
        stockEvent.IsPositive = random.Next(0, 2) == 0;
        stockEvent.Weight = GenerateWeight(random, stockEvent.Company);

        stockEvent.Description = DescriptionForCompanyEvent(stockEvent.Company, stockEvent.Type, stockEvent.IsPositive);

        return stockEvent;
    }
    
    public static StockEvent GenerateRandomEventForIndustry()
    {
        Random random = Global.Random;
        StockEvent stockEvent = new();

        stockEvent.Date = DateTime.Now;
        stockEvent.Industry = (Industry)random.Next(0, Enum.GetValues<Industry>().Length);
        stockEvent.Type = IndustryEvents[random.Next(IndustryEvents.Length)];
        stockEvent.IsPositive = random.Next(0, 2) == 0;
        stockEvent.Weight = GenerateWeight(random);

        stockEvent.Description = DescriptionForIndustryEvent(stockEvent.Industry.Value, stockEvent.Type, stockEvent.IsPositive);

        return stockEvent;
    }

    private static decimal GenerateWeight(Random random, Company company = null)
    {
        int contraversiality = (int)random.RandomGaussian((double)(100 - (company?.Controversy * 10) ?? 30), 5);
        contraversiality = Math.Clamp(contraversiality, 3, 100);
        bool skyfall = random.Next(contraversiality) == 0;
        return (decimal)random.RandomGaussian(0, skyfall ? 0.03 : 0.003);
    }
}

public enum StockEventType
{
    // Company events
    Dividend,
    Split,
    Merger,
    Acquisition,
    Financial,
    IPO,
    CEO,
    Restructure,
    Rumor,
    Hype,

    // Industry events
    Demand,
    Supply,
    WorldEvent,
    Regulation,
    InterestRate,
    Inflation,
}