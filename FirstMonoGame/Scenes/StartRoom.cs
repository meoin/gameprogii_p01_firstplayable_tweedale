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

public class StartRoom : GameplayScene
{
    private const int TUTORIAL_X_POS = 300;
    private const int TUTORIAL_Y_POS = 30;
    private const int TUTORIAL_X_OFFSET = 20;
    private const string TUTORIAL_TEXT_MOVE = "WASD - Move";
    private Vector2 _tutorialMovePos;
    private Vector2 _tutorialMoveOrigin;
    private const string TUTORIAL_TEXT_ATTACK = "Spacebar - Attack";
    private Vector2 _tutorialAttackPos;
    private Vector2 _tutorialAttackOrigin;
    private const string TUTORIAL_TEXT_ROLL = "Shift - Roll";
    private Vector2 _tutorialRollPos;
    private Vector2 _tutorialRollOrigin;

    public StartRoom(string tilemapName) : base(tilemapName) { }
    public StartRoom(string tilemapName, Player player, Vector2 playerPosition) : base(tilemapName, player, playerPosition) { }

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        _enemies = new List<Enemy>();
        _obstacles = _tilemap.GetObstacles();
        _transitions = new List<RoomTransition>();

        _enemies.Add(new Slime(2, GetSpecificTile(14, 2), _slimeSprite, _player));

        Vector2 transitionDestination = new Vector2(_tilemap.TileWidth + 10, 10 * _tilemap.TileHeight);
        // Set level transition
        _transitions.Add
        (
            new RoomTransition
            (
                GetSpecificTile(_tilemap.Columns - 1, 2),
                (int)_tilemap.TileWidth,
                (int)_tilemap.TileHeight * 2,
                new Room1("room-1", _player, transitionDestination),
                transitionDestination,
                new List<Obstacle> 
                {
                    new Obstacle(_verticalWallSprite, GetSpecificTile(_tilemap.Columns - 1, 2)),
                    new Obstacle(_verticalWallSprite, GetSpecificTile(_tilemap.Columns - 1, 3))
                }
            )
        );

        // Tutorial info
        Vector2 size = _font.MeasureString(TUTORIAL_TEXT_MOVE);
        _tutorialMovePos = new Vector2(TUTORIAL_X_POS, TUTORIAL_Y_POS);
        _tutorialMoveOrigin = new Vector2(0, size.Y * 0.5f);

        size = _font.MeasureString(TUTORIAL_TEXT_ATTACK);
        _tutorialAttackPos = new Vector2(TUTORIAL_X_POS + TUTORIAL_X_OFFSET, _tutorialMovePos.Y + size.Y);
        _tutorialAttackOrigin = new Vector2(0, size.Y * 0.5f);

        size = _font.MeasureString(TUTORIAL_TEXT_ROLL);
        _tutorialRollPos = new Vector2(TUTORIAL_X_POS + (2*TUTORIAL_X_OFFSET), _tutorialAttackPos.Y + size.Y);
        _tutorialRollOrigin = new Vector2(0, size.Y * 0.5f);
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

        // Draw tutorial text

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        Core.SpriteBatch.DrawString(
            _font,
            TUTORIAL_TEXT_MOVE,
            _tutorialMovePos,
            Color.White,
            0.0f,
            _tutorialMoveOrigin,
            1.0f,
            SpriteEffects.None,
            0.0f
        );

        Core.SpriteBatch.DrawString(
            _font,
            TUTORIAL_TEXT_ATTACK,
            _tutorialAttackPos,
            Color.White,
            0.0f,
            _tutorialAttackOrigin,
            1.0f,
            SpriteEffects.None,
            0.0f
        );

        Core.SpriteBatch.DrawString(
            _font,
            TUTORIAL_TEXT_ROLL,
            _tutorialRollPos,
            Color.White,
            0.0f,
            _tutorialRollOrigin,
            1.0f,
            SpriteEffects.None,
            0.0f
        );

        Core.SpriteBatch.End();
    }
}