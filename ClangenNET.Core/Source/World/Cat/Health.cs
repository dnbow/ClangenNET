
using System.Collections.Generic;

namespace ClangenNET;

public enum ConditionSeverity
{
    Minor, Major, Severe
}

public struct Condition
{
    public struct Stage
    {

    }

    public readonly string Id;
    public readonly Stage[] Stages;
}

public partial class Cat
{
    public bool Dead { get; private set; } = false;

    public readonly List<Condition> Conditions;
    public readonly List<Condition> Injuries;
    public readonly List<Condition> Illnesses;


    public void Die()
    {
        Dead = true;

        Injuries.Clear();
        Illnesses.Clear();
    }
}

