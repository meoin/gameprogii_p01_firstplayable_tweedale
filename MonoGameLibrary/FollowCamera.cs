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
        int left = roomBounds.Left + Bounds.Width / 2;
        int right = roomBounds.Right - Bounds.Width / 2;
        int top = roomBounds.Top + Bounds.Height / 2;
        int bottom = roomBounds.Bottom - Bounds.Height / 2;

        float xPosition = 0;
        float yPosition = 0;

        if (left <= right) xPosition = Math.Clamp(target.X, left, right);
        else xPosition = roomBounds.Center.X;

        if (top <= bottom) yPosition = Math.Clamp(target.Y, top, bottom);
        else yPosition = roomBounds.Center.Y;

        Position = new Vector2(
            xPosition,
            yPosition
        );
    }
}