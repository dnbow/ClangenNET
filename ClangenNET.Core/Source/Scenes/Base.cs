using ClangenNET.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ClangenNET.Scenes;

public abstract class BaseElement
{
    public bool Visible { get; set; }

    public abstract void Draw(SpriteBatchEx Batch);
    public virtual void DrawHovered(SpriteBatchEx Batch) => Draw(Batch);
}



public abstract class BaseScene : IScene
{
    protected readonly List<BaseElement> Elements;
    protected readonly List<BaseElement> HoveredElements;
    protected BaseElement? LastElement;

    public BaseScene()
    {
        Elements = [];
        HoveredElements = [];
    }

    void IScene.Draw(SpriteBatchEx Batch)
    {
        Batch.Begin();

        for (int I = 0; I < Elements.Count; I++)
        {
            LastElement = Elements[I];

            if (!LastElement.Visible)
                continue;

            LastElement.Draw(Batch);
        }

        Batch.End();
    }

    void IScene.Update(GameTime GameTime)
    {

    }
}
