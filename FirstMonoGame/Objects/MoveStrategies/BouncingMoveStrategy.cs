using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using FirstMonoGame.Objects.Enemies;
using System;
using System.Diagnostics;

namespace FirstMonoGame.Objects.MoveStrategies;

internal class BouncingMoveStrategy : IMoveStrategy
{
    private Vector2 _velocity = Vector2.Zero;
    private Enemy _enemy;

    public BouncingMoveStrategy(Enemy enemy)
    {
        _enemy = enemy;
    }

    public Vector2 Move(Vector2 position, float speed, Rectangle roomBounds, GameTime gameTime)
    {
        if (_velocity == Vector2.Zero) AssignRandomVelocity(speed);

        Vector2 movementVector = _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 newPosition = position + movementVector;

        Vector2 normal = Vector2.Zero;

        Circle bounds = new Circle
        (
           (int)(newPosition.X + (_enemy.Sprite.Width * 0.5f)),
           (int)(newPosition.Y + (_enemy.Sprite.Height * 0.5f)),
           (int)(_enemy.Sprite.Width * 0.5f)
        );

        // Use distance based checks to determine if the bat is within the
        // _enemy.Bounds of the game screen, and if it is outside that screen edge,
        // reflect it about the screen edge normal.
        if (bounds.Left < roomBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newPosition.X = roomBounds.Left;
        }
        else if (bounds.Right > roomBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newPosition.X = roomBounds.Right - _enemy.Sprite.Width;
        }

        if (bounds.Top < roomBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newPosition.Y = roomBounds.Top;
        }
        else if (bounds.Bottom > roomBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newPosition.Y = roomBounds.Bottom - _enemy.Sprite.Height;
        }

        // If the normal is anything but Vector2.Zero, this means the bat had
        // moved outside the screen edge so we should reflect it about the
        // normal.
        if (normal != Vector2.Zero)
        {
            normal.Normalize();
            _velocity = Vector2.Reflect(_velocity, normal);
        }

        //Debug.WriteLine($"Bat position: {newPosition} | Bat velocity: {_velocity} | Movement vector: {movementVector}");

        return newPosition;
    }

    private void AssignRandomVelocity(float speed)
    {
        // Generate a random angle.
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Convert angle to a direction vector.
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        direction = Vector2.Normalize(direction);

        // Multiply the direction vector by the movement speed.
        _velocity = direction * speed;
    }
}