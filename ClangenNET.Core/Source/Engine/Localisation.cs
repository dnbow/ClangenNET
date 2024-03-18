using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using VYaml.Annotations;
using VYaml.Serialization;

namespace ClangenNET;

[YamlObject]
public sealed partial class LanguageMetadata
{
    /// <summary>
    /// Directory of all translation files. Will include subdirectories.
    /// </summary>
    [YamlMember("Directory")]
    public string Directory = null;

    /// <summary>
    /// A list of possible character sets,
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
    internal LanguageMetadata() { }

    /// <summary>
    /// Set all null fields of <paramref name="This"/> to any non-null fields of <paramref name="Other"/>
    /// </summary>
    public static LanguageMetadata operator |(LanguageMetadata This, LanguageMetadata Other) => new()
    {
        Directory = This.Directory ?? Other.Directory,
        Charset = This.Charset ?? Other.Charset,
        LanguageName = This.LanguageName ?? Other.LanguageName,
        Id = This.Id ?? Other.Id
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
    private static readonly ConcurrentDictionary<string, string> TranslationTable = new();

    /// <summary>
    /// Thread-safe dictionary, holding purely the metadata of each language.
    /// </summary>
    private static readonly ConcurrentDictionary<string, LanguageMetadata> LanguageTable = new();

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
                LanguageTable[Key] = Overwrite ? (New | Existing) : (Existing | New);
                continue;
            }

            New.Id = Key;
            New.Directory = $"Common\\Localisation\\{New.Directory}";

            LanguageTable[Key] = New;
        }
    }

    /// <summary>
    /// Set the current game language to a given key. Returns 0 on success, negative number on failure.
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

        Utility.RunTasks(Tasks).GetAwaiter().GetResult();
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
    /// Get a translated text by its key. Returns <see langword="null"/> if no translated text was found.
    /// </summary>
    public static string Get(string Key)
    {
        return TranslationTable.TryGetValue(Key, out string Value) ? Value : null;
    }
}

/// <summary>
/// Represents a string that depends on a set language.
/// </summary>
public readonly struct TranslationKey(string Key)
{
    /// <summary>
    /// The actual translation key
    /// </summary>
    public readonly string Key = Key;

    /// <summary>
    /// Get the translated string. 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return L10N.Get(Key);
    }

    public static implicit operator TranslationKey(string Key) => new(Key);
}