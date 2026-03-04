using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Obstacles;

public class DamagingFloor : Obstacle
{
    public float DamageTimer;
    public DamagingFloor(Sprite sprite, Vector2 position, float damageTimer) : base(sprite, position)
    {
        DamageTimer = damageTimer;
    }
}