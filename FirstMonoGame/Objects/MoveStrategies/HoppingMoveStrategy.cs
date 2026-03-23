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

        if (_hopping) return HopTravel(position, speed, gameTime);

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

    private Vector2 HopTravel(GameTime gameTime)
    {
        _hopDistanceTravelled += (float)gameTime.ElapsedGameTime.TotalSeconds * _hopSpeed;
        _hopDistanceTravelled = MathF.Min(_hopDistanceTravelled, 1.0f);
        Vector2 newPosition = Vector2.SmoothStep(_hopOriginPoint, _target, _hopDistanceTravelled);

        if (_hopDistanceTravelled >= 1.0f)
        {
            _hopping = false;
            _hopTimer = _hopTimerMax;
            _hopDistanceTravelled = 0f;
        }

        return newPosition;
    }

    private Vector2 HopTravel(Vector2 position, float speed, GameTime gameTime)
    {
        Vector2 movementDirection = _target - position;

        if (movementDirection != Vector2.Zero) movementDirection.Normalize();

        Vector2 velocity = movementDirection * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _entity.SetLastMovementVector(velocity);

        Vector2 newPosition = position + velocity;

        _hopDistanceTravelled += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Vector2.Distance(position, _target) <= 10.0f || _hopDistanceTravelled >= 0.5f)
        {
            _hopping = false;
            _hopTimer = _hopTimerMax;
            _hopDistanceTravelled = 0f;
        }

        return newPosition;
    }

    public bool Moving()
    {
        return _hopping;
    }
}