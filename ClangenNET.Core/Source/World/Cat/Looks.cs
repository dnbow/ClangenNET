using ClangenNET.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using YAXLib.Attributes;
using YAXLib.Enums;
using YAXLib.Exceptions;
using static ClangenNET.Cat;
using static ClangenNET.Content;
using static ClangenNET.MathEx;

namespace ClangenNET;

# region Defs
internal class TintConfigDef
{
    internal class GroupDef
    {
        [YAXAttributeForClass]
        internal required string Id { get; set; }

        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "li")]
        internal required List<string> AllowedColors { get; set; }

        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "li")]
        internal required List<string> PossibleTints { get; set; }
    }

    internal class TintDef
    {
        [YAXDictionary(EachPairName = "li", KeyName = "Id", ValueName = "Value", SerializeKeyAs = YAXNodeTypes.Attribute, SerializeValueAs = YAXNodeTypes.Attribute)]
        internal required Dictionary<string, string> Colors { get; set; }

        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Group")]
        internal required List<GroupDef> Groups { get; set; }
    }

    internal TintDef? Tint { get; set; }
    internal TintDef? WhiteTint { get; set; }
}

public class PeltPoseDefs : Def
{
    public class PoseItem
    {
        [YAXAttributeForClass]
        public required string Id { get; init; }

        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string? Fallback { get; init; }
    }

    public class PoseDef
    {
        [YAXAttributeForClass]
        public required string Id { get; set; }

        [YAXAttributeForClass]
        public required uint Rows { get; set; } = 0;

        [YAXAttributeForClass]
        public required uint Columns { get; set; } = 0;

        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "li")]
        public required List<PoseItem> Poses { get; set; }
    }

    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "PoseDef")]
    public required List<PoseDef> Poses { get; init; }
}

public class PeltDefs
{
    public class RegionItem
    {
        [YAXAttributeForClass]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string? Color { get; init; }
    }


    public class Regions
    {
        [YAXAttributeForClass]
        public required int Width { get; init; }

        [YAXAttributeForClass]
        public required int Height { get; init; }

        [YAXAttributeForClass]
        public required string PoseSet { get; init; }

        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "li")]
        public required List<RegionItem> RegionItems { get; init; }
    }

    public class PeltDef
    {
        [YAXAttributeForClass]
        public required string Id { get; init; }

        public required string Texture { get; init; }

        public required Regions Regions { get; init; }

        public string? Description { get; init; }
    }

    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "PeltDef")]
    public required List<PeltDef> Defs { get; init; }
}
#endregion

public static partial class Content
{
    internal static void LoadTints(IEnumerable<string> Files) // OPTIMISE if possible, it looks quite the mess right now
    {
        Dictionary<string, Color> Colors = [];
        Dictionary<string, Color> ColorsWhite = [];
        Dictionary<string, List<string>> ColorGroups = [];
        Dictionary<string, List<string>> ColorGroupsWhite = [];
        Dictionary<string, List<string>> PossibleTints = [];
        Dictionary<string, List<string>> PossibleTintsWhite = [];

        foreach (string FilePath in Files)
        {
            TintConfigDef? TintDef;
            using (StreamReader StreamReader = new(FilePath, System.Text.Encoding.Default, true, Config.BufferSize_Def))
                TintDef = DeserializeDef<TintConfigDef>(StreamReader);

            if (TintDef is null)
                continue;

            if (TintDef.Tint is not null)
            {
                foreach (var (K, V) in TintDef.Tint.Colors ?? [])
                    Colors[K] = Utility.ColorFromString(V);

                foreach (var Group in TintDef.Tint.Groups ?? [])
                {
                    foreach (var Color in Group.AllowedColors ?? [])
                    {
                        if (!ColorGroups.TryGetValue(Color, out var ExistingGroups))
                            ColorGroups[Color] = ExistingGroups = [];
                        ExistingGroups.Add(Group.Id);
                    }

                    if (PossibleTints.TryGetValue(Group.Id, out var Existing))
                        Existing.AddRange(Group.PossibleTints);
                    PossibleTints[Group.Id] = Existing ?? Group.PossibleTints;
                }
            }

            if (TintDef.WhiteTint is not null)
            {
                foreach (var (K, V) in TintDef.WhiteTint.Colors ?? [])
                    ColorsWhite[K] = Utility.ColorFromString(V);

                foreach (var Group in TintDef.WhiteTint.Groups ?? [])
                {
                    foreach (var Color in Group.AllowedColors ?? [])
                    {
                        if (!ColorGroupsWhite.TryGetValue(Color, out var ExistingGroups))
                            ColorGroupsWhite[Color] = ExistingGroups = [];
                        ExistingGroups.Add(Group.Id);
                    }

                    if (PossibleTintsWhite.TryGetValue(Group.Id, out var Existing))
                        Existing.AddRange(Group.PossibleTints);
                    PossibleTintsWhite[Group.Id] = Existing ?? Group.PossibleTints;
                }
            }
        }

        Looks.Tints.Colors = Colors;
        Looks.Tints.ColourGroups = ColorGroups.ToDictionary(I => I.Key, O => (IReadOnlyList<string>)O.Value);
        Looks.Tints.PossibleTints = PossibleTints.ToDictionary(I => I.Key, O => (IReadOnlyList<string>)O.Value);
        Looks.WhiteTints.Colors = ColorsWhite;
        Looks.WhiteTints.ColourGroups = ColorGroupsWhite.ToDictionary(I => I.Key, O => (IReadOnlyList<string>)O.Value);
        Looks.WhiteTints.PossibleTints = PossibleTintsWhite.ToDictionary(I => I.Key, O => (IReadOnlyList<string>)O.Value);
    }

    internal static void LoadPoses(IEnumerable<string> Files)
    {
        Dictionary<string, PoseSet> PoseSets = [];

        foreach (string FilePath in Files)
        {
            PeltPoseDefs? PoseDef;
            using (StreamReader StreamReader = new(FilePath, System.Text.Encoding.Default, true, Config.BufferSize_Def))
                PoseDef = DeserializeDef<PeltPoseDefs>(StreamReader);

            if (PoseDef is null) 
                continue;

            foreach (var Pose in PoseDef.Poses)
                PoseSets[Pose.Id] = new (Pose);
        }

        Looks.Poses = PoseSets;
    }

    internal static void LoadPelts(IEnumerable<string> Files)
    {
        if (Looks.Poses is null)
            throw new Exception();

        Dictionary<string, PeltType> PeltTypes = [];

        foreach (string FilePath in Files)
        {
            PeltDefs? PeltDefs;
            using (StreamReader StreamReader = new(FilePath, System.Text.Encoding.Default, true, Config.BufferSize_Def))
                PeltDefs = DeserializeDef<PeltDefs>(StreamReader);

            if (PeltDefs is null)
                continue;

            foreach (var Def in PeltDefs.Defs)
            {
                if (!File.Exists(ResolvePath($"Resources\\{Def.Texture}")))
                {
                    Console.WriteLine($"WARNING Path of PeltDef.Texture not found \"{Def.Texture}\"");
                    continue;
                }

                if (!Looks.Poses.TryGetValue(Def.Regions.PoseSet, out PoseSet? Poses))
                    throw new Exception();

                Rectangle TexSize = GetTexture(Def.Texture).Bounds;
                int Width = TexSize.Width / (Poses.Columns * Def.Regions.Width);
                int Height = TexSize.Height / (Poses.Rows * Def.Regions.Height);

                int PoseWidth = Width * Poses.Columns, PoseHeight = Height * Poses.Rows;

                Dictionary<string, Rectangle> PoseColorMap = [];
                for (int I = 0; I < Def.Regions.RegionItems.Count; I++)
                {
                    var Region = Def.Regions.RegionItems[I];
                    if (Region.Color is null)
                        continue;

                    PoseColorMap[Region.Color] = new (PoseWidth * (I % Def.Regions.Width), PoseHeight * (I / Def.Regions.Width), Width * Poses.Columns, Height * Poses.Rows);
                    
                }

                PeltTypes[Def.Id] = new PeltType()
                {
                    TexturePath = Def.Texture,
                    Regions = PoseColorMap.AsReadOnly(),
                    PoseSet = Poses
                };
            }
        }

        Looks.PeltTypes = PeltTypes;
    }
}

public class PoseSet
{
    private readonly string[] PoseIds;
    private readonly string?[] PoseDefaults;
    public readonly string Id;
    public readonly int Rows;
    public readonly int Columns;

    internal PoseSet(PeltPoseDefs.PoseDef Def)
    {
        Id = Def.Id;
        Rows = (int)Def.Rows;
        Columns = (int)Def.Columns;
        PoseIds = new string[Def.Poses.Count];
        PoseDefaults = new string?[Def.Poses.Count];

        int Index = 0;
        foreach (var Pose in Def.Poses)
            (PoseIds[Index], PoseDefaults[Index++]) = (Pose.Id, Pose.Fallback);
    }

    public string? Get(int Index)
    {
        return PoseIds[Index];
    }

    public string? GetDefault(string Id)
    {
        int Index = IndexOf(Id);
        return -1 < Index ? PoseDefaults[Index] : null;
    }

    public void ResolveSize(ref Rectangle Source, int Index)
    {
        Source.Width /= Columns;
        Source.Height /= Rows;
        Source.X += (Index % Columns);
        Source.Y += Index / Columns;
    }

    public int IndexOf(string Id) => Array.IndexOf(PoseIds, Id);
}

public class PeltType
{
    public required string TexturePath;
    public required PoseSet PoseSet;
    public required IReadOnlyDictionary<string, Rectangle> Regions;
}

public static class LooksGroups // TEMPORARY (maybe) TODO abstract string values to another value type
{
    public static readonly string[] EyeColours = [
        "Yellow", "Amber", "Hazel", "Palegreen", "Green", "Blue", "Darkblue", "Grey", "Cyan", "Emerald",
        "Paleblue", "Paleyellow", "Gold", "Heatherblue", "Copper", "Sage", "Cobalt", "Sunlitice", "Greenyellow", "Bronze",
        "Silver"
    ];

    public static readonly string[] YellowEyes = [
        "Yellow", "Amber", "Paleyellow", "Gold", "Copper", "Greenyellow", "Bronze", "Silver"
    ];

    public static readonly string[] BlueEyes = [
        "Blue", "Darkblue", "Cyan", "Paleblue", "Heatherblue", "Cobalt", "Sunlitice", "Grey"
    ];

    public static readonly string[] GreenEyes = [
        "Palegreen", "Green", "Emerald", "Sage", "Hazel"
    ];

    public static readonly string[] Vitiligo = [
        "Vitiligo", "Vitiligotwo", "Moon", "Phantom", "Karpati", "Powder", "Bleached", "Smokey"
    ];

    public static readonly string[] TortieBases = [
        "Single", "Tabby", "Bengal", "Marbled", "Ticked", "Smoke", "Rosette", "Speckled", "Mackerel", "Classic", "Sokoke", "Agouti", "Singlestripe", "Masked"
    ];

    public static readonly string[] TortiePatterns = [
        "One", "Two", "Three", "Four", "Redtail", "Delilah", "Minimalone", "Minimaltwo", "Minimalthree", "Minimalfour", "Half", "Oreo", "Swoop", "Mottled", "Sidemask", "Eyedot", "Bandana", "Pacman", "Streamstrike",
        "Oriole", "Chimera", "Daub", "Ember", "Blanket", "Robin", "Brindle", "Paige", "Rosetail", "Safi", "Smudged", "Dapplenight", "Streak", "Mask", "Chest", "Armtail", "Smoke", "Grumpyface", "Brie", "Beloved",
        "Body", "Shiloh", "Freckled", "Heartbeat"
    ];

    public static readonly string[] PeltColours = [
        "White", "Palegrey", "Silver", "Grey", "Darkgrey", "Ghost", "Black", "Cream", "Paleginger", "Golden", 
        "Ginger", "Darkginger", "Sienna", "Lightbrown", "Lilac", "Brown", "Goldbrown", "Darkbrown", "Chocolate"
    ];

    public static readonly string[] Tabbies = ["Tabby", "Ticked", "Mackerel", "Classic", "Sokoke", "Agouti"];
    public static readonly string[] Spotted = ["Speckled", "Rosette"];
    public static readonly string[] Plain = ["Single", "Singlestripe", "TwoColour", "Smoke"];
    public static readonly string[] Exotic = ["Bengal", "Marbled", "Masked"];
    public static readonly string[] Torties = ["Tortie", "Calico"];
    public static readonly IReadOnlyList<IReadOnlyList<string>> PeltCategories = [Tabbies, Spotted, Plain, Exotic, Torties];

    public static readonly string[] GingerColours = ["Cream", "Paleginger", "Golden", "Ginger", "Darkginger", "Sienna"];
    public static readonly string[] BlackColours = ["Grey", "Darkgrey", "Ghost", "Black"];
    public static readonly string[] WhiteColours = ["White", "Palegrey", "Silver"];
    public static readonly string[] BrownColours = ["Lightbrown", "Lilac", "Brown", "Goldbrown", "Darkbrown", "Chocolate"];
    public static readonly string[][] ColourCategories = [GingerColours, BlackColours, WhiteColours, BrownColours];

    public static readonly string[] PointMarkings = [
        "Colourpoint", "Ragdoll", "Sepiapoint", "Minkpoint", "Sealpoint"
    ];

    public static readonly IReadOnlyList<string> LittleWhite = [
        "Little", "LightTuxedo", "Buzzardfang", "Tip", "Blaze", "Bib", "Vee", "Paws", "Belly", "Tailtip", "Toes", "Brokenblaze", "Liltwo", "Scourge", "Toestail", "Ravenpaw",
        "Honey", "Luna", "Extra", "Mustache", "Reverseheart", "Sparkle", "Rightear", "Leftear", "Estrella", "ReverseEye", "Backspot", "Eyebags", "Locket", "Blazemask", "Tears"
    ];

    public static readonly IReadOnlyList<string> MiddleWhite = [
        "Tuxedo", "Fancy", "Unders", "Damien", "Skunk", "Mitaine", "Squeaks", "Star", "Wings", "Diva", "Savannah", "Fadespots", "Beard", "Dapplepaw", "Topcover", "Woodpecker",
        "Miss", "Bowtie", "Vest", "Fadebelly", "Digit", "Fctwo", "Fcone", "Mia", "Rosina", "Princess", "Dougie"
    ];

    public static readonly IReadOnlyList<string> HighWhite = [
        "Any", "Anytwo", "Broken", "Freckles", "Ringtail", "HalfFace", "Pantstwo", "Goatee", "Prince", "Farofa", "Mister", "Pants", "Reversepants", "Halfwhite", "Appaloosa", "Piebald",
        "Curved", "Glass", "Maskmantle", "Mao", "Painted", "Shibainu", "Owl", "Bub", "Sparrow", "Trixie", "Sammy", "Front", "Blossomstep", "Bullseye", "Finn", "Scar", "Buster",
        "Hawkblaze", "Cake"
    ];

    public static readonly IReadOnlyList<string> MostlyWhite = [
        "Van", "OneEar", "Lightsong", "Tail", "Heart", "Moorish", "Apron", "Capsaddle", "Chestspeck", "Blackstar", "Petal", "HeartTwo", "Pebbleshine", "Boots", "Cow", "Cowtwo", "Lovebug",
        "Shootingstar", "Eyespot", "Pebble", "Tailtwo", "Buddy", "Kropka"
    ];

    public static readonly IReadOnlyList<IReadOnlyList<string>> AllWhites = [
        LittleWhite, MiddleWhite, HighWhite, MostlyWhite, ["Fullwhite"]
    ];

    public static readonly IReadOnlyList<string> SkinSprites = [
        "Black", "Pink", "Darkbrown", "Brown", "Lightbrown", "Dark", "Darkgrey", "Grey", "Darksalmon",
        "Salmon", "Peach", "Darkmarbled", "Marbled", "Lightmarbled", "Darkblue", "Blue", "Lightblue", "Red"
    ];

    public static readonly IReadOnlyList<string> Scars = [
        "One", "Two", "Three", "Tailscar", "Snout", "Cheek", "Side", "Throat", "Tailbase", "Belly",
        "Legbite", "Neckbite", "Face", "Manleg", "Brightheart", "Mantail", "Bridge", "Rightblind", "Leftblind", "Bothblind",
        "Beakcheek", "Beaklower", "Catbite", "Ratbite", "Quillchunk", "Quillscratch"
    ];

    public static readonly IReadOnlyList<string> MissingScars = [
        "Leftear", "Rightear", "Notail", "Halftail", "Nopaw", "Noleftear", "Norightear", "Noear"
    ];

    public static readonly IReadOnlyList<string> SpecialScars = [
        "Snake", "Toetrap", "Burnpaws", "Burntail", "Burnbelly", "Burnrump", "Frostface", "FrostTail", "Frostmitt"
    ];

    public static readonly IReadOnlyList<string> PlantAccessories = [
        "Mapleleaf", "Holly", "Blueberries", "Forgetmenots", "Ryestalk", "Laurel", "Bluebells", "Nettle", "Poppy", "Lavender",
        "Herbs", "Petals", "Dryherbs", "Oakleaves", "Catmint", "Mapleseed", "Juniper"
    ];

    public static readonly IReadOnlyList<string> WildAccessories = [
        "Redfeathers", "Bluefeathers", "Jayfeathers", "Mothwings", "Cicadawings"
    ];

    public static readonly IReadOnlyList<string> Collars = [
        "Crimson", "Blue", "Yellow", "Cyan", "Red", "Lime", "Green", "Rainbow", "Black", "Spikes", "White", "Pink", "Purple", "Multi", "Indigo"
    ];
}


public enum PeltLength
{
    Short, Medium, Long
}


public static class SpriteType
{
    public const byte Kit0 = 0;
    public const byte Kit1 = 1;
    public const byte Kit2 = 2;
    public const byte Adolescent0 = 3;
    public const byte Adolescent1 = 4;
    public const byte Adolescent2 = 5;
    public const byte YoungShort = 6;
    public const byte AdultShort = 7;
    public const byte SeniorShort = 8;
    public const byte YoungLong = 9;
    public const byte AdultLong = 10;
    public const byte SeniorLong = 11;
    public const byte Senior0 = 12;
    public const byte Senior1 = 13;
    public const byte Senior2 = 14;
    public const byte ParalyzedShort = 15;
    public const byte ParalyzedLong = 16;
    public const byte ParalyzedYoung = 17;
    public const byte SickAdult = 18;
    public const byte SickYoung = 19;
    public const byte Newborn = 20;
}

public class Looks // TODO abstract all string values so theyre not strings (cuz it can kinda suck safety wise)
{
    public static class Tints
    {
        public static IReadOnlyDictionary<string, Color>? Colors { get; internal set; }
        public static IReadOnlyDictionary<string, IReadOnlyList<string>>? ColourGroups { get; internal set; }
        public static IReadOnlyDictionary<string, IReadOnlyList<string>>? PossibleTints { get; internal set; }
    }

    public static class WhiteTints
    {
        public static IReadOnlyDictionary<string, Color>? Colors { get; internal set; }
        public static IReadOnlyDictionary<string, IReadOnlyList<string>>? ColourGroups { get; internal set; }
        public static IReadOnlyDictionary<string, IReadOnlyList<string>>? PossibleTints { get; internal set; }
    }

    public static IReadOnlyDictionary<string, PoseSet>? Poses { get; internal set; }

    public static IReadOnlyDictionary<string, PeltType>? PeltTypes { get; internal set; }


    public PeltLength Length;

    public string Base;
    public string Colour;
    public string? Pattern;

    public string EyeColour;
    public string? EyeColour2;

    public string Skin;
    public string? Accessory;
    public List<string>? Scars;

    public string? TortieBase;
    public string? TortieColour;
    public string? TortiePattern;

    public string? Vitiligo;
    public string? WhitePatches;
    public string? Points;

    public string? Tint;
    public string? WhiteTint;

    public byte SpriteNewborn = SpriteType.Newborn;
    public byte SpriteKitten;
    public byte SpriteAdolescent;
    public byte SpriteYoungAdult;
    public byte SpriteYoungSick = SpriteType.SickYoung;
    public byte SpriteYoungParalyzed = SpriteType.ParalyzedLong;
    public byte SpriteAdult;
    public byte SpriteAdultSick = SpriteType.SickAdult;
    public byte SpriteAdultParalyzed;
    public byte SpriteSeniorAdult;
    public byte SpriteSenior;

    public byte Opacity = 255;
    public bool Reversed;

    public bool HasWhite => WhitePatches is null && Points is null;


    private static readonly int[] RPC_PeltWeights = [35, 20, 30, 15, 0];
    private static readonly int[] PCI_TabbiesWeights = [50, 10, 5, 7, 0];
    private static readonly int[] PCI_SpottedWeights = [10, 50, 5, 5, 0];
    private static readonly int[] PCI_PlainWeights = [5, 5, 50, 0, 0];
    private static readonly int[] PCI_ExoticWeights = [15, 15, 1, 45, 0];
    private static readonly int[] PCI_GingerWeights = [40, 0, 0, 10];
    private static readonly int[] PCI_BlackWeights = [0, 40, 2, 5];
    private static readonly int[] PCI_ShortWeights = [50, 10, 2];
    private static readonly int[] PCI_MediumWeights = [25, 50, 25];
    private static readonly int[] PCI_WhiteWeights = [0, 5, 40, 0];
    private static readonly int[] PCI_BrownWeights = [10, 5, 0, 35];
    private static readonly int[] PCI_LongWeights = [2, 10, 50];
    private static readonly string[] LC_TortiePatterns = ["Tabby", "Mackerel", "Classic", "Single", "Smoke", "Agouti", "Ticked"];
    private static readonly IReadOnlyCollection<string> LC_TortiePatternPrerequisites = ["Singlestripe", "Smoke", "Single"];

    /// <summary>
    /// Initialise this <see cref="Looks"/> object puesdo-randomly based on a given <see cref="Cat">
    /// </summary>
    public Looks([DisallowNull] Cat Source, RandomEx? GivenRandom = null)
    {
        RandomEx Random = GivenRandom ?? new(Source.Seed);
        bool HasWhite;

        // Init Pattern
        Base = Random.Choose(LooksGroups.PeltCategories, RPC_PeltWeights);
        Colour = Random.Choose<string>(LooksGroups.ColourCategories);
        Length = Random.Choose<PeltLength>();

        if (Random.Chance(Source.Sex ? ThisWorld.Config.CatGeneration.MaleTortieChance : ThisWorld.Config.CatGeneration.FemaleTortieChance))
        {
            TortieBase = (Base == "Single" || Base == "TwoColour") ? "Single" : Base.ToLower();
            Base = Random.Choose(LooksGroups.Torties);
        }

        HasWhite = (Base == "Single" && Colour == "White") && (Base != "Tortie") && Random.Chance(.4);

        // Init White Patches
        if (HasWhite)
        {
            if (Base != "Tortie" && Random.Chance(ThisWorld.Config.CatGeneration.RandomPointChance))
                Points = Random.Choose(LooksGroups.PointMarkings);

            WhitePatches = Random.Choose<string, int>(
                LooksGroups.AllWhites, Base == "Tortie" ? [2, 1, 0, 0, 0] : (Base == "Calico" ? [0, 0, 20, 15, 1] : [10, 10, 10, 10, 1])
            );

            if (Points is not null && (LooksGroups.HighWhite.Contains(WhitePatches) || LooksGroups.MostlyWhite.Contains(WhitePatches) || WhitePatches == "Fullwhite"))
                Points = null;
        }

        // Init Sprite
        SpriteKitten = Random.Choose([SpriteType.Kit0, SpriteType.Kit1, SpriteType.Kit2]);
        SpriteAdolescent = Random.Choose([SpriteType.Adolescent0, SpriteType.Adolescent1, SpriteType.Adolescent2]);
        SpriteSenior = Random.Choose([SpriteType.Senior0, SpriteType.Senior1, SpriteType.Senior2]);

        if (Length is PeltLength.Long)
        {
            SpriteSeniorAdult = SpriteYoungAdult = SpriteAdult = Random.Choose([SpriteType.YoungLong, SpriteType.AdultLong, SpriteType.SeniorLong]);
            SpriteAdultParalyzed = SpriteType.ParalyzedLong;
        }
        else
        {
            SpriteSeniorAdult = SpriteYoungAdult = SpriteAdult = Random.Choose([SpriteType.YoungShort, SpriteType.AdultShort, SpriteType.SeniorShort]);
            SpriteAdultParalyzed = SpriteType.ParalyzedShort;
        }

        Skin = Random.Choose(LooksGroups.SkinSprites);
        Reversed = Random.Choose();

        // Init Scars + Accessories
        if (Source.Age is not AgeStage.Newborn)
        {
            if (Random.Chance(Source.Age is AgeStage.Kitten or AgeStage.Adolescent ? 0.02 : (Source.Age is AgeStage.YoungAdult or AgeStage.Adult ? 0.05 : 0.0625)))
                Scars = [Random.Choose() ? Random.Choose(LooksGroups.Scars) : Random.Choose(LooksGroups.MissingScars)];
            
            if (Random.Chance(Source.Age is AgeStage.Kitten or AgeStage.Adolescent ? 0.005 : (Source.Age is AgeStage.YoungAdult or AgeStage.Adult ? 0.01 : 0.0125)))
                Accessory = Random.Choose() ? Random.Choose(LooksGroups.PlantAccessories) : Random.Choose(LooksGroups.WildAccessories);
        }

        // Init Eyes
        EyeColour = Random.Choose(LooksGroups.EyeColours);

        float Chance = ThisWorld.Config.CatGeneration.BaseHeterochromiaChance;

        if (WhitePatches is not null
            && (LooksGroups.HighWhite.Contains(WhitePatches) || LooksGroups.MostlyWhite.Contains(WhitePatches) || WhitePatches == "Fullwhite" || Colour == "White"))
            Chance *= WhitePatches == "Fullwhite" || Colour == "White" ? 5.1f : 4;

        if (Chance >= 1 || Random.Chance(Chance))
            EyeColour2 = Random.Choose<string>(
                LooksGroups.YellowEyes.Contains(EyeColour) ? [LooksGroups.BlueEyes, LooksGroups.GreenEyes] : (
                    LooksGroups.BlueEyes.Contains(EyeColour) ? [LooksGroups.YellowEyes, LooksGroups.GreenEyes] : [LooksGroups.YellowEyes, LooksGroups.BlueEyes]
                )
            );

        // Init Pattern
        if (LooksGroups.Torties.Contains(Base))
        {
            TortieColour = "Golden";
            TortieBase ??= Random.Choose(LooksGroups.TortieBases);
            Pattern ??= Random.Choose(LooksGroups.TortiePatterns);

            if (Colour is not null)
            {
                if (Random.Chance(ThisWorld.Config.CatGeneration.WildcardTortieChance))
                {
                    TortiePattern = Random.Choose(LooksGroups.TortieBases);
                    TortieColour = Random.ChooseExcept(LooksGroups.PeltColours, Colour);
                }
                else
                {
                    TortiePattern = LC_TortiePatternPrerequisites.Contains(TortieBase) 
                        ? Random.Choose(LC_TortiePatterns) : (Random.Chance(.97) ? TortieBase : "single");

                    if (Colour == "White")
                        Colour = Random.ChooseExcept(LooksGroups.WhiteColours, Colour);

                    if (LooksGroups.BlackColours.Contains(Colour) || LooksGroups.WhiteColours.Contains(Colour))
                        TortieColour = Random.Choose((Random.Chance(2 / 3) ? LooksGroups.GingerColours : LooksGroups.BrownColours));
                    else if (LooksGroups.GingerColours.Contains(Colour))
                        TortieColour = Random.Choose(Random.Chance(2 / 3) ? LooksGroups.BlackColours : LooksGroups.BrownColours);
                    else if (LooksGroups.BrownColours.Contains(Colour))
                        TortieColour = Random.ChooseExcept(
                            Random.Choose<string[], int>([LooksGroups.BrownColours, LooksGroups.BlackColours, LooksGroups.GingerColours], [1, 1, 2]), Colour
                        );
                }
            }
        }

        // Init Tint
        if (Tints.PossibleTints is not null && Colour is not null)
        {
            IReadOnlyList<string>? ColorArray = null;
            Tints.PossibleTints.TryGetValue("Basic", out IReadOnlyList<string>? BasicArray);
            Tints.ColourGroups?.TryGetValue(Colour, out ColorArray);

            if ((BasicArray = (Random.Choose() ? BasicArray : ColorArray) ?? BasicArray ?? ColorArray) is not null)
                Tint = Random.Choose(BasicArray);
        }        
        if (WhiteTints.PossibleTints is not null && Colour is not null && (WhitePatches is not null || Points is not null))
        {
            IReadOnlyList<string>? ColorArray = null;
            WhiteTints.PossibleTints.TryGetValue("Basic", out IReadOnlyList<string>? BasicArray);
            WhiteTints.ColourGroups?.TryGetValue(Colour, out ColorArray);

            if ((BasicArray = (Random.Choose() ? BasicArray : ColorArray) ?? BasicArray ?? ColorArray) is not null)
                WhiteTint = Random.Choose(BasicArray);
        }

        if (Colour is null) // TEMPORARY 
            throw new Exception();

        // Cleanup
        Fix();
    }

    /// <summary>
    /// Initialise this <see cref="Looks"/> object puesdo-randomly based on a given <see cref="Cat"> and their parents.
    /// </summary>
    public Looks(Cat Source, Cat FirstParent, Cat SecondParent, RandomEx? GivenRandom = null)
    {
        throw new NotImplementedException();
    }

    public void Fix()
    {
        if (Scars?.Contains("Notail") ?? false)
            Scars.Remove("Halftail");
    }

    public byte GetRegionIndexByAge(AgeStage Age) => Age switch
    {
        AgeStage.Newborn => SpriteNewborn,
        AgeStage.Kitten => SpriteKitten,
        AgeStage.Adolescent => SpriteAdolescent,
        AgeStage.YoungAdult => SpriteYoungAdult,
        AgeStage.Adult => SpriteAdult,
        AgeStage.SeniorAdult => SpriteSeniorAdult,
        AgeStage.Senior => SpriteSenior,
        _ => 0
    };
}



public partial class Cat
{
    public void Draw(SpriteBatchEx Batch, Rectangle Destination)
    {
        if (Looks.Base == "TwoColour")
            return;

        var PeltData = Looks.PeltTypes[Looks.Base];
        var PoseData = PeltData.PoseSet;

        Rectangle Source = PeltData.Regions[Looks.Colour];
        PoseData.ResolveSize(ref Source, Looks.GetRegionIndexByAge(Age));

        DrawTexture(PeltData.TexturePath, Destination, Source, Color.White);
    }
}