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

public class Room1 : GameplayScene
{
    public Room1(string tilemapName) : base(tilemapName) {}
    public Room1(string tilemapName, Player player, Vector2 playerPosition) : base(tilemapName, player, playerPosition) { }

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        _enemies = new List<Enemy>();
        _obstacles = _tilemap.GetObstacles();
        _transitions = new List<RoomTransition>();

        _enemies.Add(new Slime(2, GetSpecificTile(8, 10), _slimeSprite, _player));
        _enemies.Add(new Slime(2, GetSpecificTile(2, 7), _slimeSprite, _player));
        _enemies.Add(new Slime(2, GetSpecificTile(6, 6), _slimeSprite, _player));
        _enemies.Add(new Slime(2, GetSpecificTile(13, 10), _slimeSprite, _player));

        _enemies.Add(new Spider(2, GetSpecificTile(16, 9), _spiderSprite, _player));
        _enemies.Add(new Spider(4, GetSpecificTile(3, 2), _spiderSprite, _player));
        _enemies.Add(new Spider(4, GetSpecificTile(11, 2), _spiderSprite, _player));

        _enemies.Add(new Bat(3, GetSpecificTile(15, 4), _batSprite));
        _enemies.Add(new Bat(3, GetSpecificTile(8, 7), _batSprite));

        _pickups.Add(new Shield(GetSpecificTile(15, 10), _shieldSprite, 1));


        Vector2 transitionDestination = new Vector2(_tilemap.TileWidth + 10, 4 * _tilemap.TileHeight);
        // Set level transition
        _transitions.Add
        (
            new RoomTransition
            (
                GetSpecificTile(_tilemap.Columns - 1, 5),
                (int)_tilemap.TileWidth,
                (int)_tilemap.TileHeight * 2,
                new Room2("room-2", _player, transitionDestination),
                transitionDestination,
                new List<Obstacle>
                {
                    new Obstacle(_verticalWallSprite, GetSpecificTile(_tilemap.Columns - 1, 5)),
                    new Obstacle(_verticalWallSprite, GetSpecificTile(_tilemap.Columns - 1, 6))
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