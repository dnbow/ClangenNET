using FontStashSharp;
using Microsoft.Xna.Framework;
using System.IO;

namespace ClangenNET.Graphics;

public class Font
{
    private readonly FontSystem Collector;
    private readonly DynamicSpriteFont Face;
    private readonly string Text;
    private readonly Vector2 Position;


    public readonly FileInfo Path;

    

    public Font(string Path, uint Unit) : this(new FileInfo(Path), Unit) { }

    public Font(FileInfo Path, uint Unit) 
    {
        Collector = new FontSystem();

        using (var FontStream = File.OpenRead(Path.FullName))
        {
            Collector.AddFont(FontStream);
        }

        Face = Collector.GetFont(Unit);
    }


    public void Draw(GameTime GameTime, SpriteBatchEx Batch)
    {
        Batch.DrawString(Face, Text, Position, null);
    }
}

