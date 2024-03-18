using ClangenNET.Graphics;
using System;
using System.Collections.Generic;

namespace ClangenNET
{

    public static partial class Runtime
    {
        private static readonly Dictionary<Type, IScene> SceneInstances = new ();
        private static readonly HashSet<Type> SceneTypes = new ();

        /// <summary>
        /// Get the Current Scene.
        /// </summary>
        public static IScene CurrentScene { get; private set; }

        public static void SwitchToScene<TScene>() where TScene : class, IScene
        {
            if (CurrentScene.GetType() == typeof(TScene))
                return;

            CurrentScene.Close();
        }

        [InternalLoader(typeof(IScene))]
        static void LoadScene(Type Scene)
        {
            SceneTypes.Add(Scene);
        }
    }
}



namespace ClangenNET.Graphics
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SceneInfo : Attribute
    {
        public readonly string Id = null;
        public readonly bool Discoverable = true;

        public SceneInfo() { }
        public SceneInfo(string Id, bool Discoverable)
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
        internal void Tick(uint Tick);

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
}
