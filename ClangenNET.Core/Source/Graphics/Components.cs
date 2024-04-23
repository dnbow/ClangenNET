using ClangenNET.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClangenNET.Components;

public interface IComponent
{
    void Draw(SpriteBatchEx Batch);
}

public interface IButtonComponent : IComponent
{
    bool IsVisible { get; set; }

    void DrawHovered(SpriteBatchEx Batch) => Draw(Batch);
    void DrawUnavailable(SpriteBatchEx Batch) => Draw(Batch);


}



public class Slider : IComponent
{
    public string LineTexture;
    public string PointerTexture;

    public required int Min;
    public required int Max;
    public int Step = 1;

    public int Value { get; protected set; }

    public void Draw(SpriteBatchEx Batch)
    {
        
    }
}


public class NinePointBody : IComponent
{
    public string Texture;

    public Rectangle TopLeft;
    public Rectangle Top;
    public Rectangle TopRight;
    public Rectangle MiddleLeft;
    public Rectangle Middle;
    public Rectangle MiddleRight;
    public Rectangle BottomLeft;
    public Rectangle Bottom;
    public Rectangle BottomRight;

    
    public Rectangle Bounds = new (200, 200, 500, 500);

    public void Draw(SpriteBatchEx Batch)
    {
        if (Bounds.Width - Middle.X < Middle.Width)
        {
            Content.DrawTexture(
                Texture,
                new(Bounds.X + Middle.X, Bounds.Y + Middle.Y, Bounds.Width - MiddleLeft.Width - MiddleRight.Width, Bounds.Height - Top.Height - Bottom.Height),
                new(Middle.X, Middle.Y, Middle.Width, Middle.Height),
                Color.White
            );
        }

        // Corners
        Content.DrawTexture(
            Texture, new(Bounds.X, Bounds.Y, TopLeft.Width, TopLeft.Height), TopLeft, Color.White
        );
        Content.DrawTexture(
            Texture, new(Bounds.X + Bounds.Width - TopRight.Width, Bounds.Y, TopRight.Width, TopRight.Height), TopRight, Color.White
        );
        Content.DrawTexture(
            Texture, new(Bounds.X, Bounds.Bottom - BottomLeft.Height , BottomLeft.Width, BottomLeft.Height), BottomLeft, Color.White
        );
        Content.DrawTexture(
            Texture, new(Bounds.Right - BottomRight.Width, Bounds.Bottom - BottomRight.Height, BottomRight.Width, BottomRight.Height), BottomRight, Color.White
        );

        // Sides
        if (Bounds.Width - MiddleLeft.X < MiddleLeft.Width)
        {
            Content.DrawTexture(
                Texture,
                new(Bounds.X + MiddleLeft.X, Bounds.Y + MiddleLeft.Y, (Bounds.Width - MiddleLeft.X), (Bounds.Height - MiddleLeft.Y)),
                new(MiddleLeft.X, MiddleLeft.Y, (Bounds.Width - MiddleLeft.X - MiddleLeft.Width), (Bounds.Height - MiddleLeft.Y - MiddleLeft.Height)),
                Color.White
            );
        }
       
    }
}