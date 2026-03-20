using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite
{
    private int _currentFrame;
    private TimeSpan _elapsed;
    private Animation _animation;

    /// <summary>
    /// Gets or Sets the animation for this animated sprite.
    /// </summary>
    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }

    /// <summary>
    /// Creates a new animated sprite.
    /// </summary>
    public AnimatedSprite() { }

    /// <summary>
    /// Creates a new animated sprite with the specified frames and delay.
    /// </summary>
    /// <param name="animation">The animation for this animated sprite.</param>
    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }

    public AnimatedSprite(AnimatedSprite sprite)
    {
        Animation = sprite.Animation;
        Scale = sprite.Scale;
    }

    /// <summary>
    /// Sets the delay of this sprites animation in milliseconds.
    /// </summary>
    /// <param name="milliseconds">The delay in milliseconds to set for the animation.</param>
    public void SetDelay(int milliseconds)
    {
        _animation.Delay = TimeSpan.FromMilliseconds(milliseconds);
    }

    /// <summary>
    /// Updates this animated sprite.
    /// </summary>
    /// <param name="gameTime">A snapshot of the game timing values provided by the framework.</param>
    public void Update(GameTime gameTime)
    {
        _elapsed += gameTime.ElapsedGameTime;

        if (_elapsed >= _animation.Delay)
        {
            _elapsed -= _animation.Delay;
            _currentFrame++;

            if (_currentFrame >= _animation.Frames.Count)
            {
                _currentFrame = 0;
            }

            Region = _animation.Frames[_currentFrame];
        }
    }

    public bool OnLastFrame()
    {
        return _currentFrame >= _animation.Frames.Count - 1;
    }

    public void ResetAnimation()
    {
        _currentFrame = 0;
        _elapsed = TimeSpan.Zero;
    }

    public int GetCurrentFrame()
    {
        return _currentFrame;
    }
}