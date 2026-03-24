using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Obstacles;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using FirstMonoGame.Objects.Enemies;

namespace FirstMonoGame.Objects;

public class Entity
{
    public const int DEFAULT_MAX_SHIELD = 1;
    private const int DEFAULT_KNOCKBACK = 1;
    private const float HURT_INVINCIBILITY_SECONDS = 1.5f;
    public bool InvincibleAfterBeingHurt = false;
    private float _hurtInvincibilityTimer = 0f;
    protected float _speedMultiplier = 1f;
    private float _floorDamageTimerMax = 5f;
    private float _floorDamageTimer = 0f;
    private bool _inDamagingFloor = false;
    public string Name { get; private set; }
    public Health Health { get; private set; }
    public Health Shield { get; private set; }
    public bool IsDead { get; protected set; } = false;
    public Vector2 PreviousPosition { get; protected set; }
    protected Vector2 _position;
    protected bool _animateSprite = true;
    protected bool _inKnockback = false;
    protected Vector2 _knockbackVector;
    protected float _knockbackTimer;
    public Vector2 Position
    {
        get => _position;
    }
    public AnimatedSprite Sprite { get; private set; }
    public Circle Hitbox 
    { 
        get => new Circle
        (
            (int)(Position.X + (Sprite.Width * 0.5f)),
            (int)(Position.Y + (Sprite.Height * 0.5f)),
            (int)(Sprite.Width * 0.3f)
        );
    }
    public Circle Bounds
    {
        get => new Circle
        (
            (int)(Position.X + (Sprite.Width * 0.5f)),
            (int)(Position.Y + (Sprite.Height * 0.5f)),
            (int)(Sprite.Width * 0.5f)
        );
    }

    protected Vector2 _lastMovementVector = new Vector2(0, 0);

    public void SetLastMovementVector(Vector2 lastVector)
    {
        _lastMovementVector = lastVector;
    }

    public Entity(string name, int maxHealth, int maxShield, int startingShield, Vector2 position, AnimatedSprite sprite)
    {
        Name = name;
        Health = new Health(maxHealth);
        Shield = new Health(maxShield, startingShield);
        _position = position;
        Sprite = new AnimatedSprite(sprite);
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
        else if (_inDamagingFloor)
        {
            _floorDamageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_floorDamageTimer <= 0)
            {
                TakeDamage(1);
                _inDamagingFloor = false;
            }
        }

        if (_inKnockback) ApplyKnockback(gameTime);

        RemainWithinRoomBounds(roomBounds);

        if (_animateSprite) Sprite.Update(gameTime);
    }

    public virtual void Draw()
    {
        if (InvincibleAfterBeingHurt)
        {
            float hurtFlashWave = MathF.Sin(_hurtInvincibilityTimer * MathHelper.Pi * 5);
            if (hurtFlashWave > 0) Sprite.Draw(Core.SpriteBatch, Position, Color.Red);
            else Sprite.Draw(Core.SpriteBatch, Position);
        } 
        else if (_inDamagingFloor)
        {
            int redness = (int)(255 * (_floorDamageTimer / _floorDamageTimerMax));
            Color drawColor = new Color(255, redness, redness);
            Sprite.Draw(Core.SpriteBatch, Position, drawColor);
        }
        else Sprite.Draw(Core.SpriteBatch, Position);
    }

    private void RemainWithinRoomBounds(Rectangle roomBounds)
    {
        float boundsMargin = (float)(Sprite.Width * 0.5) - (float)Bounds.Radius;

        // Use distance based checks to determine if the entity is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // move it back inside.
        if (Bounds.Left < roomBounds.Left)
        {
            _position.X = roomBounds.Left - boundsMargin;
        }
        else if (Bounds.Right > roomBounds.Right)
        {
            _position.X = roomBounds.Right - Sprite.Width + boundsMargin;
        }

        if (Bounds.Top < roomBounds.Top)
        {
            _position.Y = roomBounds.Top - boundsMargin;
        }
        else if (Bounds.Bottom > roomBounds.Bottom)
        {
            _position.Y = roomBounds.Bottom - Sprite.Height + boundsMargin;
        }
    }

    protected void ApplyKnockback(GameTime gameTime)
    {
        float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds * 2;

        float knockbackX = _knockbackVector.X * frameTime;
        float knockbackY = _knockbackVector.Y * frameTime;

        _position = new Vector2(_position.X + knockbackX, _position.Y + knockbackY);

        _knockbackVector = new Vector2(_knockbackVector.X - knockbackX, _knockbackVector.Y - knockbackY);

        _knockbackTimer += frameTime;

        if (_knockbackTimer >= 1f) _inKnockback = false;
    }

    public virtual void TakeDamage(int damage, Vector2 source)
    {
        int knockback = DEFAULT_KNOCKBACK;

        if (InvincibleAfterBeingHurt) return;

        TakeDamage(damage, knockback, source);
    }

    public virtual void TakeDamage(int damage, int knockback, Vector2 source)
    {
        if (InvincibleAfterBeingHurt) return;

        _knockbackVector = new Vector2((_position.X - source.X) * knockback, (_position.Y - source.Y) * knockback);
        _knockbackTimer = 0f;
        _inKnockback = true;
        
        TakeDamage(damage);
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

    public virtual void EntityInteraction(List<Entity> entities, Rectangle roomBounds)
    {
        if (this is Bat) return;
        
        foreach (Entity entity in entities)
        {
            if (entity == this || entity is Bat) continue;
            if (!Hitbox.Intersects(entity.Hitbox)) continue;

            BlockMovement(entity.Hitbox.ToRectangle(), roomBounds);
        }
    }

    public virtual void ObstacleInteraction(List<Obstacle> obstacles, Rectangle roomBounds)
    {
        _speedMultiplier = 1f;
        bool startDamagingFloorTimer = false;
        bool inDamagingFloor = false;
        float newFloorDamageTime = 0f;

        // Check each obstacle
        foreach (Obstacle obstacle in obstacles)
        {
            // If entity is colliding with the obstacle, revert them to their previous position
            if (!Bounds.Intersects(obstacle.Bounds)) continue;

            if (obstacle is Wall)
            {
                BlockMovement(obstacle.Bounds, roomBounds);
            }
            else if (obstacle is SpeedModifier speedModifier)
            {
                _speedMultiplier = speedModifier.SpeedChange;
            }
            else if (obstacle is DamagingFloor damagingFloor)
            {
                inDamagingFloor = true;

                if (!_inDamagingFloor && !InvincibleAfterBeingHurt)
                {
                    startDamagingFloorTimer = true;
                    newFloorDamageTime = damagingFloor.DamageTimer;
                } 
            }
        }

        if (!inDamagingFloor) _inDamagingFloor = false;

        if (!startDamagingFloorTimer) return;

        _inDamagingFloor = true;
        _floorDamageTimer = newFloorDamageTime;
        _floorDamageTimerMax = newFloorDamageTime;
    }

    protected void BlockMovement(Rectangle obstacle, Rectangle roomBounds)
    {
        _position = PreviousPosition;

        // Try moving the entity in the X axis that they were trying to move without the Y axis
        _position += new Vector2(_lastMovementVector.X, 0);

        if (!Bounds.Intersects(obstacle))
        {
            // Ensure entity is still remaining within the bounds of the room
            RemainWithinRoomBounds(roomBounds);
            return;
        }
        else
        {
            // If moving horizontally still collides then
            // Revert back to previous position and then try moving the entity in the Y axis that they were trying to move without the X axis
            _position = PreviousPosition;
            _position += new Vector2(0, _lastMovementVector.Y);

            if (!Bounds.Intersects(obstacle))
            {
                RemainWithinRoomBounds(roomBounds);
                return;
            }
            // If both directions lead to a collision then just keep them at their original position
            else _position = PreviousPosition;
        }
    }
}