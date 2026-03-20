using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GumRuntime;

namespace FirstMonoGame.Objects;

public class Player : Entity
{
    public static AnimatedSprite RootDeathSprite;
    public static AnimatedSprite RootWalkSprite;
    public static AnimatedSprite RootRollSprite;
    private const int ROLL_IFRAMES = 5;
    private const float MOVEMENT_SPEED = 300.0f;
    private AnimatedSprite _deathSprite;
    private AnimatedSprite _rollSprite;
    public Vector2 FacingDirection;
    private bool _facingLeft = false;
    public bool WeaponExtended { get; private set; } = false;
    private bool _dying = false;
    public bool Dead { get; private set; } = false;
    public bool Rolling { get; private set; } = false;
    public bool InIFrames { get; private set; } = false;
    public Weapon Weapon;
    public int Gold;

    public Player(string name, int maxHealth, int maxShield, int startingShield, Vector2 position)
     : base( name, maxHealth, maxShield, startingShield, position, RootWalkSprite)
    {
        Weapon = new Weapon(Position, 1, 2);
        _deathSprite = RootDeathSprite;
        _rollSprite = RootRollSprite;
        FacingDirection = new Vector2(Sprite.Width * 0.5f, 0);

        Weapon.SetDirection(GetCenter() + FacingDirection, Direction.Right);
    }

    public Player(int maxHealth, Vector2 position) : base("Player", maxHealth, DEFAULT_MAX_SHIELD, 0, position, RootWalkSprite)
    {
        Weapon = new Weapon(Position, 1, 2);
        _deathSprite = RootDeathSprite;
        _rollSprite = RootRollSprite;
        FacingDirection = new Vector2(Sprite.Width * 0.5f, 0);

        Weapon.SetDirection(GetCenter() + FacingDirection, Direction.Right);
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PreviousPosition = Position;
        if (!_dying && !_inKnockback) PlayerMovement(gameTime);
        else
        {
            _deathSprite.Update(gameTime);
            if (_deathSprite.OnLastFrame()) Dead = true;
        }

        if (Rolling) UpdateRoll(gameTime);

        if (WeaponExtended) Weapon.Update(gameTime);
        if (WeaponExtended && Weapon.Sprite.OnLastFrame()) WeaponExtended = false;

        base.Update(gameTime, roomBounds);
    }

    private void UpdateRoll(GameTime gameTime)
    {
        _rollSprite.Update(gameTime);

        InIFrames = _rollSprite.GetCurrentFrame() < ROLL_IFRAMES;

        if (!_rollSprite.OnLastFrame()) return;
        
        // If you are on the last frame of the roll:
        Rolling = false;
        _rollSprite.ResetAnimation();
    }

    public override void Draw()
    {
        if (WeaponExtended)
        {
            Weapon.Draw();
        }

        if (_facingLeft)
        {
            Sprite.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            _rollSprite.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
        }
        else
        {
            Sprite.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
            _rollSprite.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
        } 

        if (!_dying && !Rolling) base.Draw();
        else if (Rolling) _rollSprite.Draw(Core.SpriteBatch, Position);
        else _deathSprite.Draw(Core.SpriteBatch, Position);
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

        if (Rolling) _speedMultiplier = 1f;

        float speed = MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds * _speedMultiplier;
        Vector2 movementVector = Vector2.Zero;

        if (keyboard.WasKeyJustPressed(Keys.Space) && !WeaponExtended && !Rolling)
        {
            WeaponExtended = true;
            Weapon.Sprite.ResetAnimation();
        } 

        if (WeaponExtended) speed *= 0.5f;
        else if (keyboard.WasKeyJustPressed(Keys.LeftShift)) Rolling = true;

        if (Rolling) speed *= 1.5f;
        if (_rollSprite.GetCurrentFrame() >= 5) speed *= 0f;

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
        {
            movementVector.Y -= speed;
            FacingDirection = new Vector2(0, Sprite.Height * -0.5f);

            Weapon.SetDirection(GetCenter() + FacingDirection, Direction.Up);
        }

        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
        {
            movementVector.Y += speed;
            FacingDirection = new Vector2(0, Sprite.Height * 0.5f);

            Weapon.SetDirection(GetCenter() + FacingDirection, Direction.Down);
        }

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            movementVector.X -= speed;
            FacingDirection = new Vector2(Sprite.Width * -0.5f, 0);

            Weapon.SetDirection(GetCenter() + FacingDirection, Direction.Left);

            _facingLeft = true;
        }

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            movementVector.X += speed;
            FacingDirection = new Vector2(Sprite.Width * 0.5f, 0);

            Weapon.SetDirection(GetCenter() + FacingDirection, Direction.Right);

            _facingLeft = false;
        }

        // Ensuring that movement vector is always normalized

        if (movementVector != Vector2.Zero)
            movementVector = Vector2.Normalize(movementVector);
        movementVector *= speed;

        _lastMovementVector = movementVector;
        // Applying the movement vector to the slime

        _position = new Vector2(Position.X + movementVector.X, Position.Y + movementVector.Y);

        if (keyboard.CurrentState.GetPressedKeys().Length > 0) return true;
        else return false;
    }

    private void CheckGamepadInput(GameTime gameTime)
    {
        GamePadInfo gamepadOne = Core.Input.GamePads[(int)PlayerIndex.One];

        float speed = MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 movementVector = Vector2.Zero;

        if (gamepadOne.WasButtonJustPressed(Buttons.X) && !WeaponExtended)
        {
            WeaponExtended = true;
            Weapon.Sprite.ResetAnimation();
        }

        if (WeaponExtended) speed *= 0.5f;
        else if (gamepadOne.IsButtonDown(Buttons.A))
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

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }
}