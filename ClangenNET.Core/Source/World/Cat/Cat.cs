using System;
using System.Linq;
using System.Collections.Generic;

using static ClangenNET.Utility;
using static ClangenNET.Context;
using System.Runtime.CompilerServices;

namespace ClangenNET;

public static partial class Context
{
    private static readonly Dictionary<ushort, Cat> Cats = new(512);
    private static ushort LastCatId = 0;

    /// <summary>
    /// Represents a reference to a <see cref="Cat"> object
    /// </summary>
    public readonly struct CatRef
    {
        public static readonly CatRef None = new(0);

        /// <summary>
        /// The ID of the <see cref="Cat"/> object
        /// </summary>
        public readonly ushort Id = 0;

        public CatRef(ushort Id)
        {
            this.Id = Id;
        }

        public bool IsValid() => Id > 0;

        public static implicit operator Cat(CatRef Value) => Cats[Value.Id];
        public static implicit operator CatRef(Cat Value) => new(Value.Id);
    }


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

        if (CatWithThisIdExists)
            NewId = (ushort)(1 + Cats.Keys.Max()); // If no id is found, just skip to getting the max

        Cats[NewId] = Cat;
        return LastCatId = NewId;
    }
}



public partial class Cat : IEquatable<Cat>
{
    /// <summary>
    /// Helper interface to allow for different naming conventions.
    /// </summary>
    public interface IName
    {
        /// <summary>
        /// Create a Name regardless of any given condition, returns whether or not name creation was successful.
        /// </summary>
        public bool Create();

        /// <summary>
        /// Create a Name from a given Cat, returns whether or not name creation was successful. <br/>
        /// This doesnt have to be implemented, but it allows people to create a name conditionally based around the cat.
        /// </summary>
        public bool Create(Cat Cat)
        {
            return Create();
        }

        /// <summary>
        /// Checks if this name is valid.
        /// </summary>
        public bool IsValidName();

        /// <summary>
        /// Returns this name in string form.
        /// </summary>
        public string GetName();
    }



    public readonly struct PronounRef
    {
        /// <summary>
        /// The Identifer to be used in saves, and when getting Pronoun string variants
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// Represents <see langword="true"/> if this pronoun is Plural, <see langword="false"/> 
        /// if it is Singular (they > plural > true, he > singular > false).
        /// </summary>
        public readonly bool Conjugate; // TEMP
    }



    public enum AgeStage : byte
    {
        Newborn,
        Kitten,
        Adolescent,
        YoungAdult,
        Adult,
        SeniorAdult,
        Senior
    }

    /// <summary>
    /// A non-zero, unique unsigned short that should only be assigned by <see cref="CatManager"/> (in constructor of course)
    /// </summary>
    public readonly ushort Id;

    /// <summary>
    /// An unsigned integer used as a starting seed for this cats' generation.
    /// </summary>
    public readonly uint Seed;

    /// <summary>
    /// The Cats name. Use <see cref="IName.GetName()"/> to get string representation, see 
    /// <see cref="IName"/> for more info
    /// </summary> 
    public readonly IName Name;

    /// <summary>
    /// A bool representing biological sex -> <see langword="true"/> for Male and <see langword="false"/> for Female.
    /// </summary>
    public readonly bool Sex;


    public readonly Looks Looks;


    public readonly Health Health;

    /// <summary>
    /// The moon a cat was born
    /// </summary>
    public readonly uint BirthMoon;


    public readonly Faction Faction;


    public TranslationKey Thought;

    /// <summary>
    /// Get <see cref="Age"/> Enum based on this cats age assuming of course theyre still alive
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAlive() => !Health.Dead;


    public static bool operator ==(Cat Cat, CatRef Ref) => Cat.Id == Ref.Id;
    public static bool operator !=(Cat Cat, CatRef Ref) => Cat.Id != Ref.Id;


    public Cat()
    {

    }


    public override string ToString() => $"CAT {Id} {Seed}";
    public override bool Equals(object obj) => Equals(obj as Cat);
    public override int GetHashCode() => (int)Seed;

    /// <summary>
    /// Check if two cats are the same Cat
    /// </summary>
    public bool Equals(Cat Cat) => ReferenceEquals(this, Cat) || Id == Cat.Id;

    /// <summary>
    /// Kills a cat, but in a reversable way.
    /// </summary>
    /// <param name="BodyIsRecovered">Whether or not the body of the deceased was found</param>
    public void Kill(bool BodyIsRecovered)
    {
        Health.Die();
        Faction.OnDeath(this, BodyIsRecovered);
    }

    /// <summary>
    /// Kills a cat in such a way that there is no way to bring them back without regenerating from
    /// seed, which could be identical to them before they died or a version of them from many moons ago.
    /// </summary>
    public void KillForever()
    {
        Health.Die();
        Faction.OnPermaDeath(this);
    }


}
