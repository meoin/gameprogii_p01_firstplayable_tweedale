using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using FirstMonoGame.Objects.MoveStrategies;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Obstacles;

namespace FirstMonoGame.Objects.Enemies;

public class Campfire : Obstacle
{
    public const int HEALING_RANGE = 250;
    private const float HEALING_INTERVAL = 1.5f;
    private Player _player;
    private float _healTimer = 0f;

    public Campfire(AnimatedSprite sprite, Vector2 position, Player player) : base(sprite, position)
    {
        _player = player;
    }

    public override void Update(GameTime gameTime, Rectangle roomBounds)
    {
        base.Update(gameTime, roomBounds);

        if (_healTimer > 0f)
        {
            _healTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            return;
        }

        float dist = Vector2.Distance(_player.Position, Position);
        if (dist > HEALING_RANGE) return;

        _player.Health.Heal(1);
        _healTimer = HEALING_INTERVAL;
    }

    
}