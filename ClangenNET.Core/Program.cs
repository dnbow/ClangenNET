using System;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static ClangenNET.Utility;

namespace ClangenNET.Core
{
    internal static class Program
    {
        public static void Main(string[] Args)
        {
            using (var Algorithm = SHA256.Create())
            {
                var Timer = new Stopwatch();

                Timer.Restart();
                byte[] Hash = ComputeHash(Algorithm, new DirectoryInfo("Common"), SearchOption: SearchOption.AllDirectories).GetAwaiter().GetResult();
                Timer.Stop();

                Console.WriteLine($"{string.Join("", Convert.ToHexString(Hash)).Replace("-", string.Empty)}.END\n1 Taken {Timer.ElapsedMilliseconds} ms");
            }

            L10N.LoadLanguages("Common\\Localisation\\Languages.yml");
            Console.WriteLine(L10N.SetLanguage("LEnglish"));
            Console.WriteLine(L10N.GetText("Backstory.Kittypet3"));

            using (ClangenNetGame Game = new ())
            {
                Game.Run();
            }
        }
    }

    public class ClangenNetGame : Game
    {
        private GraphicsDeviceManager Graphics;
        private SpriteBatch Batch;

        public ClangenNetGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Common";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            if (L10N.SetLanguage(Settings.Language) < 0)
                L10N.SetLanguage("LEnglish");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Batch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            Batch.End();

            base.Draw(gameTime);
        }
    }
}