using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using FirstMonoGame.Objects.MoveStrategies;

namespace FirstMonoGame.Objects.Enemies;

public class Enemy : Entity
{
    private const float MOVEMENT_SPEED = 300.0f;
    protected IMoveStrategy _moveStrategy;
    protected Vector2 _targetPosition;

    public Enemy(string name, int maxHealth, int maxShield, int startingShield, Vector2 position, AnimatedSprite sprite) : base(name, maxHealth, maxShield, startingShield, position, sprite)
    {
        
    }

    public Enemy(int maxHealth, Vector2 position, AnimatedSprite sprite) : base("Enemy", maxHealth, 0, 0, position, sprite)
    {
        
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PreviousPosition = Position;

        if (!_inKnockback) _position = _targetPosition;

        base.Update(gameTime, roomBounds);
    }

    public void EntityInteraction(List<Enemy> enemies, Rectangle roomBounds)
    {
        if (this is Bat) return;

        foreach (Entity entity in enemies)
        {
            if (entity == this || entity is Bat) continue;
            if (!Hitbox.Intersects(entity.Hitbox)) continue;

            BlockMovement(entity.Hitbox.ToRectangle(), roomBounds);
        }
    }

    public void ResetPosition(Vector2 newPosition)
    {
        _position = newPosition;
    }

    public override void TakeDamage(int damage, int knockback, Vector2 source)
    {
        base.TakeDamage(damage, knockback, source);

        if (Health.CurrentHealth <= 0) Die();
    }

    private void Die()
    {
        IsDead = true;
    }
}