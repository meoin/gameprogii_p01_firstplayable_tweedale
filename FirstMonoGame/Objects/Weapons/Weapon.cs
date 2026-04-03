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
    private const int DEFAULT_HIT_FRAMES = 4;
    private Vector2 _baseScale;
    private int _baseDamage;
    private int _baseKnockback;
    public static AnimatedSprite RootSlashSprite;
    public AnimatedSprite Sprite { get; private set; }
    public Rectangle Hitbox { get; private set; }
    public Vector2 Position {get; private set; }
    private float _rotation;
    public int Damage;
    public int Knockback;
    public bool InHitFrame;

    public Weapon(AnimatedSprite sprite, Vector2 source, int damage, int knockback)
    {
        Sprite = sprite;
        Position = source;
        SetDirection(source, Direction.Right);
        _rotation = 0f;
        Damage = damage;
        Knockback = knockback;
        _baseScale = Sprite.Scale;
        _baseDamage = damage;
        _baseKnockback = knockback;
    }

    public Weapon(Vector2 source, int damage, int knockback)
    {
        Sprite = RootSlashSprite;
        Position = source;
        SetDirection(source, Direction.Right);
        _rotation = 0f;
        Damage = damage;
        Knockback = knockback;
        _baseScale = Sprite.Scale;
        _baseDamage = damage;
        _baseKnockback = knockback;
    }

    public void Update(GameTime gameTime)
    {
        Sprite.Update(gameTime);
        InHitFrame = Sprite.GetCurrentFrame() < DEFAULT_HIT_FRAMES;
    }

    public void Draw()
    {
        Sprite.Rotation = MathHelper.ToRadians(_rotation);
        Sprite.Draw(Core.SpriteBatch, Position);
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

    public void SetScale(Vector2 scale)
    {
        Sprite.Scale = new Vector2(_baseScale.X * scale.X, _baseScale.Y * scale.Y);
    }

    public void SetDamageMultiplier(int multiplier)
    {
        Damage = _baseDamage * multiplier;
    }

    public void SetKnockbackMultiplier(int multiplier)
    {
        Knockback = _baseKnockback * multiplier;
    }

}