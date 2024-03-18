using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ClangenNET;

/// <summary>
/// A partial class, containing all information of the games execution aswell as types within the game.
/// </summary>
public static partial class Runtime
{
    /// <summary>
    /// Define a method to be called when a certain type or type interface is found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Interfaces"></param>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class InternalLoader(Type BaseType, params Type[] Interfaces) : Attribute
    {
        public readonly Type Type = BaseType;
        public readonly Type[] Interfaces = Interfaces;
    }

    private static byte[] InternalAssetHash = [];
    private static Assembly[] InternalAssemblies = [];

    public const byte MAJOR = 0;
    public const byte MINOR = 0;
    public const ushort REVISION = 0;

    /// <summary>
    /// The build of this version of the game
    /// </summary>
    public const uint BUILD = (MAJOR << 24) | (MINOR << 16) | REVISION;

    /// <summary>
    /// A nice little label that represents the current Major/Minor update!
    /// </summary>
    public const string VERSION_LABEL = "Primordial Soup";

    /// <summary>
    /// The directory of this games execution.
    /// </summary>
    public static readonly DirectoryInfo RootDirectory;

    /// <summary>
    /// Readonly collection of all paths used by the base game.
    /// </summary>
    public static readonly ReadOnlyCollection<FileInfo> RootPaths;

    /// <summary>
    /// Readonly collection of all loaded assemblies.
    /// </summary>
    public static ReadOnlyCollection<Assembly> Assemblies { get => InternalAssemblies.AsReadOnly(); }

    /// <summary>
    /// Byte array representing SHA256 Hash of all of the base game files found within exclusive Common.
    /// </summary>
    public static byte[] AssetHash { get => InternalAssetHash; }


    static Runtime()
    {
        RootPaths = new ((RootDirectory = new(".")).EnumerateFiles("*.*", SearchOption.AllDirectories).ToArray());
    }


    public static void Create(ClangenNetGame Game)
    {
        Assembly[] ExecutionAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        // ^FIX - Doesnt allow for external assemblys, need to finalise mod format first and fully look at security risks

        Task<byte[]> HashTask;
        SHA256 Hasher = null;

        try
        {
            Console.WriteLine("Hashing . . .");
            Hasher = SHA256.Create();
            HashTask = Hasher.ComputeHash(RootDirectory, RootPaths);
        }
        catch
        {
            Hasher?.Dispose();
            throw; // TEMPORARY DEBUG
        }

        

        try
        {  // FOLLOWING CODE PATTERN IS VERY SCUFFED, will probably switch to another design pattern or a source generator later on.
            Console.WriteLine("Injecting . . .");
            Assembly Assembly;
            Type[] Types;
            Type Type;

            Dictionary<Type, List<MethodInfo>> TempLoaders = [];
            InternalLoader LoaderInfo;

            foreach (var Method in typeof(Runtime).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
            {
                if ((LoaderInfo = Method.GetCustomAttribute<InternalLoader>()) is null)
                    continue;

                if (!TempLoaders.TryGetValue(LoaderInfo.Type, out var Methods))
                    TempLoaders[LoaderInfo.Type] = Methods = [];

                Methods.Add(Method);
            }

            Dictionary<Type, MethodInfo[]> Loaders = [];
            foreach (var (Key, Value) in TempLoaders)
                Loaders[Key] = [.. Value];

            List<MethodInfo> PossibleMethods = new (5);

            for (int I = 0; I < ExecutionAssemblies.Length; I++)
            {
                Types = (Assembly = ExecutionAssemblies[I]).GetTypes();

                for (int K = 0; K < Types.Length; K++)
                {
                    Type = Types[K];

                    if (Type.IsInterface | Type.IsAbstract) // Theres no scenario where these are needed
                        continue;

                    PossibleMethods.Clear();

                    if (Loaders.TryGetValue(Type, out var Methods))
                        PossibleMethods.AddRange(Methods);

                    foreach (Type Interface in Type.GetInterfaces())
                        if (Loaders.TryGetValue(Interface, out Methods))
                            PossibleMethods.AddRange(Methods);

                    for (int J = 0; J < PossibleMethods.Count; J++)
                        PossibleMethods[J].Invoke(Type, [Type]);
                    
                }
            }

            InternalAssemblies = ExecutionAssemblies; // Needs to be added at the end, so that if we alter Assemblies in any way it'll persist
        }
        finally
        {
            HashTask.Wait();
            if ((InternalAssetHash = HashTask.Result).Length == 0)
                Console.WriteLine("Hashing failed");

            HashTask.Dispose();
            Hasher.Dispose();
        }
    }
}