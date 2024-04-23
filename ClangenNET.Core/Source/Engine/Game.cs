using ClangenNET.Graphics;
using ClangenNET.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static ClangenNET.Content;

namespace ClangenNET
{
    public class ClangenNetGame : Game
    {
        private static GraphicsDeviceManager? GraphicsDeviceManager;

        public ClangenNetGame()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 700,
            };

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            if (GraphicsDeviceManager is null)
                throw new Exception();

            PrepareContext(GraphicsDeviceManager);
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime GameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CurrentScene?.Update(GameTime); // FIX -> deal with delta time and ticks per second
            base.Update(GameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            ClangenNET.Content.Draw();
            base.Draw(gameTime);
        }

        protected override void BeginRun()
        {
            SetScene<LoadingScreen>();
            base.BeginRun();
        }
    }
}