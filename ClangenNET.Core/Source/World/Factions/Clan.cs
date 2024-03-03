using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static ClangenNET.Context;

namespace ClangenNET.Factions
{
    /// <summary>
    /// Represents an in-game clan, whether that be the player clan or another clan.
    /// </summary>
    public class Clan : Faction
    {
        public enum Status : byte
        {
            None, Elder, Apprentice, Warrior, MediatorApprentice, Mediator, MedicineCatApprentice, MedicineCat, Deputy, Leader
        }

        public CatRef Leader;
        public byte LeaderLives = 0;

        public CatRef Deputy;
        public CatRef MedCat;
        public CatRef Instructor;

        /// <summary>
        /// Whether or not the Instructor will lead this clan Starclan (<see langword="true"/>) 
        /// or the Dark Forest (<see langword="false"/>)
        /// </summary>
        public bool StarclanOriented;

        /// <summary>
        /// A dictionary with Key to mentors, and Value to their apprentice(s).
        /// </summary>
        public readonly Dictionary<CatRef, List<CatRef>> MentorApprenticePairs;

        /// <summary>
        /// A dictionary with Key to cats and Value to their role within the clan.
        /// </summary>
        public readonly Dictionary<CatRef, Status> Roles;

        public Clan()
        {
            MentorApprenticePairs = new (3);
        }

        public new void SendToAfterlife(Cat Cat)
        {
            if (!Instructor.IsValid())
                Purgatory.Add(Cat);
            else if (StarclanOriented)
                StarClan.Add(Cat);
            else
                DarkForest.Add(Cat);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidMentorTo(Cat Mentor, Cat Apprentice) =>
            Roles.TryGetValue(Apprentice, out Status AppRole) && Roles.TryGetValue(Mentor, out Status MentorRole) // Check if roles have been assigned
                && Mentor.IsAlive() && Apprentice.IsAlive() // Check if mentor is able to work
                && ( // Check if the role of both match each other
                    (AppRole is Status.Apprentice && (MentorRole is Status.Warrior || MentorRole is Status.Deputy || MentorRole is Status.Leader))
                    || (AppRole is Status.MediatorApprentice && MentorRole is Status.Mediator)
                    || (AppRole is Status.MedicineCatApprentice && MentorRole is Status.MedicineCat)
            );



        public void AssignMentor(Cat NewApprentice)
        {
            if (MentorApprenticePairs.TryGetValue(NewApprentice, out List<CatRef> Apprentices))
            {

            }
        }
    }



    public sealed class PlayerClan : Clan
    {
        public PlayerClan() : base()
        {

        }

        /// <summary>
        /// Handle the death of a clan cat.
        /// </summary>
        /// <param name="BodyIsRecovered">Whether or not the body of the deceased was found</param>
        public new bool OnDeath(Cat Cat, bool BodyIsRecovered)
        {
            if (Cat == Leader)
            {
                if (LeaderLives > 0) --LeaderLives;
                Cat.Thought = LeaderLives == 0 ? "Thought.Death.Leader" : "Thought.Death.LeaderFinal";
            }
            else if (!Instructor.IsValid())
                Cat.Thought = "DeathThought.Purgatory";
            else if (StarclanOriented)
                Cat.Thought = "DeathThought.StarClan";
            else
                Cat.Thought = "DeathThought.DarkForest";

            if (MentorApprenticePairs.TryGetValue(Cat, out List<CatRef> Apprentices)) // If this cat was a mentor, update their apprentices
            {
                for (int i = 0; i < Apprentices.Count; i++)
                    AssignMentor(Apprentices[i]);

                MentorApprenticePairs.Remove(Cat);
            }

            if (ThisWorld.GameMode is not GamemodeType.Classic)
                Grief(BodyIsRecovered);

            SendToAfterlife(Cat);
            return true;
        }


        /// <summary>
        /// Grief a cats death.
        /// </summary>
        /// <param name="BodyIsRecovered">Whether or not the body of the deceased was found</param>
        public void Grief(bool BodyIsRecovered)
        {

        }


        public void Exile(Cat Cat)
        {

        }
    }
}
