using System;
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
    private IMoveStrategy _moveStrategy;

    public Enemy(string name, int maxHealth, int maxShield, int startingShield, Vector2 position, AnimatedSprite sprite) : base(name, maxHealth, maxShield, startingShield, position, sprite)
    {
        
    }

    public Enemy(int maxHealth, Vector2 position, AnimatedSprite sprite) : base("Enemy", maxHealth, 0, 0, position, sprite)
    {
        
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        base.Update(gameTime, roomBounds);
    }

    public void ResetPosition(Vector2 newPosition)
    {
        _position = newPosition;
    }
}