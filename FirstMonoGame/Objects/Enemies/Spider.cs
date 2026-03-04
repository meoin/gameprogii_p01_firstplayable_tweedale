using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using FirstMonoGame.Objects.MoveStrategies;

namespace FirstMonoGame.Objects.Enemies;

public class Spider : Enemy
{
    private const float MOVEMENT_SPEED = 100.0f;

    public Spider(int maxHealth, Vector2 position, AnimatedSprite sprite, Player player) : base("Slime", maxHealth, 0, 0, position, sprite)
    {
        _moveStrategy = new HoppingMoveStrategy(player, this, 1.5f, 300f, 2f);
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PreviousPosition = Position;

        _position = _moveStrategy.Move(_position, MOVEMENT_SPEED, roomBounds, gameTime);

        _animateSprite = _moveStrategy.Moving();

        base.Update(gameTime, roomBounds);
    }
}