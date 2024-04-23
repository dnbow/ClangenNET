using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClangenNET;

public partial class Cat
{
    public struct Trait
    {

    }

    public Trait[] Traits;

    /// <summary>
    /// Byte representing the luck of this <see cref="Cat"/>, with -128 is reserved for supernaturally unlucky.
    /// </summary>
    public byte Luck;


    public byte Lawfulness;

    public byte Sociability;

    public byte Stability;
}