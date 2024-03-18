using ClangenNET.Graphics;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace ClangenNET
{
    internal static class Program
    {
        public static void Main(string[] Args)
        {
            L10N.LoadLanguages("Common\\Localisation\\Languages.yml");
            Console.WriteLine(L10N.SetLanguage("LEnglish"));
            Console.WriteLine(L10N.Get("Backstory.Kittypet3"));

            ClangenNetGame Game = null;
            
            try
            {
                Game = new();

                Runtime.Create(Game);

                Game.Run();
            }
            finally
            {
                Game?.Dispose();
            }
        }
    }

    public class ClangenNetGame : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private FontSystem Fonts;
        private SpriteBatchEx Batch;

        public ClangenNetGame()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef
            };

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
            Batch = new SpriteBatchEx(GraphicsDevice);

            Fonts = new FontSystem();
            //Fonts.CurrentAtlasFull += (e, a) => Fonts.Reset();
            //^ Faster performance with fonts, but causes sync errors every now and again - need to fix

            //Fonts.AddFont(File.ReadAllBytes("C:\\Users\\dnbow\\OneDrive\\Desktop\\ClangenNET\\ClangenNET.Core\\Common\\Fonts\\Clangen.ttf"));
            Fonts.AddFont(File.ReadAllBytes("C:\\Users\\dnbow\\OneDrive\\Desktop\\ClangenNET\\ClangenNET.Core\\Common\\Fonts\\NotoSans.ttf"));

            //foreach (FileInfo File in new DirectoryInfo(".\\Common\\Fonts").EnumerateFiles("*.ttf", SearchOption.AllDirectories))Fonts.AddFont(System.IO.File.ReadAllBytes(File.FullName));

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue );

            Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            //Runtime.GetScene().Draw(Batch);

            Batch.End();

            base.Draw(gameTime);
        }
    }
}