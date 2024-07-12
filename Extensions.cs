using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Gambling_my_beloved;

public static class Extensions
{
    public static double RandomGaussian(this Random random, double mean, double stdDev)
    {
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal;
    }
    
    public static T RandomElement<T>(this IEnumerable<T> enumerable, Random random)
    {
        return enumerable.ElementAt(random.Next(enumerable.Count()));
    }
    
    public static T RandomRecord<T>(this DbSet<T> dbSet, Random random) where T : class
    {
        int count = dbSet.Count();
        return dbSet.Skip(random.Next(count)).First();
    }
    
    public static T RandomRecord<T>(this IQueryable<T> query, Random random)
    {
        int count = query.Count();
        return query.Skip(random.Next(count)).First();
    }
    
    public static string ToCurrency(this decimal value)
    {
        // round it to a max of 2 decimal places
        // and add a currency symbol
        // make sure to use a negative sign, not brackets
        return "$" + value.ToString("C")
            .Replace("$", "")
            .Replace("(", "-")
            .Replace(")", "");
    }
}