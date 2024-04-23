using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static ClangenNET.MathEx;

namespace ClangenNET;

public static class GameInfo
{
    public const byte MAJOR = 0;
    public const byte MINOR = 0;
    public const ushort PATCH = 0;

    /// <summary>
    /// The build of this version of the game
    /// </summary>
    public const uint BUILD = (MAJOR << 24) | (MINOR << 16) | PATCH;

    /// <summary>
    /// A nice little label that represents the current Major/Minor update!
    /// </summary>
    public const string VERSION_LABEL = "Primordial Soup";

    public static readonly string VERSION_MAJOR_MINOR_PATCH = $"{MAJOR}.{MINOR}.{PATCH}";

    public static readonly CultureInfo CultureInfo;

    public static readonly TextInfo TextInfo;

    static GameInfo()
    {
        CultureInfo = CultureInfo.InstalledUICulture;
        TextInfo = CultureInfo.TextInfo;
    }
}

/// <summary>
/// Represents a dynamic <see cref="Version"/> i.e a Version created from a game save.
/// </summary>
public readonly struct Snapshot
{
    public readonly byte Major = GameInfo.MAJOR;
    public readonly byte Minor = GameInfo.MINOR;
    public readonly ushort Revision = GameInfo.PATCH;

    /// <summary>
    /// An unsigned integer summarising the version; its useful in save data and error logging
    /// </summary>
    public readonly uint Build = GameInfo.BUILD;

    public Snapshot(byte Major, byte Minor, ushort Revision)
    {
        this.Major = Major;
        this.Minor = Minor;
        this.Revision = Revision;
        this.Build = (uint)((Major << 24) | (Minor << 16) | Revision);
    }

    /// <summary>
    /// Initialise a Snapshot from a string representation i.e "0.1.12", "65548"
    /// </summary>
    public Snapshot(string Version)
    {
        if (uint.TryParse(Version, out uint PotentialBuild))
        {
            this.Major = (byte)(PotentialBuild >> 24);
            this.Minor = (byte)(PotentialBuild >> 16);
            this.Revision = (ushort)(PotentialBuild & 0xFFFF);
            this.Build = PotentialBuild;
            return;
        }

        string[] Sections = Version.Split('.');

        if (Sections.Length != 3)
            throw new ArgumentException("Version was malformed, must be in format of MAJOR.MINOR.REVISION, or a string representation of a build number", nameof(Version));

        this.Major = byte.Parse(Sections[0]);
        this.Minor = byte.Parse(Sections[1]);
        this.Revision = ushort.Parse(Sections[2]);
        this.Build = (uint)((Major << 24) | (Minor << 16) | Revision);
    }

    /// <summary>
    /// Initialise a Snapshot from an unsigned build number i.e 65548 (0.1.12)
    /// </summary>
    public Snapshot(uint Build)  : this((byte)(Build >> 24), (byte)(Build >> 16), (ushort)(Build & 0xFFFF)) { }

    public override string ToString() => $"{Major}.{Minor}.{Revision}";
    public override int GetHashCode() => (int)Build;

    /// <summary>
    /// Check that this Snapshot information is valid.
    /// </summary>
    public bool IsValid() => Major switch
    {
        0 => Minor switch
        {
            0 => Revision switch { <= 0 => true, _ => false, },
            _ => false,
        },
        _ => false,
    };
}

/// <summary>
/// Helper class containing many specific but useful extensions.
/// </summary>
public static class UtilityExtensions
{
    /// <summary>
    /// Generate a hash from a given collection of files using their relative path to a given directory.
    /// </summary>
    public static byte[]? ComputeHash(this HashAlgorithm Algorithm, DirectoryInfo RootDirectory, ReadOnlyCollection<FileInfo> Files)
    {
        int RootDirIndex = RootDirectory.FullName.Length + 1;

        Files = new (Files.OrderBy(i => i.Name).ToArray());


        using (CryptoStream CryptoStream = new (Stream.Null, Algorithm, CryptoStreamMode.Write))
        {
            FileInfo File;
            for (int i = 0; i < Files.Count; i++)
            {
                CryptoStream.Write(
                    Encoding.Unicode.GetBytes((File = Files[i]).FullName[RootDirIndex..]) // Allows for hashes to be the same regardless of file location
                );

                using FileStream FileStream = new(File.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 1048576);
                FileStream.CopyTo(CryptoStream);
            }

            CryptoStream.FlushFinalBlock();
        }

        return Algorithm.Hash;
    }

    /// <summary>
    /// Converts a stream to an array of bytes.
    /// </summary>
    public static byte[] ToByteArray(this Stream Stream) // TODO add buffer option
    {
        byte[] Bytes;

        if (Stream.CanSeek && Stream.Length == Stream.Position)
            Stream.Seek(0, SeekOrigin.Begin);
        
        using (MemoryStream MemoryStream = new ())
        {
            Stream.CopyTo(MemoryStream); 
            Bytes = MemoryStream.ToArray();
        }

        return Bytes;
    }

    /// <summary>
    /// Merge two dictionaries, with the new overwriting the old.
    /// </summary>
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> Source, IDictionary<TKey, TValue> Other)
    {
        foreach (TKey Key in Other.Keys)
            Source[Key] = Other[Key];
    }

    /// <summary>
    /// Gets the value associated with the specific key and casts it to <typeparamref name="TResult"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the object that implements <see cref="IDictionary{TKey, TValue}"/> contains an element with specified key; otherwise, <see langword="false"/></returns>
    public static bool TryGetValueCast<TKey, TValue, TResult>(this IDictionary<TKey, TValue> Source, TKey Key, [MaybeNullWhen(false)] out TResult Value) where TResult : notnull, TValue
    {
        bool Result;
        Value = (Result = Source.TryGetValue(Key, out TValue? __T)) ? (TResult?)__T : default;
        return Result;
    }

    /// <summary>
    /// Gets the value associated with the specific key and casts it to <typeparamref name="TResult"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the object that implements <see cref="IReadOnlyDictionary{TKey, TValue}"/> contains an element with specified key; otherwise, <see langword="false"/></returns>
    public static bool TryGetValueCast<TKey, TValue, TResult>(this IReadOnlyDictionary<TKey, TValue> Source, TKey Key, [MaybeNullWhen(false)] out TResult Value) where TResult : notnull, TValue
    {
        bool Result;
        Value = (Result = Source.TryGetValue(Key, out TValue? __T)) ? (TResult?)__T : default;
        return Result;
    }

    /// <summary>
    /// Clamp a comparable value between two given bounds.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult ClampTo<TResult>(this TResult Value, TResult Minimum, TResult Maximum) where TResult : notnull, IComparable<TResult>
    {
        return Value.CompareTo(Minimum) < 0 ? Minimum : (Value.CompareTo(Maximum) > 0 ? Maximum : Value);
    }
}


/// <summary>
/// Helper class containing many bodys for niche cases or miscellaneous purpose.
/// </summary>
public static partial class Utility
{
    /// <summary>
    /// A global <see cref="RandomEx"> object to avoid unnessecary creation.
    /// </summary>
    public static readonly RandomEx GlobalNG = new();

    /// <summary>
    /// Run multiple tasks at the same time, with exceptions thrown as needed.
    /// </summary>
    public static async Task RunTasks(params Task[] Tasks)
    {
        Task AllTasks = Task.WhenAll(Tasks);

        try
        {
            await AllTasks; return;
        }
        catch (Exception Exc)
        {
            throw AllTasks.Exception ?? Exc;
        }
    }

    /// <summary>
    /// Run multiple tasks at the same time, with exceptions thrown as needed.
    /// </summary>
    public static async Task<TResult[]> RunTasks<TResult>(params Task<TResult>[] Tasks)
    {
        Task<TResult[]> AllTasks = Task.WhenAll(Tasks);

        try
        {
            return await AllTasks;
        }
        catch (Exception Exc)
        {
            throw AllTasks.Exception ?? Exc;
        }
    }

    public static Microsoft.Xna.Framework.Color ColorFromString(string Input) // FIX -> allow for RGBA, HSV, HEX and CMYK input
    {
        byte[] Fragments = Array.ConvertAll(Input[1..^1].Split(','), byte.Parse);
        return new(Fragments[0], Fragments[1], Fragments[2]);
    }
}