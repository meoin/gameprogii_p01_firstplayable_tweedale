using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Scenes;

namespace FirstMonoGame.Objects;

public class RoomTransition
{
    public Vector2 Position { get; private set; }
    public Rectangle Bounds { get; private set; }
    public Scene DestinationScene { get; private set; }
    public Vector2 DestinationPosition { get; private set; }

    public RoomTransition(Vector2 position, int width, int height, Scene destScene, Vector2 destPosition)
    {
        Position = position;
        Bounds = new Rectangle((int)position.X, (int)position.Y, width, height);
        DestinationScene = destScene;
        DestinationPosition = destPosition;
    }

    public void CheckIfPlayerEnter(Player player)
    {
        if(player.Bounds.Intersects(Bounds))
        {
            Core.ChangeScene(DestinationScene);
            player.SetPosition(DestinationPosition);
        }
    }

}