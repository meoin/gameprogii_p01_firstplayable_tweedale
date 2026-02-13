using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Diagnostics;

namespace FirstMonoGame.Objects.MoveStrategies;

internal class FollowPlayerMoveStrategy : IMoveStrategy
{
    private Player _player;
    public FollowPlayerMoveStrategy(Player player)
    {
        _player = player;
    }

    public Vector2 Move(Vector2 position, float speed, Rectangle roomBounds, GameTime gameTime)
    {
        Vector2 movementDirection = _player.Position - position;

        if (movementDirection != Vector2.Zero)
        {
            movementDirection.Normalize();
        }

        Vector2 velocity = movementDirection * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 newPosition = position + velocity;

        // Debug.WriteLine($"Slime position: {newPosition} | Movement vector: {velocity}");

        return newPosition;
    }
}