namespace ClangenNET;

internal static class Program
{
    public static void Main()
    {
        using ClangenNetGame Game = new();
        Game.Run();
    }
}