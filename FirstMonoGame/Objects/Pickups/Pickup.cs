using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Obstacles;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FirstMonoGame.Objects.Pickups;

public class Pickup
{
    protected Vector2 _position;
    protected Sprite _sprite;
    public bool IsCollected { get; set; } = false;
    public Circle Bounds
    {
        get => new Circle
        (
            (int)(_position.X + (_sprite.Width * 0.5f)),
            (int)(_position.Y + (_sprite.Height * 0.5f)),
            (int)(_sprite.Width * 0.5f)
        );
    }

    public Pickup(Vector2 position, Sprite sprite)
    {
        _position = position - new Vector2(sprite.Width / 2, sprite.Height / 2);
        _sprite = sprite;
    }

    public void Draw()
    {
        _sprite.Draw(Core.SpriteBatch, _position);
    }

    public virtual void Collect(Player player)
    {
        IsCollected = true;
    }
}