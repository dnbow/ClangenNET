using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Concurrent;


namespace ClangenNET.Graphics;


public static class ImageManager
{
    private static readonly ConcurrentDictionary<string, Texture2D> Cache = new ();

    public static Texture2D Get(GraphicsDevice Device, string Identifier)
    {
        return Cache.TryGetValue(Identifier, out Texture2D Existing) ? Existing : (Cache[Identifier] = Texture2D.FromFile(Device, Identifier));
    }
}
