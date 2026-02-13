using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;

namespace FirstMonoGame.Objects;

public class Entity
{
    public const int DEFAULT_MAX_SHIELD = 5;
    private const float HURT_INVINCIBILITY_SECONDS = 2.5f;
    public bool InvincibleAfterBeingHurt = false;
    private float _hurtInvincibilityTimer = 0f;

    public string Name { get; private set; }
    public Health Health { get; private set; }
    public Health Shield { get; private set; }
    public Vector2 PreviousPosition { get; protected set; }
    protected Vector2 _position;
    public Vector2 Position
    {
        get => _position;
    }
    public AnimatedSprite Sprite { get; private set; }
    public Circle Bounds { get; private set; }

    public Entity(string name, int maxHealth, int maxShield, int startingShield, Vector2 position, AnimatedSprite sprite)
    {
        Name = name;
        Health = new Health(maxHealth);
        Shield = new Health(maxShield, startingShield);
        _position = position;
        Sprite = sprite;
    }

    public Entity(int maxHealth, Vector2 position, AnimatedSprite sprite)
    {
        Name = "UNNAMED_ENTITY";
        Health = new Health(maxHealth);
        Shield = new Health(DEFAULT_MAX_SHIELD, 0);
        _position = position;
        Sprite = sprite;
    }

    public virtual void Update(GameTime gameTime, Rectangle roomBounds)
    {
        if (InvincibleAfterBeingHurt)
        {
            _hurtInvincibilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_hurtInvincibilityTimer >= HURT_INVINCIBILITY_SECONDS)
            {
                InvincibleAfterBeingHurt = false;
                _hurtInvincibilityTimer = 0;
            }
        }

        RemainWithinRoomBounds(roomBounds);

        Sprite.Update(gameTime);
    }

    public virtual void Draw()
    {
        if (InvincibleAfterBeingHurt)
        {
            float hurtFlashWave = MathF.Sin(_hurtInvincibilityTimer * MathHelper.Pi * 5);
            if (hurtFlashWave > 0) Sprite.Draw(Core.SpriteBatch, Position, Color.Red);
            else Sprite.Draw(Core.SpriteBatch, Position);
        } 
        else Sprite.Draw(Core.SpriteBatch, Position);


    }

    private void RemainWithinRoomBounds(Rectangle roomBounds)
    {
        // Creating a bounding circle for the entity.
        Bounds = new Circle(
            (int)(Position.X + (Sprite.Width * 0.5f)),
            (int)(Position.Y + (Sprite.Height * 0.5f)),
            (int)(Sprite.Width * 0.5f)
        );

        // Use distance based checks to determine if the entity is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // move it back inside.
        if (Bounds.Left < roomBounds.Left)
        {
            _position.X = roomBounds.Left;
        }
        else if (Bounds.Right > roomBounds.Right)
        {
            _position.X = roomBounds.Right - Sprite.Width;
        }

        if (Bounds.Top < roomBounds.Top)
        {
            _position.Y = roomBounds.Top;
        }
        else if (Bounds.Bottom > roomBounds.Bottom)
        {
            _position.Y = roomBounds.Bottom - Sprite.Height;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (InvincibleAfterBeingHurt) return;

        if (damage < 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Damage cannot be less than 0!");
            Console.ForegroundColor = ConsoleColor.White;

            return;
        }


        InvincibleAfterBeingHurt = true;

        if (Shield.CurrentHealth > 0)
        {
            int spilloverDamage = Shield.TakeDamage(damage);

            if (spilloverDamage > 0)
            {
                Health.TakeDamage(spilloverDamage);
            }
        }
        else
        {
            Health.TakeDamage(damage);
        }
    }

    public Vector2 GetCenter()
    {
        return Position + new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
    }
}