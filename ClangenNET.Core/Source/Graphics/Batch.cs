using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace ClangenNET.Graphics;

public class SpriteBatchEx : SpriteBatch
{
    private readonly Texture2D _Pixel;

    public SpriteBatchEx(GraphicsDevice GraphicsDevice) : base(GraphicsDevice)
    {
        _Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        _Pixel.SetData([Color.White]);
    }

    public void DrawLine(Vector2 Origin, float Length, float Angle, Color Color, float Thickness)
        => Draw(_Pixel, Origin, null, Color, Angle, Vector2.Zero, new Vector2(Length, Thickness), SpriteEffects.None, 0);
    public void DrawLine(Vector2 Origin, Vector2 End, Color Color, float Thickness)
        => DrawLine(Origin, Vector2.Distance(Origin, End), (float)Math.Atan2(End.Y - Origin.Y, End.X - Origin.X), Color, Thickness);

    public void DrawRectangle(Rectangle Rectangle, Color Color)
        => Draw(_Pixel, Rectangle, Color);
    public void DrawRectangle(Rectangle Rectangle, Color Color, float Angle)
        => Draw(_Pixel, Rectangle, null, Color, Angle, Vector2.Zero, SpriteEffects.None, 0);
    public void DrawRectangle(Vector2 Origin, Vector2 Size, Color Color, float Angle)
        => Draw(_Pixel, Origin, null, Color, Angle, Vector2.Zero, Size, SpriteEffects.None, 0);
}