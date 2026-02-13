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
    private IMoveStrategy _moveStrategy;

    public Slime(int maxHealth, Vector2 position, AnimatedSprite sprite, Player player) : base("Slime", maxHealth, 0, 0, position, sprite)
    {
        _moveStrategy = new FollowPlayerMoveStrategy(player, this);
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        PreviousPosition = Position;

        _position = _moveStrategy.Move(_position, MOVEMENT_SPEED, roomBounds, gameTime);

        base.Update(gameTime, roomBounds);
    }
}