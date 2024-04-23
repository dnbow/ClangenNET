using FontStashSharp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ClangenNET.Graphics;

public class Font // FIX -> just a clusterfuck
{
    private readonly FontSystem Collector;
    public readonly DynamicSpriteFont Face;


    public readonly string Path;


    public Font(string Path, uint Unit) 
    {
        this.Path = Path;

        Collector = new ();

        using (FileStream FontStream = File.OpenRead(Path))
        {
            Collector.AddFont(FontStream);
        }

        Face = Collector.GetFont(Unit);
    }

    public void Draw(SpriteBatchEx Batch, string Text, Rectangle Bounds, Color? Color = null)
    {
        Vector2 ActualSize = Face.MeasureString(Text);

        Face.DrawText(
            Batch, Text, Bounds.Center.ToVector2(), Color ?? Microsoft.Xna.Framework.Color.White, 0, ActualSize / 2, new Vector2(0.5f, 0.5f), 0, 0, 0, TextStyle.None, FontSystemEffect.None, 0
        );
    }

    public void Draw(SpriteBatchEx Batch, string Text, Vector2 Centre, Color? Color = null)
    {
        Vector2 ActualSize = Face.MeasureString(Text);
        Rectangle Bounds = new ((int)(Centre.X + (ActualSize.X / 2)), (int)(Centre.Y + (ActualSize.Y / 2)), (int)ActualSize.X, (int)ActualSize.Y);

        Face.DrawText(
            Batch, Text, new Vector2((int)(Centre.X - (ActualSize.X / 2)), (int)(Centre.Y - (ActualSize.Y / 2))), Color ?? Microsoft.Xna.Framework.Color.White
        );
    }
}

