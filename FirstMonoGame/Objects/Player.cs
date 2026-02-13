using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using Gum.Managers;

namespace FirstMonoGame.Objects;

public class Player
{
    private const float MOVEMENT_SPEED = 5.0f;

    public string Name { get; private set; }
    public Health Health { get; private set; }
    public Health Shield { get; private set; }
    private Vector2 _position;
    public Vector2 Position
    {
        get => _position;
    }
    public AnimatedSprite Sprite { get; private set; }
    public Circle Bounds {get; private set; }

    public Player(string name, int maxHealth, int maxShield, int startingShield, Vector2 position, AnimatedSprite sprite)
    {
        Name = name;
        Health = new Health(maxHealth);
        Shield = new Health(maxShield, startingShield);
        _position = position;
        Sprite = sprite;
    }

    public void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PlayerMovement(gameTime);

        // Creating a bounding circle for the slime.
        Bounds = new Circle(
            (int)(Position.X + (Sprite.Width * 0.5f)),
            (int)(Position.Y + (Sprite.Height * 0.5f)),
            (int)(Sprite.Width * 0.5f)
        );

        // Use distance based checks to determine if the slime is within the
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

        Sprite.Update(gameTime);
    }

    public void Draw()
    {
        Sprite.Draw(Core.SpriteBatch, Position);
    }

    private void PlayerMovement(GameTime gameTime)
    {
        KeyboardInfo keyboard = Core.Input.Keyboard;

        float speed = MOVEMENT_SPEED;
        Vector2 movementVector = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
        {
            movementVector.Y -= speed;
        }

        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
        {
            movementVector.Y += speed;
        }

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            movementVector.X -= speed;
        }

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            movementVector.X += speed;
        }

        // Ensuring that movement vector is always normalized

        if (movementVector != Vector2.Zero)
            movementVector = Vector2.Normalize(movementVector);
        movementVector *= speed;

        // Applying the movement vector to the slime

        _position = new Vector2(Position.X + movementVector.X, Position.Y + movementVector.Y);
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Damage cannot be less than 0!");
            Console.ForegroundColor = ConsoleColor.White;

            return;
        }

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
}