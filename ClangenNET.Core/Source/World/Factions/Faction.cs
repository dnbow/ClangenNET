using System;
using System.Reflection;
using System.Collections.Generic;

using ClangenNET.Factions.Afterlife;
using ClangenNET.Factions;

namespace ClangenNET;

public static partial class Context
{
    private static FactionDef[] FactionDefs;
    private static readonly Faction[] FactionInstances = new Faction[256];
    private static byte FactionCount = 0;

    public static readonly StarClan StarClan = new ();
    public static readonly DarkForest DarkForest = new ();
    public static readonly Purgatory Purgatory = new ();

    /// <summary>
    /// Represents everything needed to create a <see cref="Faction"> instance.
    /// </summary>
    private readonly struct FactionDef
    {
        private readonly Type Type;
        public readonly string Name;

        public FactionDef(Type Type)
        {
            this.Type = Type;
            Name = Type.Name;
        }

        public Faction CreateInstance()
        {
            return CreateInstance(out byte _); // TEMPORARY until code is concrete
        }

        public Faction CreateInstance(out byte Id)
        {
            if (FactionCount < 255)
                return FactionInstances[Id = ++FactionCount] = Activator.CreateInstance(Type) as Faction;

            Id = 0;
            return null;
        }
    }


    internal static partial void Load(Assembly[] Assemblies)
    {
        Type[] Types;
        Type Type;

        List<FactionDef> FactionTypes = new ();

        for (int i = 0; i < Assemblies.Length; i++)
        {
            Types = Assemblies[i].GetTypes();

            for (int k = 0; k < Types.Length; k++)
            {
                Type = Types[k];

                if (Type != typeof(Faction) && Type != typeof(GlobalFaction) && Type.IsSubclassOf(typeof(Faction)) && !Type.IsSubclassOf(typeof(GlobalFaction)))
                    FactionTypes.Add(new FactionDef(Type));
                
            }
        }

        FactionDefs = FactionTypes.ToArray();
    }


    public static Faction CreateFaction(string Name)
    {
        FactionDef Current;

        for (int i = 0; i < FactionDefs.Length; i++)
            if ((Current = FactionDefs[i]).Name == Name)
                return Current.CreateInstance();

        return null;
    }
}

public abstract class Faction
{
    # region Listeners
    /// <summary>
    /// Method to be called when a Cat within this faction dies.
    /// </summary>
    /// <param name="BodyIsRecovered">Whether or not the body of the deceased was found</param>
    public virtual bool OnDeath(Cat Cat, bool BodyIsRecovered) => true;

    /// <summary>
    /// Method to be called when a Cat within this faction dies forever i.e a cat fades from starclan
    /// </summary>
    public virtual bool OnPermaDeath(Cat Cat) => true;

    /// <summary>
    /// Method to be called when a Cat leaves this faction
    /// </summary>
    public virtual void OnLeave(Cat Cat) { }

    /// <summary>
    /// Method to be called when a Cat is forcably removed from this faction
    /// </summary>
    public virtual void OnForceLeave(Cat Cat) {}
    #endregion

    #region Commands

    /// <summary>
    /// Method to be called when a Cat joins this faction
    /// </summary>
    public virtual void Add(Cat Cat) {}

    /// <summary>
    /// Method to decide where a Cat should be in the afterlife and place them there.
    /// </summary>
    public virtual void SendToAfterlife(Cat Cat) {}
    #endregion
}



public abstract class GlobalFaction : Faction
{

}