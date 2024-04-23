using ClangenNET.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ClangenNET;



public static partial class Content
{
    private static readonly Dictionary<Type, IScene> SceneInstances = [];
    private static readonly HashSet<Type> SceneTypes = [];

    /// <summary>
    /// Get the Current Scene.
    /// </summary>
    public static IScene? CurrentScene { get; private set; }

    public static void SetScene<TScene>() where TScene : class, IScene
    {
        Type SceneType = typeof(TScene);

        if (CurrentScene is not null && CurrentScene.GetType() == SceneType)
            return;

        SceneTypes.Add(SceneType);

        if (SceneInstances.TryGetValue(SceneType, out IScene? Existing))
        {
            CurrentScene = Existing;
        }
        else
        {
            TScene? New;

            if ((New = Activator.CreateInstance(SceneType) as TScene) is not null)
            {
                CurrentScene?.Close();
                SceneInstances[SceneType] = CurrentScene = New;
            }
            else
            {
                Console.WriteLine($"Failed to create Scene of type \"{SceneType.FullName}\"");
                return;
            }
        }

        CurrentScene.Open();
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class SceneInfoAttribute : Attribute
{
    public readonly string? Id;
    public readonly bool Discoverable;

    public SceneInfoAttribute() { }
    public SceneInfoAttribute(string? Id = null, bool Discoverable = true)
    {
        this.Id = Id;
        this.Discoverable = Discoverable;
    }
}

public interface IScene
{
    /// <summary>
    /// Method to be called every game tick.
    /// </summary>
    internal void Update(GameTime GameTime);

    /// <summary>
    /// Method to be called when drawing the scene.
    /// </summary>
    internal void Draw(SpriteBatchEx Batch);

    /// <summary>
    /// Method to be called when switching to another scene.
    /// </summary>
    internal void Close() { }

    /// <summary>
    /// Method to be called when switching to this scene.
    /// </summary>
    internal void Open() { }
}