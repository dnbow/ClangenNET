using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClangenNET;

public partial class Cat
{
    public enum SkillPath : byte
    {

    }

    public string[] Traits; // TODO abstract string to struct type

    /// <summary>
    /// Byte representing the luck of this <see cref="Cat"/>, with -128 is reserved for supernaturally unlucky.
    /// </summary>
    public byte Luck;

    /// <summary>
    /// Byte representing how fair this <see cref="Cat"/> is.
    /// </summary>
    public byte Fairness;

    /// <summary>
    /// Byte representing how social this <see cref="Cat"/> is.
    /// </summary>
    public byte Sociability;

    /// <summary>
    /// Signed byte representing this cats sanity, with -128 resrved for forever insane.
    /// </summary>
    public sbyte Sanity;
}