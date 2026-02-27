using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary;

public class FollowCamera
{
    public Vector2 Position;
    public Rectangle Bounds;

    public FollowCamera(Rectangle bounds)
    {
        Position = Vector2.Zero;
        Bounds = bounds;
    }

    public void Follow(Vector2 target, Rectangle roomBounds)
    {
        Position = new Vector2(
            Math.Clamp(target.X, roomBounds.Left + Bounds.Width/2, roomBounds.Right - Bounds.Width / 2),
            Math.Clamp(target.Y, roomBounds.Top + Bounds.Height / 2, roomBounds.Bottom - Bounds.Height / 2)
        );
    }
}