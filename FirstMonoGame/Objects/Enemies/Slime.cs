using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using FirstMonoGame.Objects.MoveStrategies;

namespace FirstMonoGame.Objects.Enemies;

public class Slime : Enemy
{
    private const float MOVEMENT_SPEED = 100.0f;
    private const float TARGETING_RANGE = 700f;

    public Slime(int maxHealth, Vector2 position, AnimatedSprite sprite, Player player) : base("Slime", maxHealth, 0, 0, position, sprite)
    {
        _moveStrategy = new FollowPlayerMoveStrategy(player, this, TARGETING_RANGE);
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PreviousPosition = Position;

        _targetPosition = _moveStrategy.Move(_position, MOVEMENT_SPEED * _speedMultiplier, roomBounds, gameTime);

        base.Update(gameTime, roomBounds);
    }
}