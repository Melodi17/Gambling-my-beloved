namespace Gambling_my_beloved;

public class Global
{
    public static readonly int Seed = Environment.TickCount * 31;
    public static Random Random = new(Seed);
}