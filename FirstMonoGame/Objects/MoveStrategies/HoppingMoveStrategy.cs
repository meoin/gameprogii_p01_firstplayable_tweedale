using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;
using System.Diagnostics;

namespace FirstMonoGame.Objects.MoveStrategies;

internal class HoppingMoveStrategy : IMoveStrategy
{
    private Player _player;
    private Entity _entity;
    private float _hopTimer;
    private float _hopTimerMax;
    private float _aggroRange;
    private bool _hopping = false;
    private float _hopDistanceTravelled;
    private Vector2 _hopOriginPoint;
    private float _hopSpeed;
    private Vector2 _target;
    public HoppingMoveStrategy(Player player, Entity entity, float hopTimer, float aggroRange, float hopSpeed)
    {
        _player = player;
        _entity = entity;
        _hopTimer = hopTimer;
        _hopTimerMax = hopTimer;
        _aggroRange = aggroRange;
        _hopSpeed = hopSpeed;
    }

    public Vector2 Move(Vector2 position, float speed, Rectangle roomBounds, GameTime gameTime)
    {
        Vector2 newPosition = position;

        if (_hopping)
        {
            _hopDistanceTravelled += (float)gameTime.ElapsedGameTime.TotalSeconds * _hopSpeed;
            _hopDistanceTravelled = MathF.Min(_hopDistanceTravelled, 1.0f);
            newPosition = Vector2.SmoothStep(_hopOriginPoint, _target, _hopDistanceTravelled);

            if (_hopDistanceTravelled >= 1.0f) 
            {
                _hopping = false;
                _hopTimer = _hopTimerMax;
                _hopDistanceTravelled = 0f;
            }

            return newPosition;
        }

        _hopTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_hopTimer > 0) return newPosition;

        _hopping = true;

        _hopOriginPoint = position;

        if (_aggroRange > Vector2.Distance(_player.Position, position))
        {
            _target = _player.Position;
        }
        else
        {
            Random rand = new Random();
            float randomXDistance = (float)(rand.NextDouble() * (_aggroRange * 2) - _aggroRange) * 0.8f;
            float randomYDistance = (float)(rand.NextDouble() * (_aggroRange * 2) - _aggroRange) * 0.8f;
            _target = new Vector2(position.X + randomXDistance, position.Y + randomYDistance);
        }

        return newPosition;
    }

    public bool Moving()
    {
        return _hopping;
    }
}