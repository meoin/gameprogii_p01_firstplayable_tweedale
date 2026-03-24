using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Scenes;
using System.Collections.Generic;

namespace FirstMonoGame.Objects;

public class RoomTransition
{
    public Vector2 Position { get; private set; }
    public Rectangle Bounds { get; private set; }
    public Scene DestinationScene { get; private set; }
    public Vector2 DestinationPosition { get; private set; }
    public bool Open;
    public List<Obstacle> Walls;

    public RoomTransition(Vector2 position, int width, int height, Scene destScene, Vector2 destPosition, List<Obstacle> walls)
    {
        Position = position;
        Bounds = new Rectangle((int)position.X - 10, (int)position.Y - 10, width + 20, height + 20);
        DestinationScene = destScene;
        DestinationPosition = destPosition;
        Walls = walls;
    }

    public void CheckIfPlayerEnter(Player player)
    {
        if (!Open) return;

        if(player.Bounds.Intersects(Bounds))
        {
            Core.ChangeScene(DestinationScene);
        }
    }

    public void Draw()
    {
        if(Open) return;

        foreach(Obstacle wall in Walls)
        {
            wall.Draw();
        }
    }

}