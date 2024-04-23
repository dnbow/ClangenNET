
using System.Collections.Generic;

namespace ClangenNET;

public enum ConditionSeverity : byte
{
    Minor, Major, Severe
}



public partial class Cat
{
    public bool Dead { get; private set; } = false;


}

