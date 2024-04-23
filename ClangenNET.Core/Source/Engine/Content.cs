using ClangenNET.Graphics;
using ClangenNET.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Content.Pipeline.Builder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VYaml.Parser;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace ClangenNET;

/// <summary>
/// Represents a one-way bridge from .xml to a C# class or struct.
/// </summary>
/// <typeparam name="TOutput">The class or struct to produce</typeparam>
public abstract class ThrowawayDef<TOutput>
{
    public ThrowawayDef() { }

    public abstract bool Digest(out TOutput Output);
}

/// <summary>
/// Represents a two-way bridge from .xml to a C# class or struct.
/// </summary>
public abstract class Def
{
    public Def() { }
}

# region Defs
[YAXSerializeAs("MetaData")]
internal class ContentPackDef
{
    [YAXAttributeForClass]
    public string? Id { get; set; }

    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? Name { get; set; }

    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? Description { get; set; }

    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? Icon { get; set; }

    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? Thumbnail { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "li")]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string[]? Authors { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "li")]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string[]? SupportedVersions { get; set; }

    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? RootFolder { get; set; }
}

public class ContentPack
{
    public readonly string Id;
    public readonly string Name;
    public readonly string Description;
    public readonly string? Thumbnail;
    public readonly ReadOnlyCollection<string> Authors;
    public readonly ReadOnlyCollection<Snapshot> SupportedVersions;
    public required DirectoryInfo RootFolder { get; init; }


    internal ContentPack(ContentPackDef Payload)
    {
        if (Payload.Id is null)
            throw new Exception("CONTENT PACK ID IS NULL");

        Id = Payload.Id.ToLower();
        Name = Payload.Name ?? "???";
        Description = Payload.Description ?? "";
        Thumbnail = Payload.Thumbnail;
        Authors = new(Payload.Authors ?? []);
        SupportedVersions = new(Array.ConvertAll(Payload.SupportedVersions ?? [], X => new Snapshot(X)));
    }
}

public class GlobalConfig : Def
{
    [YAXDontSerialize]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public int BufferSize_Def { get; init; } = 1048576;

    public uint LastPlayedVersion { get; init; } = 0;

    [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "li")]
    public List<string> ActiveMods { get; init; } = new(["core"]);

    // Utility Related
    public bool CheckForUpdates { get; set; } = true;
    public bool ShowChangelog { get; set; } = true;
    public ushort AutosaveInterval { get; set; } = 15;
    public string Language { get; set; } = "LEnglish";

    // Theme Related
    public bool IsFullscreen { get; set; } = false;
    public bool IsDarkMode { get; set; } = true;
    public bool CustomCursorAllowed { get; set; } = true;
    public bool ShadersAllowed { get; set; } = false;
    public bool GoreAllowed { get; set; } = false;

    // Misc
    public bool DiscordIntegration { get; set; } = false;
}

internal class LocaleMetaData : Def
{
    [YAXSerializeAs("MetaData")]
    internal class RootElement
    {
        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Locale")]
        public LocaleMetaData[]? MetaData { get; set; }
    }

    [YAXAttributeForClass]
    public string Id { get; init; }

    [YAXAttributeForClass]
    public string Path { get; init; }

    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public string? Name { get; init; } = "???";

    [YAXDictionary(EachPairName = "li", KeyName = "Id", ValueName = "Name", SerializeKeyAs = YAXNodeTypes.Attribute, SerializeValueAs = YAXNodeTypes.Attribute)]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public Dictionary<string, string> Aliases { get; init; } = [];

    [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "li")]
    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
    public List<string> Charsets { get; init; } = [];

    [YAXDontSerialize]
    public string ParentFolder;

    [YAXDontSerialize]
    public List<string> Paths = [];

# nullable disable
    public LocaleMetaData() { }
# nullable enable
    public static LocaleMetaData[]? From(TextReader Reader) => Content.DeserializeDef<RootElement>(Reader)?.MetaData;
}

#endregion

public static partial class Content
{
    private const string ErrorConstant = "[ERR_CONSTANT:{0}]";


    private static readonly Dictionary<Type, YAXSerializer> __SerializerCache = [];


    private static readonly ConcurrentDictionary<string, Texture2D> __TextureCache = new();


    private static readonly ConcurrentDictionary<string, Effect> __EffectCache = new();

    /// <summary>
    /// The directory of this games execution.
    /// </summary>
    public static readonly DirectoryInfo BaseDirectory;

    /// <summary>
    /// Readonly collection of all paths used by the base game.
    /// </summary>
    public static readonly ReadOnlyCollection<FileInfo> BasePaths;


    public static readonly DirectoryInfo GameDataPath;


    public static readonly DirectoryInfo SaveDataPath;


    private static GraphicsDeviceManager GraphicsDeviceManager;


    private static SpriteBatchEx Batch;


    private static PipelineManager PipelineManager;

    /// <summary>
    /// Thread-safe dictionary, mapping translation keys to single text values of the current language.
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> TranslationTable = new();

    /// <summary>
    /// Thread-safe dictionary, mapping translation keys to multiple text values of the current language.
    /// </summary>
    private static readonly ConcurrentDictionary<string, string[]> TranslationArrayTable = new();

    /// <summary>
    /// Thread-safe dictionary, holding purely the metadata of each language.
    /// </summary>
    private static readonly ConcurrentDictionary<string, LocaleMetaData> LanguageTable = new();

    /// <summary>
    /// The key of the current-selected language.
    /// </summary>
    private static LocaleMetaData? CurrentLanguage;

    /// <summary>
    /// Byte array representing SHA256 Hash of all of the base game files found within exclusive Common.
    /// </summary>
    public static string? AssetHash { get; private set; }


    public static GlobalConfig Config { get; private set; }


    public static ReadOnlyCollection<ContentPack>? ActivePacks { get; private set; }

#nullable disable
    static Content()
    {
        BaseDirectory = new(".");
        BasePaths = new(BaseDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories).ToArray());
        GameDataPath = new($"{BaseDirectory.FullName}\\GameData");
        SaveDataPath = new($"{BaseDirectory.FullName}\\SaveData");
    }
#nullable enable

    #region Context Creation
    internal static void PrepareContext(GraphicsDeviceManager GraphicsDeviceManager)
    {
        StreamReader Reader;
        StreamWriter Writer;

        // Preprocess certain types
        Assembly[] ExecutionAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        // ^FIX - Doesnt allow for external assemblys, need to finalise mod format first and fully look at any possible security risks
        Assembly Assembly;
        Type[] Types;
        Type Type;

        Console.WriteLine("Creating context . . .");

        for (int I = 0; I < ExecutionAssemblies.Length; I++)
        {
            Types = (Assembly = ExecutionAssemblies[I]).GetTypes();

            for (int K = 0; K < Types.Length; K++)
            {
                Type = Types[K];

                if (Type.IsInterface || Type.IsAbstract) // No conceivable scenario where these are needed here.
                    continue;

                if (Type.GetInterfaces().Any(X => X.IsGenericType && X.GetGenericTypeDefinition() == typeof(ThrowawayDef<>)))
                {
                    __SerializerCache[Type] = new(Type);
                }
                else if (typeof(Def).IsAssignableFrom(Type))
                {
                    __SerializerCache[Type] = new(Type);
                }
            }
        }

        // GlobalConfig Loading
        string Path = $"{SaveDataPath.FullName}\\Config\\Global.xml";
        GlobalConfig? Config = null;

        try
        {
            if (File.Exists(Path))
            {
                using (Reader = new(Path))
                    Config = DeserializeDef<GlobalConfig>(Reader);
            }

            if (Config is null)
                throw new Exception("Failed To Load Config");
        }
        catch (Exception)
        {
            Console.WriteLine("GlobalConfig failure to load, rewriting.");
            Content.Config = new();
        }
        finally
        {
            using (Writer = new(Path))
                SerializeDef(Writer, Config);
        }

        // Content Metadata Loading
        DirectoryInfo[] ModDirectories = GameDataPath.EnumerateDirectories().ToArray();

        List<ContentPack> Packs = [];
        ContentPackDef? Def;
        for (int I = 0; I < ModDirectories.Length; I++)
        {
            Path = $"{ModDirectories[I].FullName}\\About.xml";

            if (!File.Exists(Path))
            {
                Console.WriteLine($"WARNING : Found Mod at \"{ModDirectories[I].FullName}\" with no About.xml, mod has not been loaded.");
                continue;
            }

            using (Reader = new(Path))
            {
                if ((Def = DeserializeDef<ContentPackDef>(Reader)) is not null)
                {
                    Packs.Add(new(Def) { RootFolder = ModDirectories[I] });
                }
            }

            Console.WriteLine($"Found Mod at \"{ModDirectories[I].FullName}\" . . .");
        }

        PipelineManager = new("", "", "")
        {
            Profile = GraphicsDeviceManager.GraphicsDevice.GraphicsProfile,
        };

        ActivePacks = Packs.AsReadOnly();

        Content.GraphicsDeviceManager = GraphicsDeviceManager;
        Batch = new SpriteBatchEx(GraphicsDeviceManager.GraphicsDevice);
    }

    internal static void CreateContext()
    {
        // Start hashing task
        Task<byte[]?> HashTask;
        SHA256? Hasher = null;

        try
        {
            Console.WriteLine("Hashing . . .");
            Hasher = SHA256.Create();
            HashTask = Task.Run(() => Hasher.ComputeHash(BaseDirectory, BasePaths));
        }
        catch
        {
            Hasher?.Dispose();
            throw; // TEMPORARY DEBUG
        }

        if (ActivePacks is null || ActivePacks.Count < 0)
            return;

        // Load Content
        string RootPath;
        string FilePath;
        
        foreach (ContentPack Pack in ActivePacks) // OPTIMIZE -> could be modularized with service registers for directories or specific files
        {
            LoadingScreen.SetText($"Loading pack \"{Pack.Name}\" . . .");
            Console.WriteLine($"Loading pack \"{Pack.Name}\" from \"{Pack.RootFolder}\" . . .");

            if (Path.Exists(FilePath = $"{Pack.RootFolder}\\Locale\\Languages.xml"))
            {
                LoadLanguages(FilePath);
            }
            if (Path.Exists(RootPath = $"{Pack.RootFolder}\\Defs"))
            {
                if (Path.Exists($"{RootPath}\\Cats"))
                {
                    if (Path.Exists($"{RootPath}\\Cats\\Tints"))
                    {
                        LoadTints(Directory.EnumerateFiles($"{RootPath}\\Cats\\Tints"));
                    }
                    if (Path.Exists($"{RootPath}\\Cats\\Poses"))
                    {
                        LoadPoses(Directory.EnumerateFiles($"{RootPath}\\Cats\\Poses"));
                    }
                    if (Path.Exists($"{RootPath}\\Cats\\Pelts"))
                    {
                        LoadPelts(Directory.EnumerateFiles($"{RootPath}\\Cats\\Pelts"));
                    }
                }
            }
        }

        // Cleanup
        try
        {
            LoadingScreen.SetText("Hashing . . .");
            HashTask.Wait();
            byte[]? HashResult = HashTask.Result;

            if (HashResult is null)
            {
                Console.WriteLine("Hashing failed, achievements unavailable."); // future hints ;)
            }
            else
            {
                AssetHash = new (Array.ConvertAll(HashResult, K => (char)K));
            }
        }
        finally
        {
            HashTask.Dispose();
            Hasher.Dispose();
            Console.WriteLine("Hashing completed!");
        }
    }

    internal static void FinaliseContext()
    {
        // Remove un-needed serializers
        foreach (Type Key in __SerializerCache.Keys)
        {
            if (Key.GetInterfaces().Any(X => X.IsGenericType && X.GetGenericTypeDefinition() == typeof(ThrowawayDef<>)))
                __SerializerCache.Remove(Key);
        }
    }
    #endregion Context

    #region Loads
    /// <summary>
    /// Load a languages file. If two languages have the same Id, the newest language will add to any list values.
    /// </summary>
    /// <param name="Path">The path of the file</param>
    internal static void LoadLanguages(string Path)
    {
        LocaleMetaData[] LocaleDefs;
        LocaleMetaData LocaleDef;

        using (StreamReader Reader = new(Path))
            LocaleDefs = LocaleMetaData.From(Reader) ?? [];

        for (int I = 0; I < LocaleDefs.Length; I++)
        {
            LocaleDef = LocaleDefs[I];

            if (LanguageTable.TryGetValue(LocaleDef.Id, out LocaleMetaData? ExistingDef))
            {
                if (!ExistingDef.Paths.Contains(LocaleDef.Path))
                    ExistingDef.Paths.Add(LocaleDef.Path);
                if (LocaleDef.Aliases is not null)
                    ExistingDef.Aliases.AddRange(LocaleDef.Aliases);
                if (LocaleDef.Charsets is not null)
                    ExistingDef.Charsets.AddRange(LocaleDef.Charsets);
            }
            else
            {
                LocaleDef.ParentFolder = Path.Remove(Path.LastIndexOf('\\'));
                LocaleDef.Paths = [LocaleDef.Path];
                LanguageTable[LocaleDef.Id] = LocaleDef;
            }
        }
    }
    #endregion

    #region Drawing
    internal static void Draw()
    {
        GraphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);
        CurrentScene?.Draw(Batch);
    }

    public static void DrawTexture(string Id, Rectangle Destination, Rectangle? Source, Color Color) 
        => Batch.Draw(GetTexture(Id), Destination, Source, Color);

    public static void Fill(Color Fill)
    {
        GraphicsDeviceManager.GraphicsDevice.Clear(Fill);
    }
    #endregion

    #region Getters
    public static Texture2D GetTexture(string Id)
    {
        Id = ResolvePath($"Resources\\{Id}");

        if (__TextureCache.TryGetValue(Id, out Texture2D? Texture))
        {
            return Texture;
        }
        
        using FileStream FileStream = new(Id, FileMode.Open, FileAccess.Read, FileShare.Read);
        return __TextureCache[Id] = Texture2D.FromStream(GraphicsDeviceManager.GraphicsDevice, FileStream);
    }

    public static Effect GetShader(string Id)
    {
        byte[] Content = File.ReadAllBytes(ResolvePath($"Resources\\Shaders\\{Id}.mgfxo"));
        return new Effect(GraphicsDeviceManager.GraphicsDevice, Content);
    }

    public static Font GetFont(string Id, uint FontSize = 30)
    {
        return new Font(ResolvePath($"Resources\\Fonts\\{Id}.ttf"), FontSize);
    }
    #endregion

    #region Setters
    /// <summary>
    /// Set the current game language to a given key. Returns 0 on success, negative number on failure.
    /// </summary>
    internal static int SetLanguage(string Key)
    {
        static Task LoadFile(string Path)
        {
            using (FileStream Stream = File.OpenRead(Path))
            {
                if (Stream.Length < 1)
                    return Task.CompletedTask;

                YamlParser Parser = YamlParser.FromBytes(File.ReadAllBytes(Path));
                List<string> NewStringArray = [];
                string Key;

                while (Parser.Read())
                {
                    if (Parser.CurrentEventType == ParseEventType.MappingStart)
                    {
                        Parser.Read();
                        if (CurrentLanguage is not null && (Parser.ReadScalarAsString() ?? "") != CurrentLanguage.Id)
                            continue;
                        Parser.Read();

                        while (Parser.CurrentEventType != ParseEventType.MappingEnd)
                        {
                            Key = Parser.ReadScalarAsString() ?? "";

                            if (Parser.CurrentEventType == ParseEventType.SequenceStart)
                            {
                                NewStringArray.Clear();

                                Parser.Read();
                                while (Parser.CurrentEventType != ParseEventType.SequenceEnd) { NewStringArray.Add(Parser.ReadScalarAsString() ?? ""); }
                                Parser.Read();

                                TranslationArrayTable[Key] = [.. NewStringArray];
                            }
                            else
                            {
                                TranslationTable[Key] = Parser.ReadScalarAsString() ?? "";
                            }
                        }

                        Parser.Read();
                    }
                }
            }

            return Task.CompletedTask;
        }

        if (!LanguageTable.TryGetValue(Key, out LocaleMetaData? Metadata))
            return -1; // Metadata doesnt exist
        else if (Metadata.Path is null)
            return -2; // Metadata path wasnt given
        if (CurrentLanguage == Metadata)
            return -3; // Metadata already current in use

        CurrentLanguage = Metadata;

        foreach (string Path in Metadata.Paths)
        {
            string[] Paths = Directory.GetFiles($"{Metadata.ParentFolder}\\{Path}", "*.yml", SearchOption.AllDirectories);

            Task[] Tasks = new Task[Paths.Length];
            for (int i = 0; i < Paths.Length; i++)
                Tasks[i] = LoadFile(Paths[i]);

            Utility.RunTasks(Tasks).GetAwaiter().GetResult();
        }

        return 0;
    }
    #endregion

    #region Helpers
    internal static TDef? DeserializeDef<TDef>(TextReader Reader) where TDef : notnull
    {
        if (!__SerializerCache.TryGetValue(typeof(TDef), out var Serializer))
            __SerializerCache[typeof(TDef)] = Serializer = new(typeof(TDef));

        return (TDef?)Serializer.Deserialize(Reader);
    }

    internal static void SerializeDef<TDef>(TextWriter Writer, TDef Def)
    {
        if (!__SerializerCache.TryGetValue(typeof(TDef), out var Serializer))
            __SerializerCache[typeof(TDef)] = Serializer = new(typeof(TDef));

        Serializer.Serialize(Def, Writer);
    }
    #endregion

    #region Types 
    /// <summary>
    /// Represents a key to a given string.
    /// </summary>
    public readonly struct TranslationKey
    {
        /// <summary>
        /// The index to get, cant be zero.
        /// </summary>
        private readonly uint Index = 0;

        /// <summary>
        /// The actual translation key.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// Whether or not this Key points to an array.
        /// </summary>
        public readonly bool IsArray;

        /// <summary>
        /// Amount of text elements in array. If Key points to a non-array, it will return 1.
        /// </summary>
        public int Count => IsArray && GetArray(out string[] Result) ? Result.Length : 1;

        /// <summary>
        /// Get text element in array by index. If Key points to a non-array, it will raise an error for anything but 0.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Index given is bigger than array size or this Key points to single text and bigger than 0</exception>
        public string this[int Index]
        {
            get
            {
                if (IsArray && GetArray(out string[] Results))
                    return Results[Index];

                if (!IsArray && Index == 0 && GetSingle(out string? Result))
                    return Result ?? "";

                throw new IndexOutOfRangeException("Index is bigger than array or Key is non-array"); // TEMPORARY
            }
        }

        public TranslationKey(string Key)
        {
            this.Key = Key;
            this.IsArray = TranslationArrayTable.ContainsKey(Key);
        }

        public TranslationKey(string Key, uint Index)
        {
            this.Key = Key;
            this.Index = Index;
            this.IsArray = TranslationArrayTable.TryGetValue(Key, out string[]? Array);

            if ((0 < this.Index && !IsArray) || (Array is not null && Array.Length < Index))
                throw new ArgumentOutOfRangeException(nameof(Index), "Index is bigger than array length, or Key is non-array");
        }

        private bool GetArray(out string[] Result)
        {
            TranslationArrayTable.TryGetValue(Key, out string[]? PossibleResult);
            return 0 < (Result = PossibleResult ?? []).Length;
        }
        private bool GetSingle(out string? Result) => TranslationTable.TryGetValue(Key, out Result);

        /// <summary>
        /// Get the translated string. If key points to translation array, it will choose randomly. If key points to nowhere, returns key.
        /// </summary>
        public override string ToString()
        {
            if (IsArray && GetArray(out string[] ValueArray))
                return (Index > 0) ? ValueArray[Index - 1] : Utility.GlobalNG.Choose(ValueArray);
            if (!IsArray && GetSingle(out string? Value) && Value is not null)
                return Key;

            return Key;
        }

        /// <summary>
        /// Evaluate a string using predefined Key syntax.
        /// </summary>
        public static implicit operator TranslationKey(string KeyString) => FromSpan(KeyString);

        public static TranslationKey FromSpan(ReadOnlySpan<char> KeyString)
        {
            ReadOnlySpan<char> Key = KeyString;
            uint ArrayIndex = 0;

            if (Key[^1] == ']')
            {
                for (int I = Key.Length - 1; I > 0; I--)
                {
                    if (Key[I] == '[')
                    {
                        if ((ArrayIndex = uint.Parse(Key[(I + 1)..^1])) == 0)
                            throw new ArgumentException("Key array indexes can only be >0", nameof(KeyString));

                        Key = Key[..I];
                        break;
                    }
                }
            }

            return ArrayIndex > 0 ? new TranslationKey(new(Key), ArrayIndex) : new TranslationKey(new(Key));
        }

        public static bool KeyExists(string Key) => TranslationTable.ContainsKey(Key) || TranslationArrayTable.ContainsKey(Key);
        public static bool KeyExistsAsSingle(string Key) => TranslationTable.ContainsKey(Key);
        public static bool KeyExistsAsArray(string Key) => TranslationArrayTable.ContainsKey(Key);
    }

    /// <summary>
    /// Represents an inclusive, continuous range of characters.
    /// </summary>
    public readonly struct UnicodeRange
    {
        // Character sets provided from https://jrgraphix.net/r/Unicode/
        public static readonly UnicodeRange Latin = new(0x0020, 0x007F);
        public static readonly UnicodeRange Latin1 = new(0x00A0, 0x00FF);
        public static readonly UnicodeRange LatinExpandedA = new(0x0100, 0x017F);
        public static readonly UnicodeRange LatinExpandedB = new(0x0180, 0x024F);

        public readonly ushort Lower;
        public readonly ushort Upper;
        public readonly ushort Range;

        public UnicodeRange(ushort Lower, ushort Upper)
        {
            if (Lower > Upper) // Ensure Lower is actually Lower than Upper
                (Lower, Upper) = (Upper, Lower);

            Range = (ushort)((this.Upper = Upper) - (this.Lower = Lower));
        }

        public override string ToString()
        {
            if (Lower == Upper) return Lower.ToString();

            char[] Data = new char[Range];
            for (int I = 0; I < Range; I++)
                Data[I] = (char)(Lower + I);

            return new(Data);
        }
    }
    #endregion

    // v OPTIMISE -> most are using pure strings, but can be optimized into spans, unsafe/marshalled code or their own struct types
    #region Utility -> string related 
    public static string ResolvePath(string Path)
    {
        string? NewPath = null;

        if (Path.StartsWith('.'))
            NewPath = $"{GameDataPath}\\{Path}";

        for (int Index = 0; Index < Path.Length; Index++)
        {
            switch (Path[Index])
            {
                case ':':
                    GameInfo.TextInfo.ToLower(Path[..Index]);

                    if (ActivePacks is null)
                        break;
                    
                    foreach (ContentPack Pack in ActivePacks)
                    {
                        if (Pack.RootFolder is not null)
                        {
                            Console.WriteLine(Pack.RootFolder);
                            NewPath = Path.Remove(0, Index).Insert(0, Pack.RootFolder.FullName);
                            break;
                        }
                    }
                    
                    continue;

                default:
                    continue;
            }
        }

        if (NewPath is null && ActivePacks is not null)
        {
            foreach (ContentPack Pack in ActivePacks)
            {
                if (Pack.RootFolder is not null && File.Exists(NewPath = $"{Pack.RootFolder.FullName}\\{Path}"))
                {
                    break;
                }
            }
        }

        return NewPath ?? Path;
    }

    public static string GetConstant(string Text) => Text switch
    {
        "GameVersion" => GameInfo.VERSION_MAJOR_MINOR_PATCH,
        _ => string.Format(ErrorConstant, Text)
    };

    public static string ParseQuery(string Query)
    {
        throw new NotImplementedException();
    }

    public static string ParseString(string Text, int Index = 0)
    {
        static void ParseTranslationKey(int StartIndex, ref int Index, ref string Text)
        {
            while (true)
            {
                switch (Text[++Index])
                {
                    case ')':
                        if (Text[Index - 1] == '\\')
                            break;

                        if (TranslationKey.KeyExists(Text[(2 + StartIndex)..Index]))
                            Text = Text.Remove(StartIndex, 1 + Index - StartIndex).Insert(StartIndex, ((TranslationKey)Text[(2 + StartIndex)..Index]).ToString());

                        return;

                    default:
                        continue;
                }

                if (Index == Text.Length - 1)
                    return;
            }
        }
        static void ParseQueryConstant(int StartIndex, ref int Index, ref string Text)
        {
            while (true)
            {
                switch (Text[++Index])
                {
                    case ']':

                        if (Text[Index - 1] == '\\')
                            break;
                        string Constant = GetConstant(Text[(1 + StartIndex)..Index]);
                        Text = Text.Remove(StartIndex, 1 + Index - StartIndex).Insert(StartIndex, Constant);
                        Index = StartIndex + Constant.Length;
                        return;

                    default:
                        continue;
                }

                if (Index == Text.Length - 1)
                    return;
            }
        }


        while (true)
        {
            switch (Text[++Index])
            {
                case '$':
                    if ((Text.Length != 0 && Text[Index - 1] == '\\') || Text[Index + 1] != '(')
                        break;
                    ParseTranslationKey(Index, ref Index, ref Text);
                    break;

                case '[':
                    if (Text.Length != 0 && Text[Index - 1] == '\\')
                        break;
                    ParseQueryConstant(Index, ref Index, ref Text);
                    break;

                default:
                    continue;
            }

            if (Index > Text.Length - 2)
                return Text;
        }

    }
    #endregion
}