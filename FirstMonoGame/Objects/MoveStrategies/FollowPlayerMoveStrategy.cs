using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Diagnostics;

namespace FirstMonoGame.Objects.MoveStrategies;

internal class FollowPlayerMoveStrategy : IMoveStrategy
{
    private Player _player;
    private Entity _entity;
    public FollowPlayerMoveStrategy(Player player, Entity entity)
    {
        _player = player;
        _entity = entity;
    }

    public Vector2 Move(Vector2 position, float speed, Rectangle roomBounds, GameTime gameTime)
    {
        Vector2 movementDirection = _player.Position - position;

        if (movementDirection != Vector2.Zero)
        {
            movementDirection.Normalize();
        }

        Vector2 velocity = movementDirection * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _entity.SetLastMovementVector(velocity);

        Vector2 newPosition = position + velocity;

        // Debug.WriteLine($"Slime position: {newPosition} | Movement vector: {velocity}");

        return newPosition;
    }
}