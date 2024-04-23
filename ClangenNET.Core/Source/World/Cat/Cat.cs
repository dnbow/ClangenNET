using System;
using System.Collections.Generic;
using static ClangenNET.Content;

namespace ClangenNET;



/// <summary>
/// Represents a reference to a <see cref="Cat"> object
/// </summary>
public readonly struct CatRef(ushort Id)
{
    /// <summary>
    /// A CatRef leading nowhere.
    /// </summary>
    public static readonly CatRef None = new(0);

    /// <summary>
    /// The ID of the <see cref="Cat"/> object.
    /// </summary>
    public readonly ushort Id = Id;

    /// <summary>
    /// Boolean of if this CatRef leads anywhere.
    /// </summary>
    public readonly bool IsValid = 0 < Id && Id <= Cat.LastCatId;

    public static implicit operator Cat?(CatRef Value) => Cat.Cats.TryGetValue(Value.Id, out Cat? Existing) ? Existing : null;
    public static implicit operator CatRef(Cat Value) => new(Value.Id);
}

public partial class Cat : IEquatable<Cat>, IEquatable<CatRef>
{
    internal static readonly Dictionary<ushort, Cat> Cats = new(512);
    internal static ushort LastCatId = 0;

    public enum AgeStage
    {
        Newborn, Kitten, Adolescent, YoungAdult, Adult, SeniorAdult, Senior
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

    /// <summary>
    /// Non-zero, unique number.
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
    public AgeStage Age => (BirthMoon) switch // FIX -> doesnt do the from death part, dont know how to hold moons just yet though!
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
    /// The biological age of this Cat. If this cat dies, this will stop ticking.
    /// </summary>
    public uint Moons { get; internal set; }

    /// <summary>
    /// The chronological age of this Cat. If this cat dies, this will continue to tick.
    /// </summary>
    public uint MoonsChronological => BirthMoon; // FIX

    public static Cat? Get(ushort Id)
    {
        return Cats.TryGetValue(Id, out Cat? Existing) ? Existing : null;
    }

    public Cat(uint Seed)
    {
        this.Seed = Seed;

        Cats[++LastCatId] = this;
        Looks = new(this);
    }

    public static bool operator ==(Cat Cat, CatRef Ref) => Cat.Id == Ref.Id;
    public static bool operator !=(Cat Cat, CatRef Ref) => Cat.Id != Ref.Id;
    public override string ToString() => $"CAT {Id} {Seed}";
    public override bool Equals(object? Obj)
    {
        if (Obj is null)
            return false;
        if (Obj is Cat objCat)
            return Equals(objCat);

        return false;
    }

    public override int GetHashCode() => (int)Seed;

    /// <summary>
    /// Check if two cats are the same Cat
    /// </summary>
    public bool Equals(Cat? Cat)
    {
        if (Cat is null)
            return false;

        return ReferenceEquals(this, Cat) || Cat.Id == Id;
    }

    public bool Equals(CatRef Other)
    {
        return Other.Id == Id;
    }

    /// <summary>
    /// Checks if this Cat is considered a baby.
    /// </summary>
    public bool IsBaby() => Moons < 6;
}
