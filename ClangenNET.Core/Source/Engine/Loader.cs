using System;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;

using static ClangenNET.Utility;

namespace ClangenNET;

/// <summary>
/// A static class, containing all information of the games execution aswell as types within the game.
/// </summary>
public static partial class Context
{
    /// <summary>
    /// A collection of all loaded assemblies.
    /// </summary>
    public static readonly ReadOnlyMemory<Assembly> Assemblies;

    /// <summary>
    /// A SHA256 Hash of all of the base game files found within Common, not including additions from seperate Mod folders
    /// </summary>
    public static readonly ReadOnlyMemory<byte> AssetHash;


    static Context()
    {
        // FIX - Doesnt allow for external assemblys, need to finalise mod format first and fully look at security risks
        Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();

        using (var Algorithm = SHA256.Create())
        {
            AssetHash = ComputeHash(Algorithm, new DirectoryInfo("Common"), SearchOption: SearchOption.AllDirectories).GetAwaiter().GetResult();
        }

        Context.Assemblies = new (Assemblies); // Added at the end, so that if we alter Assemblies in any way it'll persist

        Load(Assemblies);
    }

    internal static partial void Load(Assembly[] Assemblies);

    public static World ThisWorld { get; private set; } = null;
}
