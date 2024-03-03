using System;

using static ClangenNET.Context;

namespace ClangenNET
{
    [Flags]
    public enum RelationType : byte
    {
        None = 0x0,
        Romantic = 0x1,
        Platonic = 0x2,
    }



    public enum KinRelationType : byte
    {
        None,
        Child,
        Parent,
        Sibling,
        Cousin,
    }



    public struct Relation
    {
        public readonly CatRef Source;
        public readonly CatRef Target;


        public readonly RelationType Type;
         
        public readonly KinRelationType KinType;

        /// <summary>
        /// A byte representing how much a <see cref="Cat"/> knows about another <see cref="Cat"/>.
        /// </summary>
        public byte Familiarity;

        /// <summary>
        /// A signed byte representing how much a <see cref="Cat"/> likes or dislikes another 
        /// <see cref="Cat"/>. -128 is reserved for irreversable hate.
        /// </summary>
        public sbyte Approval;
    }



    public class Relations
    {

    }
}
