using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;

namespace FirstMonoGame.Objects;

public class Obstacle
{
    protected Vector2 _position;
    public Vector2 Position
    {
        get => _position;
    }
    public Sprite Sprite { get; private set; }
    public AnimatedSprite AnimatedSprite { get; private set; }
    private bool _isAnimated;
    public Rectangle Bounds { get; private set; }

    public Obstacle(Sprite sprite, Vector2 position)
    {
        _position = position;
        Sprite = sprite;
        _isAnimated = false;
    }

    public Obstacle(AnimatedSprite sprite, Vector2 position)
    {
        _position = position;
        Sprite = sprite;
        _isAnimated = true;
    }

    public virtual void Update(GameTime gameTime, Rectangle roomBounds)
    {
        if (_isAnimated) AnimatedSprite.Update(gameTime);
    }

    public virtual void Draw()
    {
        if (_isAnimated) AnimatedSprite.Draw(Core.SpriteBatch, Position);
        else Sprite.Draw(Core.SpriteBatch, Position);


    }
    
    public Vector2 GetCenter()
    {
        return Position + new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
    }
}