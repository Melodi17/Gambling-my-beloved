using System.Reflection;

namespace Gambling_my_beloved.Models;

public enum Industry
{
    [Friendly("Technology")] Technology,
    [Friendly("Finance")] Finance,
    [Friendly("Healthcare")] Healthcare,
    [Friendly("Energy")] Energy,
    [Friendly("Consumer Goods")] ConsumerGoods,
    [Friendly("Services")] Services,
    [Friendly("Basic Materials")] BasicMaterials,
    [Friendly("Industrial Goods")] IndustrialGoods,
    [Friendly("Utilities")] Utilities,
    [Friendly("Luxury Goods")] LuxuryGoods,
    [Friendly("Automotives")] Automotives,
    [Friendly("Entertainment")] Entertainment
}

public class FriendlyAttribute : Attribute
{
    public string FriendlyName { get; }

    public FriendlyAttribute(string friendlyName)
    {
        this.FriendlyName = friendlyName;
    }
}

public static class FriendlyExtensions
{
    public static string GetFriendly<T>(this T value) where T : Enum
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        FriendlyAttribute attribute = (FriendlyAttribute)field.GetCustomAttributes(typeof(FriendlyAttribute), false).FirstOrDefault();
        return attribute?.FriendlyName ?? value.ToString();
    }
    
    public static T FromFriendly<T>(string friendlyName, bool ignoreCase = true) where T : Enum
    {
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            if (value.GetFriendly().Equals(friendlyName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                return value;
        }
    
        throw new ArgumentException($"No {typeof(T).Name} found with friendly name '{friendlyName}'.");
    }
}