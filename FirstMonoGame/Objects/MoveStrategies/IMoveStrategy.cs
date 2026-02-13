using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace FirstMonoGame.Objects.MoveStrategies;

internal interface IMoveStrategy
{
    Vector2 Move(Vector2 position, float speed, Rectangle roomBounds, GameTime gameTime);
}
