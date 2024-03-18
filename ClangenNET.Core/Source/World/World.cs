using System;
using System.IO;

namespace ClangenNET;

public static partial class Runtime
{
    public static World ThisWorld { get; private set; } = null;
}

public enum GamemodeType : byte
{
    Classic, Expanded, Cruel
}

public enum Season : byte
{
    Summer, Autumn, Winter, Spring
}



public class World
{
    public readonly FileInfo File;


    public readonly Season Season;


    public readonly GamemodeType GameMode;


    public uint Moon { get; private set; } = 0;


    public World(FileInfo File, Season Season, GamemodeType GameMode)
    {
        this.File = File;
        this.Season = Season;
        this.GameMode = GameMode;
    }


    public void Save()
    {
        byte[] StaticChunk = [
            Runtime.MAJOR, Runtime.MINOR, Runtime.REVISION & 0xFF, (Runtime.REVISION & 0xFF00) >> 8, // Version data
            (byte)GameMode, (byte)Season
        ];


        using StreamWriter Stream = new(File.FullName, false, System.Text.Encoding.Unicode, 65536);


        Stream.Write(StaticChunk);
    }



    public static World Load(ReadOnlySpan<byte> Data)
    {
        throw new NotImplementedException();
    }
}