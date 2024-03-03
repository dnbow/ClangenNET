using System;
using System.Collections.Concurrent;

using static ClangenNET.Utility;
using static ClangenNET.Context;

namespace ClangenNET;

public static partial class Context
{
    private static readonly ConcurrentStack<IEvent> Events = new ();

    
    public static void AddEvent(IEvent Event)
    {
        Events.Push(Event);
    }


    internal static void HandleTimeskip()
    {
        for (int i = 0; i < Events.Count; i++)
        {
            if (!Events.TryPop(out IEvent CurrentEvent))
                break;
        }
    }
}

[Flags]
public enum EventScope : uint
{
    Custom = 0x0,
    Death = 0x1,
    Disaster = 0x2,
    Injury = 0x4,
    Relationship = 0x8,
}

public enum EventSeverity : byte
{
    Inconsequential,
    Individual,
    Minor, 
    Major,
    Global
}


public interface IEvent
{
    public EventScope Scope { get; }

    /// <summary>
    /// The severity of the event
    /// </summary>
    public EventSeverity Severity { get; }

    /// <summary>
    /// The translation key for any event text
    /// </summary>
    public TranslationKey Text { get; }

    /// <summary>
    /// Method to handle the event manually if EventType is Custom
    /// </summary>
    public virtual void Handle() => throw new NotImplementedException();
}



public readonly struct DeathEvent : IEvent
{
    public readonly EventScope Scope => EventScope.Death;
    public readonly EventSeverity Severity => EventSeverity.Individual;
    public TranslationKey Text { get; init; }

    public readonly CatRef Dead;

    public readonly CatRef[] Involved;

    public DeathEvent(TranslationKey Text, CatRef Dead, params CatRef[] Involved)
    {
        this.Text = Text;
        this.Dead = Dead;
        this.Involved = Involved;
    }
}
