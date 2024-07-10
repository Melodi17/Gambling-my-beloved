﻿using Microsoft.EntityFrameworkCore;

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
}