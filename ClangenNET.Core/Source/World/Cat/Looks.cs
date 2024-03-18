using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static ClangenNET.MathEx;
using static ClangenNET.Cat;

namespace ClangenNET;

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

    public static readonly PeltColour[] PeltColours = Enum.GetValues<PeltColour>();

    public static readonly string[] Tabbies = ["Tabby", "Ticked", "Mackerel", "Classic", "Sokoke", "Agouti"];
    public static readonly string[] Spotted = ["Speckled", "Rosette"];
    public static readonly string[] Plain = ["SingleColour", "Singlestripe", "TwoColour", "Smoke"];
    public static readonly string[] Exotic = ["Bengal", "Marbled", "Masked"];
    public static readonly string[] Torties = ["Tortie", "Calico"];
    public static readonly string[][] PeltCategories = [Tabbies, Spotted, Plain, Exotic, Torties];

    public static readonly PeltColour[] GingerColours = [PeltColour.Cream, PeltColour.Paleginger, PeltColour.Golden, PeltColour.Ginger, PeltColour.Darkginger, PeltColour.Sienna];
    public static readonly PeltColour[] BlackColours = [PeltColour.Grey, PeltColour.Darkgrey, PeltColour.Ghost, PeltColour.Black];
    public static readonly PeltColour[] WhiteColours = [PeltColour.White, PeltColour.Palegrey, PeltColour.Silver];
    public static readonly PeltColour[] BrownColours = [PeltColour.Lightbrown, PeltColour.Lilac, PeltColour.Brown, PeltColour.Goldenbrown, PeltColour.Darkbrown, PeltColour.Chocolate];
    public static readonly PeltColour[][] ColourCategories = [GingerColours, BlackColours, WhiteColours, BrownColours];

    public static readonly string[] PointMarkings = [
            "Colourpoint", "Ragdoll", "Sepiapoint", "Minkpoint", "Sealpoint"
        ];

    public static readonly string[] LittleWhite = [
            "Little", "LightTuxedo", "Buzzardfang", "Tip", "Blaze", "Bib", "Vee", "Paws", "Belly", "Tailtip", "Toes", "Brokenblaze", "Liltwo", "Scourge", "Toestail", "Ravenpaw",
            "Honey", "Luna", "Extra", "Mustache", "Reverseheart", "Sparkle", "Rightear", "Leftear", "Estrella", "ReverseEye", "Backspot", "Eyebags", "Locket", "Blazemask", "Tears"
        ];
    public static readonly string[] MiddleWhite = [
            "Tuxedo", "Fancy", "Unders", "Damien", "Skunk", "Mitaine", "Squeaks", "Star", "Wings", "Diva", "Savannah", "Fadespots", "Beard", "Dapplepaw", "Topcover", "Woodpecker",
            "Miss", "Bowtie", "Vest", "Fadebelly", "Digit", "Fctwo", "Fcone", "Mia", "Rosina", "Princess", "Dougie"
        ];
    public static readonly string[] HighWhite = [
            "Any", "Anytwo", "Broken", "Freckles", "Ringtail", "HalfFace", "Pantstwo", "Goatee", "Prince", "Farofa", "Mister", "Pants", "Reversepants", "Halfwhite", "Appaloosa", "Piebald",
            "Curved", "Glass", "Maskmantle", "Mao", "Painted", "Shibainu", "Owl", "Bub", "Sparrow", "Trixie", "Sammy", "Front", "Blossomstep", "Bullseye", "Finn", "Scar", "Buster",
            "Hawkblaze", "Cake"
        ];
    public static readonly string[] MostlyWhite = [
            "Van", "OneEar", "Lightsong", "Tail", "Heart", "Moorish", "Apron", "Capsaddle", "Chestspeck", "Blackstar", "Petal", "HeartTwo", "Pebbleshine", "Boots", "Cow", "Cowtwo", "Lovebug",
            "Shootingstar", "Eyespot", "Pebble", "Tailtwo", "Buddy", "Kropka"
        ];

    public static readonly string[] SkinSprites = [
            "Black", "Pink", "Darkbrown", "Brown", "Lightbrown", "Dark", "Darkgrey", "Grey", "Darksalmon",
            "Salmon", "Peach", "Darkmarbled", "Marbled", "Lightmarbled", "Darkblue", "Blue", "Lightblue", "Red"
        ];

    public static readonly string[] Scars = [
            "One", "Two", "Three", "Tailscar", "Snout", "Cheek", "Side", "Throat", "Tailbase", "Belly",
            "Legbite", "Neckbite", "Face", "Manleg", "Brightheart", "Mantail", "Bridge", "Rightblind", "Leftblind", "Bothblind",
            "Beakcheek", "Beaklower", "Catbite", "Ratbite", "Quillchunk", "Quillscratch"
        ];
    public static readonly string[] MissingScars = [
            "Leftear", "Rightear", "Notail", "Halftail", "Nopaw", "Noleftear", "Norightear", "Noear"
        ];
    public static readonly string[] SpecialScars = [
            "Snake", "Toetrap", "Burnpaws", "Burntail", "Burnbelly", "Burnrump", "Frostface", "FrostTail", "Frostmitt"
        ];

    public static readonly string[] PlantAccessories = [
            "Mapleleaf", "Holly", "Blueberries", "Forgetmenots", "Ryestalk", "Laurel", "Bluebells", "Nettle", "Poppy", "Lavender",
            "Herbs", "Petals", "Dryherbs", "Oakleaves", "Catmint", "Mapleseed", "Juniper"
        ];
    public static readonly string[] WildAccessories = [
            "Redfeathers", "Bluefeathers", "Jayfeathers", "Mothwings", "Cicadawings"
        ];


    public static readonly string[] Collars = [
            "Crimson", "Blue", "Yellow", "Cyan", "Red", "Lime", "Green", "Rainbow", "Black", "Spikes", "White", "Pink", "Purple", "Multi", "Indigo"
        ];
}

public static class Tints
{
    public static readonly Dictionary<PeltColour, string> ColourGroups = new()
    {
        { PeltColour.White,       "White" },
        { PeltColour.Palegrey,    "Cool" },
        { PeltColour.Silver,      "Cool" },
        { PeltColour.Grey,        "Monochrome" },
        { PeltColour.Darkgrey,    "Monochrome" },
        { PeltColour.Ghost,       "Monochrome" },
        { PeltColour.Black,       "Monochrome" },
        { PeltColour.Cream,       "Warm" },
        { PeltColour.Paleginger,  "Warm" },
        { PeltColour.Golden,      "Warm" },
        { PeltColour.Ginger,      "Warm" },
        { PeltColour.Darkginger,  "Warm" },
        { PeltColour.Sienna,      "Brown" },
        { PeltColour.Lightbrown,  "Brown" },
        { PeltColour.Lilac,       "Brown" },
        { PeltColour.Brown,       "Brown" },
        { PeltColour.Goldenbrown, "Brown" },
        { PeltColour.Darkbrown,   "Brown" },
        { PeltColour.Chocolate,   "Brown" },
    };

    public static readonly Dictionary<string, string[]> PossibleTints = new()
    {
        { "White",      [ "Yellow" ] },
        { "Cool",       [ "Blue", "Purple", "Black" ] },
        { "Monochrome", [ "Blue", "Black" ] },
        { "Warm",       [ "Yellow", "Purple" ] },
        { "Brown",      [ "Yellow", "Purple", "Black" ] },
        { "Basic",      [ "Pink", "Grey", "Red", "Orange" ] }
    };

    public static readonly Dictionary<string, Color> Colors = new()
    {
        { "Orange", new (255, 247, 235) },
        { "Purple", new (235, 225, 244) },
        { "Yellow", new (250, 248, 225) },
        { "Black",  new (195, 195, 195) },
        { "Blue",   new (218, 237, 245) },
        { "Grey",   new (225, 225, 225) },
        { "Pink",   new (253, 237, 237) },
        { "Red",    new (248, 226, 228) },
    };
}

public static class PatchTints
{
    public static readonly Dictionary<PeltColour, string> ColourGroups = new()
    {
        { PeltColour.Palegrey,    "Grey" },
        { PeltColour.Silver,      "Grey" },
        { PeltColour.Grey,        "Grey" },
        { PeltColour.Darkgrey,    "Black" },
        { PeltColour.Ghost,       "Black" },
        { PeltColour.Black,       "Black" },
        { PeltColour.Cream,       "Ginger" },
        { PeltColour.Paleginger,  "Ginger" },
        { PeltColour.Golden,      "Ginger" },
        { PeltColour.Ginger,      "Ginger" },
        { PeltColour.Darkginger,  "Ginger" },
        { PeltColour.Sienna,      "Ginger" },
        { PeltColour.Lightbrown,  "Brown" },
        { PeltColour.Lilac,       "Brown" },
        { PeltColour.Brown,       "Brown" },
        { PeltColour.Goldenbrown, "Brown" },
        { PeltColour.Darkbrown,   "Brown" },
        { PeltColour.Chocolate,   "Brown" },
    };

    public static readonly Dictionary<string, string[]> PossibleTints = new()
    {
        { "Grey",   [ "Grey" ] },
        { "Black",  [ "Grey", "Darkcream", "Cream" ] },
        { "Ginger", [ "Darkcream", "Cream", "Pink" ] },
        { "Brown",  [ "Darkcream", "Cream" ] },
        { "Basic",  [ "Offwhite" ] },
    };

    public static readonly Dictionary<string, Color> Colors = new()
    {
        { "Darkcream", new (236, 229, 208) },
        { "Offwhite",  new (238, 249, 252) },
        { "Cream",     new (247, 241, 225) },
        { "Grey",      new (208, 225, 229) },
        { "Pink",      new (254, 248, 249) },
    };
}


public enum PeltColour : byte // PROBABLY be replaced to struct, members would still persist through static readonly defines
{
    None,
    White,
    Palegrey,
    Silver,
    Grey,
    Darkgrey,
    Ghost,
    Black,
    Cream,
    Paleginger,
    Golden,
    Ginger,
    Darkginger,
    Sienna,
    Lightbrown,
    Lilac,
    Brown,
    Goldenbrown,
    Darkbrown,
    Chocolate
}


public enum PeltLength : byte
{
    Short, Medium, Long
}

public enum PeltType : byte
{
    SingleColour, TwoColour, Tortie, Calico
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

public class Looks // TODO abstract all string values so theyre not strings (cuz it can kinda suck)
{
    public string Name;
    public PeltColour Colour;
    public string Pattern;
    public PeltLength Length;
    public string TortieBase;
    public string TortiePattern;
    public PeltColour TortieColour;
    public string WhitePatches;
    public string EyeColour;
    public string EyeColour2;
    public string Vitiligo;
    public string Points;
    public string Accessory;
    public List<string> Scars;
    public string Tint;
    public string PatchTint;
    public string Skin;

    public byte SpriteNewborn;
    public byte SpriteKitten;
    public byte SpriteAdolescent;
    public byte SpriteYoungAdult;
    public byte SpriteYoungSick;
    public byte SpriteYoungParalyzed;
    public byte SpriteAdult;
    public byte SpriteAdultSick;
    public byte SpriteAdultParalyzed;
    public byte SpriteSeniorAdult;
    public byte SpriteSenior;

    public byte Opacity;

    public bool Paralyzed;
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

    private Looks() // Prevents looks being created without a given cat + set defaults
    {
        Scars = [];
        Opacity = 255;

        SpriteNewborn = SpriteType.Newborn;
        SpriteAdultSick = SpriteType.SickAdult;
        SpriteAdultSick = SpriteType.SickAdult;
        SpriteYoungParalyzed = SpriteType.ParalyzedLong;
    }

    /// <summary>
    /// Initialise this <see cref="Looks"/> object randomly based on a given <see cref="Cat">
    /// </summary>
    /// <param name="Source"></param>
    public Looks([DisallowNull] Cat Source) : this()
    {
        RandomEx RNG = new (Source.Seed);

        // init_pattern_color
        bool PeltHasWhite = GeneratePattern(RNG, Source);

        // init_white_patches
        const int vitiligo_chance = 8; // TEMPORARY CONFIG
        if (RNG.ChanceEx2(vitiligo_chance)) 
            Vitiligo = RNG.ChooseFrom(LooksGroups.Vitiligo);
        if (PeltHasWhite) 
            GeneratePatches(RNG);

        // init_sprites
        SpriteKitten = RNG.ChooseFrom([SpriteType.Kit0, SpriteType.Kit1, SpriteType.Kit2]);
        SpriteAdolescent = RNG.ChooseFrom([SpriteType.Adolescent0, SpriteType.Adolescent1, SpriteType.Adolescent2]);
        SpriteSenior = RNG.ChooseFrom([SpriteType.Senior0, SpriteType.Senior1, SpriteType.Senior2]);

        if (Length == PeltLength.Long)
        {
            SpriteYoungAdult = SpriteSeniorAdult = SpriteAdult = RNG.ChooseFrom([SpriteType.YoungLong, SpriteType.AdultLong, SpriteType.SeniorLong]);
            SpriteAdultParalyzed = SpriteType.ParalyzedLong;
        }
        else
        {
            SpriteYoungAdult = SpriteSeniorAdult = SpriteAdult = RNG.ChooseFrom([SpriteType.YoungShort, SpriteType.AdultShort, SpriteType.SeniorShort]);
            SpriteAdultParalyzed = SpriteType.ParalyzedShort;
        }

        Skin = RNG.ChooseFrom(LooksGroups.SkinSprites);
        Reversed = RNG.Choose();

        // init_scars + init_accessories
        if (Source.Age is not AgeStage.Newborn)
        {
            int ScarChance, AccessoryChance;
            switch (Source.Age)
            {
                case AgeStage.Kitten:
                case AgeStage.Adolescent:
                    ScarChance = 50; AccessoryChance = 180; break;
                case AgeStage.YoungAdult:
                case AgeStage.Adult:
                    ScarChance = 20; AccessoryChance = 100; break;
                default:
                    ScarChance = 15; AccessoryChance = 80; break;
            }

            if (RNG.InverseChance(ScarChance))
                Scars.Add(RNG.ChooseFrom(RNG.Choose() ? LooksGroups.Scars : LooksGroups.SpecialScars));
            if (RNG.InverseChance(AccessoryChance))
                Accessory = RNG.ChooseFrom(RNG.Choose() ? LooksGroups.PlantAccessories : LooksGroups.WildAccessories);
        }

        // init_eye
        EyeColour = RNG.ChooseFrom(LooksGroups.EyeColours);

        int base_heterochromia = 120; // TEMPORARY CONFIG

        if (Colour is PeltColour.White || Array.IndexOf(LooksGroups.HighWhite, WhitePatches) > -1 || Array.IndexOf(LooksGroups.MostlyWhite, WhitePatches) > -1 || WhitePatches == "Fullwhite")
            base_heterochromia -= 90;
        if (Colour is PeltColour.White || WhitePatches == "Fullwhite")
            base_heterochromia -= 10;

        if (RNG.InverseChance(base_heterochromia < 0 ? 1 : base_heterochromia))
        {
            if (Array.IndexOf(LooksGroups.YellowEyes, EyeColour) > -1)
                EyeColour2 = RNG.ChooseFrom(RNG.Choose() ? LooksGroups.BlueEyes : LooksGroups.GreenEyes);
            else if (Array.IndexOf(LooksGroups.BlueEyes, EyeColour) > -1)
                EyeColour2 = RNG.ChooseFrom(RNG.Choose() ? LooksGroups.YellowEyes : LooksGroups.GreenEyes);
            else if (Array.IndexOf(LooksGroups.GreenEyes, EyeColour) > -1)
                EyeColour2 = RNG.ChooseFrom(RNG.Choose() ? LooksGroups.BlueEyes : LooksGroups.YellowEyes);
        }

        // init_pattern
        if (Array.IndexOf(LooksGroups.Torties, Name) > -1)
        {
            TortieColour = PeltColour.Golden;
            TortieBase ??= RNG.ChooseFrom(LooksGroups.TortieBases);
            Pattern ??= RNG.ChooseFrom(LooksGroups.TortiePatterns);

            if (Colour is not PeltColour.None)
            {
                const int WildcardChance = 9; // TEMPORARY CONFIG
                if (WildcardChance == 0 || RNG.InverseChance(WildcardChance))
                {
                    TortiePattern = RNG.ChooseFrom(LooksGroups.TortieBases);

                    var PossibleColours = LooksGroups.PeltColours.ToList();
                    PossibleColours.Remove(Colour);

                    TortieColour = RNG.ChooseFrom(PossibleColours);
                }
                else
                {
                    if (TortieBase == "Singlestripe" || TortieBase == "Smoke" || TortieBase == "Single")
                        TortiePattern = RNG.ChooseFrom(LC_TortiePatterns);
                    else
                        TortiePattern = RNG.Chance(.97) ? TortieBase : "Single";

                    if (Colour is PeltColour.White)
                    {
                        var PossibleColours = LooksGroups.WhiteColours.ToList();
                        PossibleColours.Remove(PeltColour.White);
                        Colour = RNG.ChooseFrom(PossibleColours);
                    }

                    if (Array.IndexOf(LooksGroups.BlackColours, Colour) > -1 || Array.IndexOf(LooksGroups.WhiteColours, Colour) > -1)
                    {
                        TortieColour = RNG.ChooseFrom(RNG.Chance(1 / 3) ? LooksGroups.GingerColours : LooksGroups.BrownColours);
                    }
                    else if (Array.IndexOf(LooksGroups.GingerColours, Colour) > -1)
                    {
                        TortieColour = RNG.ChooseFrom(RNG.Chance(1 / 3) ? LooksGroups.BrownColours : LooksGroups.BrownColours);
                    }
                    else if (Array.IndexOf(LooksGroups.BrownColours, Colour) > -1)
                    {
                        var PossibleColours = LooksGroups.BrownColours.ToList();
                        PossibleColours.Remove(Colour);
                        TortieColour = RNG.ChooseFrom(
                            RNG.ChooseFrom([[.. PossibleColours], LooksGroups.BlackColours, LooksGroups.GingerColours], [1, 1, 2])
                        );
                    }
                }
            }
        }

        // init_tints
        string[] BaseTints = Tints.PossibleTints["Basic"], ColorTints = null;

        if (Tints.ColourGroups.TryGetValue(Colour, out string value)) Tints.PossibleTints.TryGetValue(value, out ColorTints);
        
        if (BaseTints is not null || ColorTints is not null)
            Tint = RNG.ChooseFrom((RNG.Choose() ? BaseTints : ColorTints) ?? BaseTints ?? ColorTints);

        if (WhitePatches is not null || Points is not null)
        {
            PatchTints.PossibleTints.TryGetValue("Basic", out BaseTints);
            ColorTints = null;
            if (Tints.ColourGroups.TryGetValue(Colour, out string Value))
                ColorTints = Tints.PossibleTints[Value];
            if (BaseTints is not null || ColorTints is not null)
                PatchTint = RNG.ChooseFrom((RNG.Choose() ? BaseTints : ColorTints) ?? BaseTints ?? ColorTints);
        }
    }

    /// <summary>
    /// Initialise this <see cref="Looks"/> object randomly based on a given <see cref="Cat">
    /// </summary>
    /// <param name="Source"></param>
    public Looks([DisallowNull] Cat Source, [DisallowNull] Cat FirstParent, [DisallowNull] Cat SecondParent) : this()
    {
        RandomEx RNG = new(Source.Seed);
        Looks FirstLooks = FirstParent.Looks, SecondLooks = SecondParent.Looks;

        // init_pattern_color
        bool PeltHasWhite = GeneratePattern(RNG, Source, FirstParent, SecondParent);

        // init_white_patches
        int vitiligo_chance = 8 + (FirstLooks.Vitiligo is null ? 0 : -1) + (SecondLooks.Vitiligo is null ? 0 : -1); // TEMPORARY CONFIG
        if (RNG.ChanceEx2(vitiligo_chance))
            Vitiligo = RNG.ChooseFrom(LooksGroups.Vitiligo);
        if (PeltHasWhite)
            GeneratePatches(RNG, FirstParent, SecondParent);

        // init_sprites
        SpriteKitten = RNG.ChooseFrom([SpriteType.Kit0, SpriteType.Kit1, SpriteType.Kit2]);
        SpriteAdolescent = RNG.ChooseFrom([SpriteType.Adolescent0, SpriteType.Adolescent1, SpriteType.Adolescent2]);
        SpriteSenior = RNG.ChooseFrom([SpriteType.Senior0, SpriteType.Senior1, SpriteType.Senior2]);

        if (Length == PeltLength.Long)
        {
            SpriteYoungAdult = SpriteSeniorAdult = SpriteAdult = RNG.ChooseFrom([SpriteType.YoungLong, SpriteType.AdultLong, SpriteType.SeniorLong]);
            SpriteAdultParalyzed = SpriteType.ParalyzedLong;
        }
        else
        {
            SpriteYoungAdult = SpriteSeniorAdult = SpriteAdult = RNG.ChooseFrom([SpriteType.YoungShort, SpriteType.AdultShort, SpriteType.SeniorShort]);
            SpriteAdultParalyzed = SpriteType.ParalyzedShort;
        }

        Skin = RNG.ChooseFrom(LooksGroups.SkinSprites);
        Reversed = RNG.Choose();

        // init_scars + init_accessories
        if (Source.Age is not AgeStage.Newborn)
        {
            int ScarChance, AccessoryChance;
            switch (Source.Age)
            {
                case AgeStage.Kitten:
                case AgeStage.Adolescent:
                    ScarChance = 50; AccessoryChance = 180; break;
                case AgeStage.YoungAdult:
                case AgeStage.Adult:
                    ScarChance = 20; AccessoryChance = 100; break;
                default:
                    ScarChance = 15; AccessoryChance = 80; break;
            }

            if (RNG.InverseChance(ScarChance))
                Scars.Add(RNG.ChooseFrom(RNG.Choose() ? LooksGroups.Scars : LooksGroups.SpecialScars));
            if (RNG.InverseChance(AccessoryChance))
                Accessory = RNG.ChooseFrom(RNG.Choose() ? LooksGroups.PlantAccessories : LooksGroups.WildAccessories);
        }

        // init_eye
        EyeColour = RNG.Choose() ? RNG.ChooseFrom(LooksGroups.EyeColours) : (RNG.Choose() ? FirstLooks : SecondLooks).EyeColour;

        int base_heterochromia = 120; // TEMPORARY CONFIG

        if (Colour is PeltColour.White || Array.IndexOf(LooksGroups.HighWhite, WhitePatches) > -1 || Array.IndexOf(LooksGroups.MostlyWhite, WhitePatches) > -1 || WhitePatches == "Fullwhite")
            base_heterochromia -= 90;
        if (Colour is PeltColour.White || WhitePatches == "Fullwhite")
            base_heterochromia -= 10;

        base_heterochromia += (FirstLooks.EyeColour2 is not null ? -10 : 0) + (SecondLooks.EyeColour2 is not null ? -10 : 0);

        if (RNG.InverseChance(base_heterochromia < 0 ? 1 : base_heterochromia))
        {
            if (Array.IndexOf(LooksGroups.YellowEyes, EyeColour) > -1)
                EyeColour2 = RNG.ChooseFrom(RNG.Choose() ? LooksGroups.BlueEyes : LooksGroups.GreenEyes);
            else if (Array.IndexOf(LooksGroups.BlueEyes, EyeColour) > -1)
                EyeColour2 = RNG.ChooseFrom(RNG.Choose() ? LooksGroups.YellowEyes : LooksGroups.GreenEyes);
            else if (Array.IndexOf(LooksGroups.GreenEyes, EyeColour) > -1)
                EyeColour2 = RNG.ChooseFrom(RNG.Choose() ? LooksGroups.BlueEyes : LooksGroups.YellowEyes);
        }

        // init_pattern
        if (Array.IndexOf(LooksGroups.Torties, Name) > -1)
        {
            TortieColour = PeltColour.Golden;
            TortieBase ??= RNG.ChooseFrom(LooksGroups.TortieBases);
            Pattern ??= RNG.ChooseFrom(LooksGroups.TortiePatterns);

            if (Colour is not PeltColour.None)
            {
                const int WildcardChance = 9; // TEMPORARY CONFIG
                if (WildcardChance == 0 || RNG.InverseChance(WildcardChance))
                {
                    TortiePattern = RNG.ChooseFrom(LooksGroups.TortieBases);

                    var PossibleColours = LooksGroups.PeltColours.ToList();
                    PossibleColours.Remove(Colour);

                    TortieColour = RNG.ChooseFrom(PossibleColours);
                }
                else
                {
                    if (TortieBase == "Singlestripe" || TortieBase == "Smoke" || TortieBase == "Single")
                        TortiePattern = RNG.ChooseFrom(LC_TortiePatterns);
                    else
                        TortiePattern = RNG.Chance(.97) ? TortieBase : "Single";

                    if (Colour is PeltColour.White)
                    {
                        var PossibleColours = LooksGroups.WhiteColours.ToList();
                        PossibleColours.Remove(PeltColour.White);
                        Colour = RNG.ChooseFrom(PossibleColours);
                    }

                    if (Array.IndexOf(LooksGroups.BlackColours, Colour) > -1 || Array.IndexOf(LooksGroups.WhiteColours, Colour) > -1)
                    {
                        TortieColour = RNG.ChooseFrom(RNG.Chance(1 / 3) ? LooksGroups.GingerColours : LooksGroups.BrownColours);
                    }
                    else if (Array.IndexOf(LooksGroups.GingerColours, Colour) > -1)
                    {
                        TortieColour = RNG.ChooseFrom(RNG.Chance(1 / 3) ? LooksGroups.BrownColours : LooksGroups.BrownColours);
                    }
                    else if (Array.IndexOf(LooksGroups.BrownColours, Colour) > -1)
                    {
                        var PossibleColours = LooksGroups.BrownColours.ToList();
                        PossibleColours.Remove(Colour);
                        TortieColour = RNG.ChooseFrom(
                            RNG.ChooseFrom([[.. PossibleColours], LooksGroups.BlackColours, LooksGroups.GingerColours], [1, 1, 2])
                        );
                    }
                }
            }
        }

        // init_tints
        string[] BaseTints = Tints.PossibleTints["Basic"], ColorTints = null;

        if (Tints.ColourGroups.TryGetValue(Colour, out string value)) Tints.PossibleTints.TryGetValue(value, out ColorTints);

        if (BaseTints is not null || ColorTints is not null)
            Tint = RNG.ChooseFrom((RNG.Choose() ? BaseTints : ColorTints) ?? BaseTints ?? ColorTints);

        if (WhitePatches is not null || Points is not null)
        {
            PatchTints.PossibleTints.TryGetValue("Basic", out BaseTints);
            ColorTints = null;
            if (Tints.ColourGroups.TryGetValue(Colour, out string Value))
                ColorTints = Tints.PossibleTints[Value];
            if (BaseTints is not null || ColorTints is not null)
                PatchTint = RNG.ChooseFrom((RNG.Choose() ? BaseTints : ColorTints) ?? BaseTints ?? ColorTints);
        }
    }

    /// <summary>
    /// Generate all pelt attributes randomly.
    /// </summary>
    public bool GeneratePattern([DisallowNull] RandomEx RNG, [DisallowNull] Cat Source)
    {
        string ChosenPelt = RNG.ChooseFrom(
            RNG.ChooseFrom(LooksGroups.PeltCategories, RPC_PeltWeights)
        );

        PeltColour ChosenPeltColour = RNG.ChooseFrom(LooksGroups.ColourCategories);

        string ChosenTortieBase = null;


        const int tortie_chance_female = 3, tortie_chance_male = 13; // TEMPORARY CONFIG
        if (RNG.ChanceEx2(Source.Sex ? tortie_chance_male : tortie_chance_female))
        {
            ChosenTortieBase = ChosenTortieBase == "TwoColour" || ChosenTortieBase == "SingleColour" ? "Single" : ChosenPelt;
            ChosenPelt = RNG.ChooseFrom(LooksGroups.Torties);
        }

        bool PeltHasWhite = RNG.Chance(.4);

        if (ChosenPelt == "SingleColour" || ChosenPelt == "TwoColour")
            ChosenPelt = PeltHasWhite ? "TwoColour" : "SingleColour";
        else if (ChosenPelt == "Calico" && !PeltHasWhite)
            ChosenPelt = "Tortie";

        Name = ChosenPelt;
        Colour = ChosenPeltColour;
        Length = RNG.ChooseEnum<PeltLength>();
        TortieBase = ChosenTortieBase;
        return PeltHasWhite;
    }

    /// <summary>
    /// Generate all pattern attributes, influenced from given <paramref name="Parent1"/> and <paramref name="Parent2"/>.
    /// </summary>
    public bool GeneratePattern([DisallowNull] RandomEx RNG, [DisallowNull] Cat Source, [DisallowNull] Cat Parent1, [DisallowNull] Cat Parent2)
    {
        Looks FirstLooks = Parent1.Looks, SecondLooks = Parent2.Looks;
        string ChosenPelt, ChosenTortieBase = null;
        PeltColour ChosenPeltColour;

        const int direct_inheritance = 10; // TEMPORARY CONFIG
        if (RNG.InverseChance(direct_inheritance))
        {
            Looks Selected = RNG.Choose() ? FirstLooks : SecondLooks;
            Name = Selected.Name; Length = Selected.Length; Colour = Selected.Colour; TortieBase = Selected.TortieBase;
            return Selected.HasWhite;
        }

        int[] Weights = [0, 0, 0, 0, 0];

        if (Array.IndexOf(LooksGroups.Tabbies, FirstLooks) > -1)
            Weights = [50, 10, 5, 7, 0];
        else if (Array.IndexOf(LooksGroups.Spotted, FirstLooks) > -1)
            Weights = [10, 50, 5, 5, 0];
        else if (Array.IndexOf(LooksGroups.Plain, FirstLooks) > -1)
            Weights = [5, 5, 50, 0, 0];
        else if (Array.IndexOf(LooksGroups.Exotic, FirstLooks) > -1)
            Weights = [15, 15, 1, 45, 0];

        if (Array.IndexOf(LooksGroups.Tabbies, SecondLooks) > -1)
            Weights = [.. Weights, .. PCI_TabbiesWeights];
        else if (Array.IndexOf(LooksGroups.Spotted, SecondLooks) > -1)
            Weights = [.. Weights, .. PCI_SpottedWeights];
        else if (Array.IndexOf(LooksGroups.Plain, SecondLooks) > -1)
            Weights = [.. Weights, .. PCI_PlainWeights];
        else if (Array.IndexOf(LooksGroups.Exotic, SecondLooks) > -1)
            Weights = [.. Weights, .. PCI_ExoticWeights];

        ChosenPelt = RNG.ChooseFrom(
            Weights.All(X => X == 0) ? RNG.ChooseFrom<string[]>(LooksGroups.PeltCategories) : RNG.ChooseFrom(LooksGroups.PeltCategories, Weights)
        );

        int tortie_chance_male = 3, tortie_chance_female = 13; // TEMPORARY CONFIG

        if (Array.IndexOf(LooksGroups.Tabbies, FirstLooks.Name) > -1)
            tortie_chance_female /= 2; tortie_chance_male -= 1;
        if (Array.IndexOf(LooksGroups.Tabbies, SecondLooks.Name) > -1)
            tortie_chance_female /= 2; tortie_chance_male -= 1;

        if (RNG.ChanceEx2(Source.Sex ? tortie_chance_male : tortie_chance_female))
        {
            ChosenTortieBase = ChosenTortieBase == "TwoColour" || ChosenTortieBase == "SingleColour" ? "Single" : ChosenPelt;
            ChosenPelt = RNG.ChooseFrom(LooksGroups.Torties);
        }

        Weights = [0, 0, 0, 0];

        if (Array.IndexOf(LooksGroups.GingerColours, FirstLooks) > -1)
            Weights = [40, 0, 0, 10];
        else if (Array.IndexOf(LooksGroups.BlackColours, FirstLooks) > -1)
            Weights = [0, 40, 2, 5];
        else if (Array.IndexOf(LooksGroups.WhiteColours, FirstLooks) > -1)
            Weights = [0, 5, 40, 0];
        else if (Array.IndexOf(LooksGroups.BrownColours, FirstLooks) > -1)
            Weights = [10, 5, 0, 35];

        if (Array.IndexOf(LooksGroups.GingerColours, SecondLooks) > -1)
            Weights = [.. Weights, .. PCI_GingerWeights];
        else if (Array.IndexOf(LooksGroups.BlackColours, SecondLooks) > -1)
            Weights = [.. Weights, .. PCI_BlackWeights];
        else if (Array.IndexOf(LooksGroups.WhiteColours, SecondLooks) > -1)
            Weights = [.. Weights, .. PCI_WhiteWeights];
        else if (Array.IndexOf(LooksGroups.BrownColours, SecondLooks) > -1)
            Weights = [.. Weights, .. PCI_BrownWeights];

        ChosenPeltColour = RNG.ChooseFrom(
            Weights.All(X => X == 0) ? RNG.ChooseFrom<PeltColour[]>(LooksGroups.ColourCategories) : RNG.ChooseFrom(LooksGroups.ColourCategories, Weights)
        );

        switch (FirstLooks.Length)
        {
            case PeltLength.Short: 
                Weights = [50, 10, 2]; break;
            case PeltLength.Medium: 
                Weights = [25, 50, 25]; break;
            case PeltLength.Long: 
                Weights = [2, 10, 50]; break;
        }
        switch (SecondLooks.Length)
        {
            case PeltLength.Short: 
                Weights = [.. Weights, .. PCI_ShortWeights]; break;
            case PeltLength.Medium: 
                Weights = [.. Weights, .. PCI_MediumWeights]; break;
            case PeltLength.Long: 
                Weights = [.. Weights, .. PCI_LongWeights]; break;
        }

        bool PeltHasWhite = RNG.Chance(.97);

        if (ChosenPelt == "SingleColour" || ChosenPelt == "TwoColour")
            ChosenPelt = PeltHasWhite ? "TwoColour" : "SingleColour";
        else if (ChosenPelt == "Calico" && !PeltHasWhite)
            ChosenPelt = "Tortie";

        Name = ChosenPelt;
        Colour = ChosenPeltColour;
        Length = RNG.ChooseEnum<PeltLength>();
        TortieBase = ChosenTortieBase;
        return PeltHasWhite;
    }

    /// <summary>
    /// Generate patches and possibly points randomly.
    /// </summary>
    public void GeneratePatches([DisallowNull] RandomEx RNG)
    {
        const int random_point_chance = 5; // TEMPORARY CONFIG

        if (Name != "Tortie" && RNG.InverseChance(random_point_chance))
            Points = RNG.ChooseFrom(LooksGroups.PointMarkings);

        switch (RNG.ChooseFromWeights<int>(Name == "Tortie" ? [2, 1, 0, 0, 0] : (Name == "Calico" ? [0, 0, 20, 15, 1] : [10, 10, 10, 10, 1])))
        {
            case 0:
                WhitePatches = RNG.ChooseFrom(LooksGroups.LittleWhite);
                return;
            case 1:
                WhitePatches = RNG.ChooseFrom(LooksGroups.MiddleWhite);
                return;
            case 2:
                WhitePatches = RNG.ChooseFrom(LooksGroups.HighWhite);
                if (Points is not null) Points = null;
                return;
            case 3:
                WhitePatches = RNG.ChooseFrom(LooksGroups.MostlyWhite);
                if (Points is not null) Points = null;
                return;
            case 4:
                WhitePatches = "Fullwhite";
                if (Points is not null) Points = null;
                return;
        }
    }

    /// <summary>
    /// Generate patches and possibly points, influenced from given <paramref name="Parent1"/> and <paramref name="Parent2"/>
    /// </summary>
    public void GeneratePatches([DisallowNull] RandomEx RNG, [DisallowNull] Cat Parent1, [DisallowNull] Cat Parent2)
    {
        Looks FirstLooks = Parent1.Looks, SecondLooks = Parent2.Looks;
            

        List<string> ParentPoints = [], ParentPatches = [];

        if (FirstLooks.WhitePatches is not null) ParentPoints.Add(FirstLooks.WhitePatches);
        if (SecondLooks.WhitePatches is not null) ParentPoints.Add(SecondLooks.WhitePatches);
        if (FirstLooks.Points is not null) ParentPoints.Add(FirstLooks.Points);
        if (SecondLooks.Points is not null) ParentPoints.Add(SecondLooks.Points);

        const int direct_inheritance = 10; // TEMPORARY CONFIG
        if (0 < ParentPatches.Count && RNG.InverseChance(direct_inheritance))
        {
            List<string> Temp = [];
            string Patch;

            if (Name == "Tortie")
            {
                for (int i = 0; i < ParentPatches.Count; i++)
                    if (Array.IndexOf(LooksGroups.HighWhite, Patch = ParentPatches[i]) == -1 && Array.IndexOf(LooksGroups.MostlyWhite, Patch) == -1 && Patch != "Fullwhite")
                        Temp.Add(Patch);
            }
            else if (Name == "Calico")
            {
                for (int i = 0; i < ParentPatches.Count; i++)
                    if (Array.IndexOf(LooksGroups.LittleWhite, Patch = ParentPatches[i]) == -1 && Array.IndexOf(LooksGroups.MiddleWhite, Patch) == -1)
                        Temp.Add(Patch);
            }
                
            if (0 < Temp.Count)
            {
                WhitePatches = RNG.ChooseFrom(Temp);
                if (Name != "Tortie" && 0 < ParentPoints.Count)
                    Points = RNG.ChooseFrom(ParentPoints);

                return;
            }
        }

        if (Name != "Tortie" && RNG.InverseChance(0 < ParentPoints.Count ? 10 - ParentPoints.Count : 40))
            Points = RNG.ChooseFrom(LooksGroups.PointMarkings);

        int[] Weights = [0, 0, 0, 0, 0];
        for (int i = 0; i < ParentPatches.Count; i++)
        {
            var Patch = ParentPatches[i];
            int[] AddWeights = [0, 0, 0, 0, 0];

            if (Array.IndexOf(LooksGroups.LittleWhite, Patch) > -1)
                AddWeights = [40, 20, 15, 5, 0];
            else if (Array.IndexOf(LooksGroups.MiddleWhite, Patch) > -1)
                AddWeights = [10, 40, 15, 10, 0];
            else if (Array.IndexOf(LooksGroups.HighWhite, Patch) > -1)
                AddWeights = [15, 20, 40, 10, 1];
            else if (Array.IndexOf(LooksGroups.MostlyWhite, Patch) > -1)
                AddWeights = [5, 15, 20, 40, 5];
            else if (Patch == "Fullwhite")
                AddWeights = [0, 5, 15, 40, 10];

            for (int k = 0; k < 5; k++)
                Weights[k] = AddWeights[k];
        }

        if (Weights[1] != 0) Weights = [50, 5, 0, 0, 0];// Check whether or not weights habe been altered

        // Adjust weights for torties and calicos since they can't have anything greater than MiddleWhite
        if (Name == "Tortie")
            Weights = Weights[1] != 0 ? [2, 1, 0, 0, 0] : [Weights[0], Weights[1], 0, 0, 0];
        else if (Name == "Calico") 
            Weights = Weights[1] != 0 ? [2, 1, 0, 0, 0] : [Weights[0], Weights[1], 0, 0, 0];

        switch (RNG.ChooseFromWeights(Weights))
        {
            case 0:
                WhitePatches = RNG.ChooseFrom(LooksGroups.LittleWhite);
                return;
            case 1:
                WhitePatches = RNG.ChooseFrom(LooksGroups.MiddleWhite);
                return;
            case 2:
                WhitePatches = RNG.ChooseFrom(LooksGroups.HighWhite);
                if (Points is not null)
                    Points = null;
                return;
            case 3:
                WhitePatches = RNG.ChooseFrom(LooksGroups.MostlyWhite);
                if (Points is not null) Points = null;
                return;
            case 4:
                WhitePatches = "Fullwhite";
                if (Points is not null) Points = null;
                return;
        }
    }

    /// <summary>
    /// Get the stored <see cref="SpriteType"/> Value that lines up with the given <see cref="AgeStage"/> Value
    /// </summary>
    /// <exception cref="NotImplementedException">Raised when switch-case statement hasnt been updated for any newly added <see cref="Age"/> Values</exception>
    public byte GetSpriteType(AgeStage Value) => Value switch
    {
        AgeStage.Newborn => SpriteNewborn,
        AgeStage.Kitten => SpriteKitten,
        AgeStage.Adolescent => SpriteAdolescent,
        AgeStage.YoungAdult => SpriteYoungAdult,
        AgeStage.Adult => SpriteAdult,
        AgeStage.SeniorAdult => SpriteSeniorAdult,
        AgeStage.Senior => SpriteSenior,
        _ => throw new NotImplementedException(), // Only here to tell if a new Age value has been added and not accounted for
    };

    /// <summary>
    /// Get the sprite texture id from a given <see cref="Name"/> value
    /// </summary>
    public static string GetSpriteName([DisallowNull] string Value) => Value switch
    {
        "SingleColour" or "TwoColour" => "Single", "Tortie" or "Calico" => null, _ => Value,
    };

    public static string GetDescription()
    {
        throw new NotImplementedException();
    }

    public static string GetDescriptionShort()
    {
        throw new NotImplementedException();
    }

    public static string GetDescriptionOfEyes()
    {
        throw new NotImplementedException();
    }
}