using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;

namespace FirstMonoGame.Objects;

public class Player : Entity
{
    private const float MOVEMENT_SPEED = 300.0f;
    public Vector2 FacingDirection = new Vector2(1, 0);

    public Player(string name, int maxHealth, int maxShield, int startingShield, Vector2 position, AnimatedSprite sprite) : base( name, maxHealth, maxShield, startingShield, position, sprite)
    {
        
    }

    public Player(int maxHealth, Vector2 position, AnimatedSprite sprite) : base("Player", maxHealth, DEFAULT_MAX_SHIELD, 0, position, sprite)
    {
        
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PreviousPosition = Position;

        PlayerMovement(gameTime);

        base.Update(gameTime, roomBounds);
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

        if (keyboard.IsKeyDown(Keys.LeftShift))
        {
            speed *= 1.5f;
        }

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
        {
            movementVector.Y -= speed;
            FacingDirection = new Vector2(0, 1);
        }

        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
        {
            movementVector.Y += speed;
            FacingDirection = new Vector2(0, -1);
        }

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            movementVector.X -= speed;
            FacingDirection = new Vector2(-1, 0);
        }

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            movementVector.X += speed;
            FacingDirection = new Vector2(1, 0);
        }

        // Ensuring that movement vector is always normalized

        if (movementVector != Vector2.Zero)
            movementVector = Vector2.Normalize(movementVector);
        movementVector *= speed;

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