using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using YAXLib.Attributes;

namespace ClangenNET;

public enum GamemodeType : byte
{
    Classic, Expanded, Cruel
}

public enum Season : byte
{
    Summer, Autumn, Winter, Spring
}


public static partial class Content
{
    public static World ThisWorld { get; set; }
}


public class WorldConfig
{
    public static readonly WorldConfig Default = new();

    public class CatGenerationDef
    {
        public float BaseHeterochromiaChance { get; init; } = 1 / 125;

        public double FullwhiteChance { get; init; } = 1 / 20;

        public double RandomPointChance { get; init; } = 1 / (1 << 5);

        public double MaleTortieChance { get; init; } = 1 / (1 << 13);

        public double FemaleTortieChance { get; init; } = 1 / (1 << 4);

        public double WildcardTortieChance { get; init; } = 1 / (1 << 9);

        public double VitiligoChance { get; init; } = 1 / 8; 
    }

    public CatGenerationDef CatGeneration { get; init; } = new();
}



public class World
{
    public static readonly World None = new();


    public WorldConfig Config { get; init; } = WorldConfig.Default;


    public Int128 Seed { get; init; }


    public Season Season { get; init; }


    public GamemodeType GameMode { get; init; }


    public uint Moon { get; private set; } = 0;


    [YAXDontSerialize]
    public string File { get; private set; }

#nullable disable
    private World() {}
#nullable enable

    public World(string File, Season Season, GamemodeType GameMode)
    {
        this.File = File;
        this.Season = Season;
        this.GameMode = GameMode;
    }
}