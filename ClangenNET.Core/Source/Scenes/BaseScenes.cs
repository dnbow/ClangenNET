using ClangenNET.Components;
using ClangenNET.Graphics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Threading;

using static ClangenNET.Content;

namespace ClangenNET.Scenes;

public sealed class LoadingScreen : IScene // TEMPORARY
{
    private static Texture2D? LoadingIcon;
    private static Font? Font;
    private static Thread? LoadingThread;
    private static bool IsFinalising = false;
    private static string Text = "Loading . . .";

    // NOTE -> Animation will be abstracted, just here for now as this
    private const int IconFrameMax = 10;
    private static int IconFrame = 0;
    private static double TotalSeconds = 0;

    public static void SetText(string Text)
    {
        LoadingScreen.Text = Text;
    }

    public LoadingScreen()
    {
        LoadingIcon = GetTexture("LoadingCat.png");
        Font = GetFont("Clangen", 40);
    }

    void IScene.Draw(SpriteBatchEx Batch)
    {
        Fill(new Color(206, 194, 168));
        Batch.Begin();
        Batch.Draw(LoadingIcon, new Rectangle(300, 250, 200, 200), new Rectangle(200 * (IconFrame % 5), 200 * (IconFrame / 5), 200, 200), Color.White);
        Font?.Draw(Batch, Text, new Vector2(400, 500), Color.White);
        Batch.End();
    }

    void IScene.Update(GameTime GameTime)
    {
        if (LoadingThread is not null && !LoadingThread.IsAlive)
        {
            if (!IsFinalising)
            {
                LoadingThread = new Thread(FinaliseContext);
                LoadingThread.Start();
                IsFinalising = true;
                SetText("Finalising . . .");
            }
            else
            {
                SetScene<Menu>();
                SetLanguage("LEnglish");
                Console.WriteLine(ParseString("I see that $(Summer) is upon us, [A] will work in [GameVersion]"));

                ThisWorld = new("Save.xml", Season.Spring, GamemodeType.Classic);
            }
        }

        if ((TotalSeconds += GameTime.ElapsedGameTime.TotalSeconds) > 0.1)
        {
            if (++IconFrame >= IconFrameMax) 
                IconFrame = 0;

            TotalSeconds = 0;
        }

        
    }

    void IScene.Open()
    {
        LoadingThread = new Thread(CreateContext);
        LoadingThread.Start();
    }
}

public sealed class TestCatRender : IScene
{
    private const int CatAmount = CatWidth * CatHeight;
    private const int CatWidth = 10;
    private const int CatHeight = 10;
    private const int SpaceWidth = 800 / CatWidth;
    private const int SpaceHeight = 700 / CatHeight;
    private readonly Cat[] Cats;
    private static Font? Font;
    private int Frames = 0;

    public TestCatRender()
    {
        ThisWorld ??= new(".", Season.Spring, GamemodeType.Classic);
        Font = GetFont("Clangen", 40);
        Cats = new Cat[CatAmount];

        MathEx.RandomEx RNG = new(2007);
        for (uint I = 0; I < CatAmount; I++)
        {
            Cats[I] = new Cat(RNG.Unsigned());
        }
    }

    void IScene.Draw(SpriteBatchEx Batch)
    {
        Batch.Begin(samplerState: SamplerState.PointClamp);
        Font?.Draw(Batch, Frames.ToString(), new Vector2(70, 30));

        for (int I = 0; I < CatAmount; I++)
            Cats[I].Draw(Batch, new(SpaceWidth * (I % CatWidth), SpaceHeight * (I / CatHeight), SpaceWidth, SpaceHeight));

        Batch.End();
    }

    void IScene.Update(GameTime GameTime)
    {
        Frames = (int)(1 / GameTime.ElapsedGameTime.TotalSeconds);
    }
}

public sealed class MenuButton(string Text, Rectangle Bounds) // TEMPORARY, working up an abstracting solution
{
    internal static Texture2D? Texture;
    internal static Effect? ButtonHover;
    internal static Font? Font;

    public readonly string Text = Text;
    public Rectangle Bounds = Bounds;

    public static void Begin(SpriteBatchEx Batch)
    {
        Batch.Begin(samplerState: SamplerState.PointClamp);
    }

    public static void BeginHovered(SpriteBatchEx Batch)
    {
        Batch.Begin(effect: ButtonHover, samplerState: SamplerState.PointClamp);
    }

    public void Draw(SpriteBatchEx Batch)
    {
        Batch.Draw(Texture, Bounds, Color.White);
        Font?.Draw(Batch, Text, Bounds, Color.White);
    }

    public void DrawHovered(SpriteBatchEx Batch) => Draw(Batch);
}

[SceneInfo("Menu", true)]
public sealed class Menu : IScene
{
    private readonly Texture2D Background;
    private readonly List<MenuButton> Buttons;
    private readonly NinePointBody NinePointBody;

    public Menu()
    {
        Background = GetTexture("Background.png");
        MenuButton.Texture = GetTexture("UI\\Buttons\\MenuButton.png");
        MenuButton.Font = GetFont("Clangen", 40);

        MenuButton.ButtonHover = GetShader("HSL");
        MenuButton.ButtonHover.Parameters["Luminosity"].SetValue(-0.2f);
        MenuButton.ButtonHover.Parameters["Saturation"].SetValue(0.8f);
        MenuButton.ButtonHover.Parameters["Hue"].SetValue(0);

        NinePointBody = new()
        {
            Texture = "UI\\Borders\\Frame1.png",

            TopLeft = new(0, 0, 10, 10),
            Top = new(10, 0, 980, 10),
            TopRight = new(990, 0, 10, 10),

            MiddleLeft = new(0, 10, 10, 980),
            Middle = new(10, 10, 980, 980),
            MiddleRight = new(990, 10, 10, 980),

            BottomLeft = new(0, 990, 10, 10),
            Bottom = new(10, 990, 980, 10),
            BottomRight = new(990, 990, 10, 10),
        };

        Buttons = [
            new (
                "Continue", 
                new Rectangle(70, 310, 192, 32)
            ),  
            new (
                "Switch Clan", 
                new Rectangle(70, 355, 192, 32)
            ),
            new (
                "New Clan",
                new Rectangle(70, 400, 192, 32)
            ),
            new (
                "Settings",
                new Rectangle(70, 445, 192, 32)
            ),
            new (
                "Content",
                new Rectangle(70, 490, 192, 32)
            ),
            new (
                "Quit", 
                new Rectangle(70, 535, 192, 32)
            )
        ];
    }

    void IScene.Update(GameTime GameTime)
    {
        NinePointBody.Bounds.Width = Mouse.GetState().Position.X - NinePointBody.Bounds.X;
        NinePointBody.Bounds.Height = Mouse.GetState().Position.Y - NinePointBody.Bounds.Y;
    }

    void IScene.Draw(SpriteBatchEx Batch)
    {
        DrawOld(Batch);
        //Batch.Begin();
        //NinePointBody.Draw(Batch);
        //Batch.End();
    }

    private void DrawOld(SpriteBatchEx Batch)
    {
        Batch.Begin();
        Batch.Draw(Background, Background.Bounds, Color.White);
        Batch.End();

        MenuButton.BeginHovered(Batch);
        foreach (MenuButton Button in Buttons)
        {
            if (Button.Bounds.Contains(Mouse.GetState().Position))
                Button.DrawHovered(Batch);
        }
        Batch.End();

        MenuButton.Begin(Batch);
        foreach (MenuButton Button in Buttons)
        {
            if (!Button.Bounds.Contains(Mouse.GetState().Position))
                Button.Draw(Batch);
        }
        
        Batch.End();
    }

}
