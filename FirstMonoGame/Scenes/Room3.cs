using System;
using FirstMonoGame.UI;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using FirstMonoGame.Objects;
using FirstMonoGame.Objects.Enemies;
using System.Collections.Generic;
using System.Diagnostics;

namespace FirstMonoGame.Scenes;

public class Room3 : GameplayScene
{

    public Room3(string tilemapName, Player player, Vector2 playerPosition) : base(tilemapName, player, playerPosition) { }

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        _enemies = new List<Enemy>();
        _obstacles = _tilemap.GetObstacles();
        _transitions = new List<RoomTransition>();

        // _enemies.Add(new Spider(4, GetSpecificTile(12, 2), _spiderSprite, _player));
        // _enemies.Add(new Spider(4, GetSpecificTile(8, 7), _spiderSprite, _player));
        // _enemies.Add(new Spider(4, GetSpecificTile(15, 4), _spiderSprite, _player));
        // _enemies.Add(new Spider(4, GetSpecificTile(1, 1), _spiderSprite, _player));

        Vector2 transitionDestination = new Vector2(_tilemap.TileWidth + 10, 2 * _tilemap.TileHeight);
        // Set level transition
        _transitions.Add
        (
            new RoomTransition
            (
                GetSpecificTile(_tilemap.Columns - 1, 2),
                (int)_tilemap.TileWidth,
                (int)_tilemap.TileHeight * 2,
                new SpiderHallwayRoom("spider-hallway", _player, transitionDestination),
                transitionDestination,
                new List<Obstacle>
                {
                    new Obstacle(_verticalWallSprite, GetSpecificTile(_tilemap.Columns - 1, 2)),
                    new Obstacle(_verticalWallSprite, GetSpecificTile(_tilemap.Columns - 1, 2))
                }
            )
        );
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}