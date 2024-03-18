using System;
using System.Collections.Generic;
using System.Linq;
using static ClangenNET.Runtime;
using static ClangenNET.Utility;

namespace ClangenNET;

public static partial class Runtime
{
    internal static Dictionary<ushort, Cat> Cats = new(512);
    internal static ushort LastCatId = 0;

    public static ushort AddCat(Cat Cat)
    {
        bool CatWithThisIdExists = false;
        ushort NewId = LastCatId;

        // Try and get an available id for x amount of times because I want to stay away from while loops where possible
        for (int i = 0; i < 250; i++)
            if (CatWithThisIdExists = Cats.TryGetValue(++NewId, out Cat _))
                continue;
            else
                break;

        if (CatWithThisIdExists) // If no id is found, just skip to getting the max
            NewId = (ushort)(1 + Cats.Keys.Max());

        Cats[NewId] = Cat;
        return LastCatId = NewId;
    }

    public static Cat GetCat(ushort Id) => Cats.TryGetValue(Id, out Cat Existing) ? Existing : null;
}



/// <summary>
/// Represents a reference to a <see cref="Cat"> object
/// </summary>
public readonly struct CatRef(ushort Id)
{
    public static readonly CatRef None = new(0);

    /// <summary>
    /// The ID of the <see cref="Cat"/> object
    /// </summary>
    public readonly ushort Id = Id;

    /// <summary>
    /// Boolean of if this CatRef leads anywhere.
    /// </summary>
    public readonly bool IsValid = 0 < Id && Id <= LastCatId;

    public static implicit operator Cat(CatRef Value) => Cats.TryGetValue(Value.Id, out Cat Existing) ? Existing : null;
    public static implicit operator CatRef(Cat Value) => new(Value.Id);
}


public partial class Cat : IEquatable<Cat>
{


    /// <summary>
    /// Represents a set of translation keys pointing to Pronouns.
    /// </summary>
    public readonly struct Pronoun // TODO revisit
    {
        public readonly TranslationKey Subject;

        public readonly TranslationKey Object;

        public readonly TranslationKey Possesive;

        public readonly TranslationKey Inpossesive;

        public readonly TranslationKey Reflexive;

        /// <summary>
        /// Represents <see langword="true"/> if this pronoun is Plural, <see langword="false"/> 
        /// if it is Singular (they > plural > true, he > singular > false).
        /// </summary>
        public readonly bool Conjugate; // TEMP
    }

    /// <summary>
    /// Helper interface to allow for different naming conventions.
    /// </summary>
    public interface ICatName
    {
        /// <summary>
        /// Method to create an <see cref="ICatName"/> regardless of any given condition, returns whether or not name creation was successful.
        /// </summary>
        public bool Create();

        /// <summary>
        /// Method to create a Name from a given <see cref="Cat"/>, returns whether or not <see cref="ICatName"/> creation was successful. <br/>
        /// This doesnt have to be implemented, but it allows people to create a name conditionally based around the cat.
        /// </summary>
        public bool Create(Cat Cat) => Create();

        /// <summary>
        /// Method to check if this <see cref="ICatName"/> is valid.
        /// </summary>
        public bool IsValidName();

        /// <summary>
        /// Method to return a string representation of this <see cref="ICatName"/>.
        /// </summary>
        public string GetName();
    }

    public enum AgeStage : byte
    {
        Newborn, Kitten, Adolescent, YoungAdult, Adult, SeniorAdult, Senior
    }

    /// <summary>
    /// Non-zero, unique number that should only be assigned by <see cref="CatManager"/> (in constructor of course)
    /// </summary>
    public readonly ushort Id;

    /// <summary>
    /// Number used as a starting seed for this cats' generation.
    /// </summary>
    public readonly uint Seed;

    /// <summary>
    /// This cats name.
    /// </summary> 
    public readonly ICatName Name;

    /// <summary>
    /// Biological sex -> <see langword="true"/> for Male and <see langword="false"/> for Female.
    /// </summary>
    public readonly bool Sex;


    public readonly Looks Looks;

    /// <summary>
    /// The moon a cat was born
    /// </summary>
    public readonly uint BirthMoon;


    public readonly Faction Faction;


    public TranslationKey Thought;

    /// <summary>
    /// Get <see cref="AgeStage"/> Enum based on this cats age (gathered from birth to dead).
    /// </summary>
    public AgeStage Age => (ThisWorld.Moon - BirthMoon) switch
    {
            0 => AgeStage.Newborn,
        <   6 => AgeStage.Kitten,
        <  12 => AgeStage.Adolescent,
        <  48 => AgeStage.YoungAdult,
        <  96 => AgeStage.Adult,
        < 120 => AgeStage.SeniorAdult,
            _ => AgeStage.Senior
    };

    /// <summary>
    /// The biological age of this Cat. If this cat dies, they will stay as their biological age.
    /// </summary>
    public uint Moons { get; internal set; }

    /// <summary>
    /// The chronological age of this Cat. If this cat dies, this will continue to tick.
    /// </summary>
    public uint MoonsChronological => ThisWorld.Moon - BirthMoon;

    public static bool operator ==(Cat Cat, CatRef Ref) => Cat.Id == Ref.Id;
    public static bool operator !=(Cat Cat, CatRef Ref) => Cat.Id != Ref.Id;
    public override string ToString() => $"CAT {Id} {Seed}";
    public override bool Equals(object obj) => Equals(obj as Cat);
    public override int GetHashCode() => (int)Seed;

    /// <summary>
    /// Check if two cats are the same Cat
    /// </summary>
    public bool Equals(Cat Cat) => ReferenceEquals(this, Cat) || Id == Cat.Id;

    /// <summary>
    /// Checks if this Cat is considered a baby.
    /// </summary>
    public bool IsBaby() => Moons < 6;

}
