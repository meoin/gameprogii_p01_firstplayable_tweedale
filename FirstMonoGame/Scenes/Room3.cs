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
using FirstMonoGame.Objects.Pickups;

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

        _enemies.Add(new Spider(3, GetSpecificTile(4, 2), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(11, 3), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(19, 2), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(1, 8), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(8, 7), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(4, 15), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(8, 13), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(17, 14), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(21, 11), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(21, 26), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(11, 26), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(2, 21), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(11, 21), _spiderSprite, _player));
        _enemies.Add(new Spider(3, GetSpecificTile(17, 21), _spiderSprite, _player));

        _pickups.Add(new Gold(GetSpecificTile(5, 10), _goldSprite, 25));
        _pickups.Add(new Gold(GetSpecificTile(17, 10), _goldSprite, 25));
        _pickups.Add(new Shield(GetSpecificTile(14, 10), _shieldSprite, 1));
        _pickups.Add(new Shield(GetSpecificTile(22, 7), _shieldSprite, 1));
        _pickups.Add(new Shield(GetSpecificTile(20, 7), _shieldSprite, 1));

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
                    new Obstacle(_verticalWallSprite, GetSpecificTile(_tilemap.Columns - 1, 3))
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