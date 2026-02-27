using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FirstMonoGame.Objects;

public class Player : Entity
{
    public Sprite SwordSprite { get; private set; }
    private AnimatedSprite _deathSprite;
    private const float MOVEMENT_SPEED = 300.0f;
    public Vector2 FacingDirection = new Vector2(1, 0);
    private bool _facingLeft = false;
    public bool SwordExtended { get; private set; } = false;
    public Rectangle SwordHitbox { get; private set; }
    private float _swordRotation = 0f;
    private bool _dying = false;
    public bool Dead { get; private set; } = false;

    public Player(string name, int maxHealth, int maxShield, int startingShield, Vector2 position, AnimatedSprite sprite, Sprite swordSprite, AnimatedSprite deathSprite)
     : base( name, maxHealth, maxShield, startingShield, position, sprite)
    {
        SwordSprite = swordSprite;
        _deathSprite = deathSprite;
    }

    public Player(int maxHealth, Vector2 position, AnimatedSprite sprite, Sprite swordSprite, AnimatedSprite deathSprite) : base("Player", maxHealth, DEFAULT_MAX_SHIELD, 0, position, sprite)
    {
        SwordSprite = swordSprite;
        _deathSprite = deathSprite;
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PreviousPosition = Position;
        if (!_dying) PlayerMovement(gameTime);
        else
        {
            _deathSprite.Update(gameTime);
            if (_deathSprite.OnLastFrame()) Dead = true;
        }

        base.Update(gameTime, roomBounds);
    }

    public override void Draw()
    {
        if (SwordExtended)
        {
            Debug.WriteLine(FacingDirection);
            Vector2 swordPosition = GetCenter();
            SwordSprite.Rotation = MathHelper.ToRadians(_swordRotation);
            SwordSprite.Draw(Core.SpriteBatch, swordPosition + FacingDirection);
        }

        if (_facingLeft)
        {
            Sprite.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
        }
        else Sprite.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

        if (!_dying) base.Draw();
        else
        {
            _deathSprite.Draw(Core.SpriteBatch, Position);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (Health.CurrentHealth <= 0)
        {
            _dying = true;
        }
    }

    private void PlayerMovement(GameTime gameTime)
    {
        if (!GetKeyboardInput(gameTime)) CheckGamepadInput(gameTime);
    }

    private bool GetKeyboardInput(GameTime gameTime)
    {
        KeyboardInfo keyboard = Core.Input.Keyboard;

        float speed = MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 movementVector = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 0.5f;
            SwordExtended = true;
        }
        else
        {
            SwordExtended = false;
        }
        
        if (keyboard.IsKeyDown(Keys.LeftShift))
        {
            if (!SwordExtended) speed *= 1.5f;
        }

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
        {
            movementVector.Y -= speed;
            FacingDirection = new Vector2(0, Sprite.Height * -0.25f);
            _swordRotation = 270f;

            SwordHitbox = new Rectangle(
                (int)(Position.X - SwordSprite.Height * 0.5f),
                (int)(Position.Y - SwordSprite.Width),
                (int)SwordSprite.Height,
                (int)SwordSprite.Width
            );
        }

        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
        {
            movementVector.Y += speed;
            FacingDirection = new Vector2(0, Sprite.Height * 0.5f);
            _swordRotation = 90f;

            SwordHitbox = new Rectangle(
                (int)(Position.X - SwordSprite.Height * 0.5f),
                (int)(Position.Y + Sprite.Height),
                (int)SwordSprite.Height,
                (int)SwordSprite.Width
            );
        }

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            movementVector.X -= speed;
            FacingDirection = new Vector2(Sprite.Width * -0.5f, 0);
            _swordRotation = 180f;

            _facingLeft = true;

            SwordHitbox = new Rectangle(
                (int)(Position.X - SwordSprite.Width),
                (int)(Position.Y - SwordSprite.Height * 0.5f),
                (int)SwordSprite.Width,
                (int)SwordSprite.Height
            );
        }

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            movementVector.X += speed;
            FacingDirection = new Vector2(Sprite.Width * 0.5f, 0);
            _swordRotation = 0f;

            _facingLeft = false;

            SwordHitbox = new Rectangle(
                (int)(Position.X + Sprite.Width),
                (int)(Position.Y + Sprite.Height * 0.5f - SwordSprite.Height * 0.5f),
                (int)SwordSprite.Width,
                (int)SwordSprite.Height
            );
        }

        // Ensuring that movement vector is always normalized

        if (movementVector != Vector2.Zero)
            movementVector = Vector2.Normalize(movementVector);
        movementVector *= speed;

        _lastMovementVector = movementVector;
        // Applying the movement vector to the slime

        _position = new Vector2(Position.X + movementVector.X, Position.Y + movementVector.Y);

        if (movementVector != Vector2.Zero) return true;
        else return false;
    }

    private void CheckGamepadInput(GameTime gameTime)
    {
        GamePadInfo gamepadOne = Core.Input.GamePads[(int)PlayerIndex.One];

        float speed = MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 movementVector = Vector2.Zero;

        if (gamepadOne.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            gamepadOne.SetVibration(1.0f, TimeSpan.FromSeconds(1));
        }
        else
        {
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        }

        if (gamepadOne.LeftThumbStick != Vector2.Zero)
        {
            movementVector.X += gamepadOne.LeftThumbStick.X * speed;
            movementVector.Y -= gamepadOne.LeftThumbStick.Y * speed;
        }
        else
        {
            if (gamepadOne.IsButtonDown(Buttons.DPadUp))
            {
                movementVector.Y -= speed;
            }
            if (gamepadOne.IsButtonDown(Buttons.DPadDown))
            {
                movementVector.Y += speed;
            }
            if (gamepadOne.IsButtonDown(Buttons.DPadLeft))
            {
                movementVector.X -= speed;
            }
            if (gamepadOne.IsButtonDown(Buttons.DPadRight))
            {
                movementVector.X += speed;
            }
        }

        if (movementVector != Vector2.Zero)
            movementVector = Vector2.Normalize(movementVector);

        movementVector *= speed;
        _position = new Vector2(Position.X + movementVector.X, Position.Y + movementVector.Y);
    }
}