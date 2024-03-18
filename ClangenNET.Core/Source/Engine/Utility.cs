using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using static ClangenNET.MathEx;

namespace ClangenNET;

/// <summary>
/// Represents a dynamic <see cref="Version"/> i.e a Version created from a game save.
/// </summary>
public readonly struct Snapshot(byte Major, byte Minor, ushort Revision)
{
    public readonly byte Major = Major;
    public readonly byte Minor = Minor;
    public readonly ushort Revision = Revision;

    /// <summary>
    /// An unsigned integer summarising the version; its useful in save data and error logging
    /// </summary>
    public readonly uint Build = (uint)((Major << 24) | (Minor << 16) | Revision);

    /// <summary>
    /// Create a Snapshot from a string representation i.e "0.1.12"
    /// </summary>
    public static Snapshot FromString(string Version)
    {
        string[] Sections = Version.Split('.');

        if (Sections.Length != 3)
            throw new ArgumentException("Version was malformed, must be in format of MAJOR.MINOR.REVISION", nameof(Version));

        return new Snapshot(byte.Parse(Sections[0]), byte.Parse(Sections[1]), ushort.Parse(Sections[2]));
    }

    /// <summary>
    /// Create a Snapshot from an unsigned build number i.e 65548 (0.1.12)
    /// </summary>
    public static Snapshot FromBuild(uint Build) => new (
        (byte)((Build & 0xFF000000) >> 24), (byte)((Build & 0x00FF0000) >> 16), (ushort)(Build & 0xFFFF)
    );

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
/// Helper class to manage and organise unicode 
/// </summary>
public static class Unicode
{
    // Character sets provided from https://jrgraphix.net/r/Unicode/
    // NOTE: For any invisible characters, use \uXXXX where the 4 Xs are hex characters

    /// <summary>
    /// Latin characters -> 0x0020 to 0x007F
    /// </summary>
    public const string Latin = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007F";

    /// <summary>
    /// Latin-1 Suppliment characters -> 0x00A0 to 0x00FF
    /// </summary>
    public const string Latin1 = "\u00A0¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";

    /// <summary>
    /// First segment of expanded latin characters -> 0x0100 to 0x017F 
    /// </summary>
    public const string LatinExpandedA = "ĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤ"
        + "ťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſ";

    /// <summary>
    /// Second segment of expanded latin characters -> 0x0180 to 0x024F 
    /// </summary>
    public const string LatinExpandedB = "ƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤ"
        + "ǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉ"
        + "ɊɋɌɍɎɏ";
}


public readonly struct CharRange(ushort Lower, ushort Upper)
{
    public static readonly CharRange Latin = new (0x0020, 0x007F);
    public static readonly CharRange Latin1 = new (0x00A0, 0x00FF);
    public static readonly CharRange LatinExpandedA = new (0x0100, 0x017F);
    public static readonly CharRange LatinExpandedB = new (0x0180, 0x024F);

    public readonly ushort Lower = Lower, Upper = Upper;
}

/// <summary>
/// Helper class containing many specific but useful extensions.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Generate a hash from a given collection of files using their relative path to a given directory.
    /// </summary>
    public static async Task<byte[]> ComputeHash(this HashAlgorithm Algorithm, DirectoryInfo RootDirectory, ReadOnlyCollection<FileInfo> Files)
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

                using FileStream FileStream = File.OpenRead();
                await FileStream.CopyToAsync(CryptoStream);
            }

            CryptoStream.FlushFinalBlock();
        }

        return Algorithm.Hash ?? [];
    }

    /// <summary>
    /// Converts a stream to an array of bytes.
    /// </summary>
    public static byte[] ToByteArray(this Stream Stream)
    {
        byte[] Bytes;

        if (Stream.CanSeek && Stream.Length == Stream.Position)
            Stream.Seek(0, SeekOrigin.Begin);
        
        using (MemoryStream MemoryStream = new ())
        {
            Stream.CopyTo(MemoryStream); Bytes = MemoryStream.ToArray();
        }

        return Bytes;
    }

    /// <summary>
    /// Clamp a comparable value between two given bounds.
    /// </summary>
    /// <param name="Value">The value to clamp.</param>
    /// <param name="Minimum">Lower Bound</param>
    /// <param name="Maximum">Upper Bound</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult ClampTo<TResult>(this TResult Value, TResult Minimum, TResult Maximum) where TResult : notnull, IComparable<TResult>
    {
        return Value.CompareTo(Minimum) < 0 ? Minimum : (Value.CompareTo(Maximum) > 0 ? Maximum : Value);
    }
}


/// <summary>
/// Helper class containing many bodys for niche cases or miscellaneous purpose.
/// </summary>
public static class Utility
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
}