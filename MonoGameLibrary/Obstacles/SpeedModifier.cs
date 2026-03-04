using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Obstacles;

public class SpeedModifier : Obstacle
{
    public float SpeedChange;
    public SpeedModifier(Sprite sprite, Vector2 position, float speedChange) : base(sprite, position)
    {
        SpeedChange = speedChange;
    }
}