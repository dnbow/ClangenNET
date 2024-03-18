using ClangenNET.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ClangenNET.Scenes
{
    [SceneInfo("Menu", true)]
    public sealed class Menu : IScene
    {
        private Texture2D Button;

        public Menu() 
        {
            Console.WriteLine("Created");
        }

        void IScene.Tick(uint Tick)
        {

        }

        void IScene.Draw(SpriteBatchEx Batch)
        {
            // Continue
            Batch.Draw(Button, new Rectangle(70, 310, 192, 35), Color.White);
            // Switch Clan
            Batch.Draw(Button, new Rectangle(70, 355, 192, 35), Color.White);
        }
    }
}
