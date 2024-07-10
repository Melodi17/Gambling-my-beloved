namespace Gambling_my_beloved.Models;

public enum Industry
{
    Technology,
    Finance,
    Healthcare,
    Energy,
    ConsumerGoods,
    Services,
    BasicMaterials,
    IndustrialGoods,
    Utilities,
    LuxuryGoods,
    Automotives,
    Entertainment
}

public static class IndustryExtensions
{
    public static string ToFriendlyString(this Industry industry)
    {
        return industry switch
        {
            Industry.Technology => "Technology",
            Industry.Finance => "Finance",
            Industry.Healthcare => "Healthcare",
            Industry.Energy => "Energy",
            Industry.ConsumerGoods => "Consumer Goods",
            Industry.Services => "Services",
            Industry.BasicMaterials => "Basic Materials",
            Industry.IndustrialGoods => "Industrial Goods",
            Industry.Utilities => "Utilities",
            Industry.LuxuryGoods => "Luxury Goods",
            Industry.Automotives => "Automotives",
            Industry.Entertainment => "Entertainment",
            _ => throw new ArgumentOutOfRangeException(nameof(industry), industry, null)
        };
    }
}