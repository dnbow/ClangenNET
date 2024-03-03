
using System.Collections.Generic;

namespace ClangenNET;

public abstract class Condition
{

}


public class Health
{
    public bool Dead { get; private set; } = false;

    public bool Pregnant; // TEMPORARY

    public readonly List<Condition> Conditions;
    public readonly List<Condition> Injuries;
    public readonly List<Condition> Illnesses;


    /// <summary>
    /// A signed byte Ranges from -128 to 127, with -128 meaning supernaturally unlucky.
    /// </summary>
    public sbyte Luck;


    public void Die()
    {
        Dead = true;

        Injuries.Clear();
        Illnesses.Clear();
    }
}

