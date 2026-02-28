using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace FirstMonoGame;

public enum Direction
{
    Right,
    Down,
    Left,
    Up
}

public class Weapon
{
    public Sprite Sprite { get; private set; }
    public Rectangle Hitbox { get; private set; }
    public Vector2 Position {get; private set; }
    private float _rotation;

    public Weapon(Sprite sprite, Vector2 source)
    {
        Sprite = sprite;
        Position = source;
        SetDirection(source, Direction.Right);
        _rotation = 0f;
    }

    public void SetDirection(Vector2 source, Direction direction)
    {
        Position = source;

        if (direction == Direction.Right)
        {
            _rotation = 0f;

            Hitbox = new Rectangle(
                (int)Position.X,
                (int)(Position.Y - Sprite.Height * 0.25f),
                (int)Sprite.Width,
                (int)(Sprite.Height * 0.5f)
            );
        }
        else if (direction == Direction.Down)
        {
            _rotation = 90f;

            Hitbox = new Rectangle(
                (int)(Position.X - Sprite.Height * 0.25f),
                (int)Position.Y,
                (int)(Sprite.Height * 0.5f),
                (int)Sprite.Width
            );
        }
        else if (direction == Direction.Left)
        {
            _rotation = 180f;

            Hitbox = new Rectangle(
                (int)(Position.X - Sprite.Width),
                (int)(Position.Y - Sprite.Height * 0.25f),
                (int)Sprite.Width,
                (int)(Sprite.Height * 0.5f)
            );
        }
        else if (direction == Direction.Up)
        {
            _rotation = 270f;

            Hitbox = new Rectangle(
                (int)(Position.X - Sprite.Height * 0.25f),
                (int)(Position.Y - Sprite.Width),
                (int)(Sprite.Height * 0.5f),
                (int)Sprite.Width
            );
        }
    }

    public void Draw()
    {
        Sprite.Rotation = MathHelper.ToRadians(_rotation);
        Sprite.Draw(Core.SpriteBatch, Position);
    }
}