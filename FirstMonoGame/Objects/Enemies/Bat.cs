using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using FirstMonoGame.Objects.MoveStrategies;
using System.Diagnostics;

namespace FirstMonoGame.Objects.Enemies;

public class Bat : Enemy
{
    private const float MOVEMENT_SPEED = 300.0f;

    public Bat(int maxHealth, Vector2 position, AnimatedSprite sprite) : base("Bat", maxHealth, 0, 0, position, sprite)
    {
        _moveStrategy = new BouncingMoveStrategy(this);
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PreviousPosition = _position;

        _position = _moveStrategy.Move(_position, MOVEMENT_SPEED, roomBounds, gameTime);

        base.Update(gameTime, roomBounds);
    }
}