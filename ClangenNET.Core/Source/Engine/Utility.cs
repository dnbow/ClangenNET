using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using VYaml.Annotations;
using VYaml.Serialization;

using static ClangenNET.Math;

namespace ClangenNET;



public class Logger
{

}


[YamlObject]
public sealed partial class LanguageMetadata
{
    /// <summary>
    /// Directory of all translation files. Will include subdirectories.
    /// </summary>
    [YamlMember("Directory")]
    public string Directory = null;

    /// <summary>
    /// A charset that can be retrieved by <see cref="Utility.Unicode.GetCharSet(string)"/>
    /// </summary>
    [YamlMember("Charset")]
    public string[] Charset = null;

    /// <summary>
    /// The name of other languages in this language
    /// </summary>
    [YamlMember("LanguageName")]
    public Dictionary<string, string> LanguageName = null;

    /// <summary>
    /// The id of this language. Any translation files with this header will be considered part of this language
    /// </summary>
    [YamlIgnore]
    public string Id;

    // Here because we dont want people to create this object at runtime
    internal LanguageMetadata() {}

    /// <summary>
    /// Set all null fields of <paramref name="This"/> to any non-null fields of <paramref name="Other"/>
    /// </summary>
    public static LanguageMetadata operator |(LanguageMetadata This, LanguageMetadata Other) => new ()
    {
        Directory = This.Directory ?? Other.Directory,
        Charset = This.Charset ?? Other.Charset,
        LanguageName = This.LanguageName ?? Other.LanguageName,
        Id = This.Id ?? Other.Id
    };

    /// <summary>
    /// Set all fields of <paramref name="This"/> to all non-null fields of <paramref name="Other"/>
    /// </summary>
    public static LanguageMetadata operator &(LanguageMetadata This, LanguageMetadata Other) => new()
    {
        Directory = Other.Directory ?? This.Directory,
        Charset = Other.Charset ?? This.Charset,
        LanguageName = Other.LanguageName ?? This.LanguageName,
        Id = Other.Id ?? This.Id
    };
}


/// <summary>
/// Helper class containing many bodys for niche cases or miscellaneous purpose
/// </summary>
public static class Utility
{
    public static async Task<byte[]> ComputeHash(HashAlgorithm Algorithm, DirectoryInfo Directory, string SearchPattern = "*", SearchOption SearchOption = SearchOption.TopDirectoryOnly)
    {
        int RelativeIndex = Directory.FullName.Length + 1;
        FileInfo[] Files = Directory.EnumerateFiles(SearchPattern, SearchOption).OrderBy((i) => i.FullName[RelativeIndex..]).ToArray();

        using (CryptoStream CryptoStream = new (Stream.Null, Algorithm, CryptoStreamMode.Write))
        {
            for (int i = 0; i < Files.Length; i++)
            {
                FileInfo File = Files[i];
                CryptoStream.Write(Encoding.Unicode.GetBytes(File.FullName[RelativeIndex..]));
                using FileStream FileStream = File.OpenRead();
                await FileStream.CopyToAsync(CryptoStream);
            }

            CryptoStream.FlushFinalBlock();
        }

        return Algorithm.Hash;
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
    /// A global <see cref="RandomEx"> object to avoid unnessecary creation.
    /// </summary>
    public static readonly RandomEx Rand = new ();

    /// <summary>
    /// Helper class representing this games Version
    /// </summary>
    public static class Version
    {
        public const byte Major = 0;
        public const byte Minor = 0;
        public const ushort Revision = 0;

        /// <summary>
        /// An unsigned integer summarising the version; its useful in save data and error logging
        /// </summary>
        public const uint Build = (Major << 24) | (Minor << 16) | Revision;

        /// <summary>
        /// A nice little label that represents the current Major/Minor update!
        /// </summary>
        public const string Label = "Primordial Soup";

        /// <summary>
        /// Check that the given version information points to a real version.
        /// </summary>
        public static bool IsValid(byte Major, byte Minor, ushort Revision) => Major switch
        { // Looks UGLY and does have manual fields, BUT constant speed thanks to compile-time bafoonery
            0 => Minor switch {
                0 => Revision switch { <= 0 => true, _ => false, },
                _ => false, },

            _ => false,
        };

        /// <summary>
        /// Represents a dynamic <see cref="Version"> i.e a Version created from a game save.
        /// </summary>
        public class Snapshot
        {
            public readonly byte Major;
            public readonly byte Minor;
            public readonly ushort Revision;

            /// <summary>
            /// An unsigned integer summarising the version; its useful in save data and error logging
            /// </summary>
            public uint Build => (uint)((Major << 24) | (Minor << 16) | Revision);

            /// <summary>
            /// Create a Snapshot from 3 version numbers
            /// </summary>
            public Snapshot(byte Major, byte Minor, ushort Revision)
            {
                this.Major = Major;
                this.Minor = Minor;
                this.Revision = Revision;
            }

            /// <summary>
            /// Create a Snapshot from saved bytes
            /// </summary>
            /// <param name="Array"></param>
            public Snapshot(byte[] Array)
            {
                if (Array.Length != 4)
                    throw new ArgumentException("Array was malformed, must be length of 4 bytes", nameof(Array));

                Major = Array[3];
                Minor = Array[2];
                Revision = (ushort)((Array[2] << 8) | Array[3]);
            }

            /// <summary>
            /// Create a Snapshot from a string representation i.e 0.1.12
            /// </summary>
            public Snapshot(string Version)
            {
                string[] Sections = Version.Split('.');
                if (Sections.Length != 3)
                    throw new ArgumentException("Version was malformed, must be in format of MAJOR.MINOR.REVISION", nameof(Version));

                Major = byte.Parse(Sections[0]);
                Minor = byte.Parse(Sections[1]);
                Revision = ushort.Parse(Sections[2]);
            }

            /// <summary>
            /// Create a Snapshot from an unsigned build number i.e 65548 (0.1.12)
            /// </summary>
            public Snapshot(uint Version)
            {
                Major = (byte)((Version & 0xFF000000) >> 24);
                Minor = (byte)((Version & 0x00FF0000) >> 16);
                Revision = (ushort)(Version & 0xFFFF);
            }

            /// <summary>
            /// Check that this Snapshot information is valid.
            /// </summary>
            public bool IsValid()
            {
                return Version.IsValid(Major, Minor, Revision);
            }
        }        
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

        public readonly struct UnicodeRange
        {
            public readonly ushort Lower;
            public readonly ushort Upper;

            public UnicodeRange(ushort Lower, ushort Upper)
            {
                this.Lower = Lower;
                this.Upper = Upper;
            }
        }

        /// <summary>
        /// Get a set of characters with a given name, returns <see langword="null"/> 
        /// if <paramref name="Name"/> is not a sets name. This should only be used for
        /// dynamically retrieving a set i.e when interpretting data from a file
        /// </summary>
        public static string GetCharSet(string Name) => Name switch
        { // Looks UGLY and does have manual fields, BUT constant speed thanks to compile-time bafoonery
            nameof(Latin) => Latin,
            nameof(Latin1) => Latin1,
            nameof(LatinExpandedA) => LatinExpandedA,
            nameof(LatinExpandedB) => LatinExpandedB,
            _ => null,
        };

        /// <summary>
        /// Get the unicode range of a set of characters. Useful for checking if a character is
        /// within a charset
        /// </summary>
        public static UnicodeRange GetCharSetRange(string Name) => Name switch
        { // Looks UGLY and does have manual fields, BUT constant speed thanks to compile-time bafoonery
            nameof(Latin) => new (0x0020, 0x007F),
            nameof(Latin1) => new (0x00A0, 0x00FF),
            nameof(LatinExpandedA) => new (0x0100, 0x017F),
            nameof(LatinExpandedB) => new (0x0180, 0x024F),
            _ => new (0, 0),
        };
    }

    /// <summary>
    /// Helper class to facilitate multiple languages.
    /// </summary>
    public static class L10N
    {
        /// <summary>
        /// Thread-safe dictionary, mapping translation keys to text values of the current language.
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> TranslationTable = new ();

        /// <summary>
        /// Thread-safe dictionary, holding purely the metadata of each language.
        /// </summary>
        private static readonly ConcurrentDictionary<string, LanguageMetadata> LanguageTable = new ();

        /// <summary>
        /// The key of the current-selected language.
        /// </summary>
        private static LanguageMetadata Current;

        /// <summary>
        /// Load a languages file. Specify with <paramref name="Overwrite"/> whether or not data 
        /// should be overwritten if it already exists, defaults to <see langword="true"/>
        /// </summary>
        /// <param name="Path">The path of the file</param>
        /// <param name="Overwrite"><see langword="true"> if new fields should overwrite existing ones</param>
        public static void LoadLanguages(string Path, bool Overwrite = true)
        {
            ReadOnlyMemory<byte> ReadonlyData = File.ReadAllBytes(Path);

            var Tables = YamlSerializer.Deserialize<Dictionary<string, LanguageMetadata>>(ReadonlyData);

            string[] Keys = new string[Tables.Count];
            Tables.Keys.CopyTo(Keys, 0);

            string Key;
            LanguageMetadata New;
            for (int i = 0; i < Keys.Length; i++)
            {
                New = Tables[Key = Keys[i]];

                if (LanguageTable.TryGetValue(Key, out LanguageMetadata Existing))
                {
                    if (Overwrite)
                        LanguageTable[Key] = Existing & New;
                    else
                        LanguageTable[Key] = Existing | New;

                    continue;
                }

                New.Id = Key;
                New.Directory = $"Common\\Localisation\\{New.Directory}";

                LanguageTable[Key] = New;
            }
        }

        /// <summary>
        /// Set the current game language to a given key. Returns 0 on success, negative number on failure
        /// </summary>
        public static int SetLanguage(string Key)
        {
            if (!LanguageTable.TryGetValue(Key, out LanguageMetadata Metadata))
                return -1; // Metadata doesnt exist
            else if (Metadata.Directory is null)
                return -2; // Metadata path wasnt given
            else if (!Directory.Exists(Metadata.Directory))
                return -3; // Metadata path doesnt exist

            Current = Metadata;

            string[] Paths = Directory.GetFiles(Metadata.Directory, "*.yml", SearchOption.AllDirectories);
            Task[] Tasks = new Task[Paths.Length];

            for (int i = 0; i < Paths.Length; i++)
                Tasks[i] = LoadFile(Paths[i], Key);

            RunTasks(Tasks).GetAwaiter().GetResult();
            return 0;
        }

        private static async Task LoadFile(string Path, string LanguageKey)
        {
            Dictionary<string, Dictionary<string, string>> Tables;
            using (FileStream Stream = File.OpenRead(Path))
            {
                Tables = await YamlSerializer.DeserializeAsync<Dictionary<string, Dictionary<string, string>>>(Stream);
            }

            if (!Tables.TryGetValue(LanguageKey, out Dictionary<string, string> Table))
                return;

            string[] Keys = new string[Table.Count];
            Table.Keys.CopyTo(Keys, 0);

            string Key;
            for (int i = 0; i < Table.Count; i++)
                TranslationTable[Key = Keys[i]] = Table[Key];
        }

        /// <summary>
        /// Get a translated text by its key. Returns key if no translated text was found.
        /// </summary>
        public static string GetText(string Key)
        {
            return TranslationTable.TryGetValue(Key, out string Value) ? Value : Key;
        }
    }

    /// <summary>
    /// Represents a string that depends on a set language.
    /// </summary>
    public readonly struct TranslationKey
    {
        /// <summary>
        /// The actual translation key
        /// </summary>
        public readonly string Key;

        public TranslationKey(string Key)
        {
            this.Key = Key;
        }

        /// <summary>
        /// Get the translated string. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return L10N.GetText(Key);
        }

        public static implicit operator TranslationKey(string Key) => new (Key);
    }
}
