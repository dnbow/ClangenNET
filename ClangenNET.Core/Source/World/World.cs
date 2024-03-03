using System.IO;

using ClangenNET;
using ClangenNET.Factions;

using static ClangenNET.Utility;

namespace ClangenNET
{
    public enum GamemodeType : byte
    {
        Classic, Expanded, Cruel
    }

    public enum Season : byte
    {
        Summer, Autumn, Winter, Spring
    }



    public partial class WorldView
    {

    }



    public partial class World
    {
        public string SaveName;


        public GamemodeType GameMode;


        public Season Season;


        public uint Moon { get; private set; } = 0;


        public readonly PlayerClan Clan;


        public World()
        {

        }


        public bool Save()
        {
            string Path = $"Saves\\Test.sav"; // Points to one file for now

            using (FileStream Output = File.OpenWrite(Path))
            {
                Output.Write(
                    new byte[]
                    {
                        Version.Major, Version.Minor, Version.Revision & 0xFF, (Version.Revision & 0xFF00) >> 8, // Version data
                    }
                );
            }

            return true;
        }
    }
}