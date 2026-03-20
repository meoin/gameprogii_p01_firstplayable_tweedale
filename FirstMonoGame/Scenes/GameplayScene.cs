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
using FirstMonoGame.Objects.Pickups;
using System.Collections.Generic;
using System.Diagnostics;

namespace FirstMonoGame.Scenes;

public class GameplayScene : Scene
{

    #region Asset Definition

    protected AnimatedSprite _slimeSprite;
    protected AnimatedSprite _batSprite;
    protected AnimatedSprite _spiderSprite;
    protected AnimatedSprite _playerSprite;
    protected AnimatedSprite _playerDeathSprite;
    protected Sprite _swordSprite;
    protected Sprite _obstacle;
    protected Tilemap _tilemap;
    protected SpriteFont _font;
    protected Panel _pausePanel;
    protected AnimatedButton _resumeButton;
    protected SoundEffect _uiSoundEffect;
    protected TextureAtlas _atlas;
    protected TilemapAtlas _tilemapAtlas;
    protected string _tilemapName;
    protected SoundEffect _bounceSoundEffect;
    protected SoundEffect _collectSoundEffect;
    protected Sprite _goldSprite;
    protected Sprite _heartSprite;
    protected AnimatedSprite _slashSprite;

    #endregion

    #region Positional Value Definition

    // Defines the position to draw the score text at.
    protected Vector2 _scoreTextPosition;

    // Defines the position to draw the score text at.
    protected Vector2 _healthTextPosition;

    // Defines the origin used when drawing the score text.
    protected Vector2 _scoreTextOrigin;

    // Defines the origin used when drawing the score text.
    protected Vector2 _healthTextOrigin;

    // Defines the bounds of the room that the slime and bat are contained within.
    protected Rectangle _roomBounds;

    protected Rectangle _roomSize;

    protected Vector2 _playerPosition;

    #endregion

    #region Object Definition and Test Booleans
    protected Player _player;
    protected List<Enemy> _enemies;
    protected List<Obstacle> _obstacles;
    protected List<Vector2> _slimeSpawns;
    protected List<RoomTransition> _transitions;
    protected List<Pickup> _pickups;
    protected FollowCamera _camera;
    protected Matrix _translation;
    protected bool _showHitboxes = false;
    protected bool _pauseEnemiesForTesting = false;
    
    #endregion

    public GameplayScene(string tilemapName)
    {
        _tilemapName = tilemapName;
    }

    public GameplayScene(string tilemapName, Player player, Vector2 playerPosition)
    {
        _tilemapName = tilemapName;
        _player = player;
        _playerPosition = playerPosition;
    }

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;

        Rectangle screenBounds = Core.Bounds;

        _camera = new FollowCamera(Core.Bounds);

        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight,
            (int)(_tilemap.TileWidth * _tilemap.Columns - _tilemap.TileWidth * 2),
            (int)(_tilemap.TileHeight * _tilemap.Rows - _tilemap.TileHeight * 2)
         );

        _roomSize = new Rectangle(
            0,
            0,
            (int)_tilemap.TileWidth * _tilemap.Columns,
            (int)_tilemap.TileHeight * _tilemap.Rows
        );

        // Initial slime position will be the center tile of the tile map.
        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;
        Vector2 gameStartPosition = GetSpecificTile(1, 10);

        // Initialize the player
        _player ??= new Player(5, gameStartPosition);
        if(_playerPosition != Vector2.Zero) _player.SetPosition(_playerPosition);

        _enemies = new List<Enemy>();
        _obstacles = new List<Obstacle>();
        _transitions = new List<RoomTransition>();
        _pickups = new List<Pickup>();

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(Core.Bounds.Left, _tilemap.TileHeight * 0.5f);

        _healthTextPosition = new Vector2(Core.Bounds.Right, _tilemap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        float scoreTextYOrigin = _font.MeasureString("Gold").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

        // Set the origin of the text so it is right-centered.
        float healthTextXOrigin = _font.MeasureString("HP: 10").X;
        _healthTextOrigin = new Vector2(healthTextXOrigin, scoreTextYOrigin);

        InitializeUI();
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        // Create the player animated sprite and death sprite from the atlas.
        _playerSprite = _atlas.CreateAnimatedSprite("player-animation", 4.0f);
        _playerDeathSprite = _atlas.CreateAnimatedSprite("player-dying-animation", 4.0f);
        Player.RootWalkSprite ??= _atlas.CreateAnimatedSprite("player-animation", 4.0f);
        Player.RootDeathSprite ??= _atlas.CreateAnimatedSprite("player-dying-animation", 4.0f);
        Player.RootRollSprite ??= _atlas.CreateAnimatedSprite("player-roll-animation", 4.0f);

        _slashSprite = _atlas.CreateAnimatedSprite("slash-animation", 4.0f);
        _slashSprite.Origin = new Vector2(0, _slashSprite.Region.Height * 0.5f);
        Weapon.RootSlashSprite ??= _slashSprite;

        // Create the enemy animated sprites from the atlas.
        _slimeSprite = _atlas.CreateAnimatedSprite("slime-animation", 4.0f);
        _batSprite = _atlas.CreateAnimatedSprite("bat-animation", 4.0f);
        _spiderSprite = _atlas.CreateAnimatedSprite("spider-animation", 4.0f);

        // Create the tilemap from the XML configuration file.
        _tilemapAtlas = TilemapAtlas.FromFile(Content, "images/tilemap-definition.xml");
        _tilemapAtlas.Scale = new Vector2(4.0f, 4.0f);
        
        _tilemap = _tilemapAtlas.GetTilemap(_tilemapName);

        // Create the obstacle sprite from the atlas
        _obstacle = _atlas.CreateSprite("test-obstacle", 4.0f);

        // Load the bounce sound effect.
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect.
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the font.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");

        // Load the sound effect to play when ui actions occur.
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");

        _goldSprite = _atlas.CreateSprite("gold-1", 3.0f);
        _heartSprite = _atlas.CreateSprite("heart-1", 3.0f);
    }

    public override void Update(GameTime gameTime)
    {
        // Ensure the UI is always updated
        GumService.Default.Update(gameTime);

        // If the game is paused, do not continue
        if (_pausePanel.IsVisible)
        {
            return;
        }

        if (!_pauseEnemiesForTesting)
        {
            foreach (Enemy enemy in _enemies)
            {
                enemy.Update(gameTime, _roomBounds);
            }
        }

        foreach (Enemy enemy in _enemies)
        {
            enemy.ObstacleInteraction(_obstacles, _roomBounds);
        }

        // Check for keyboard input and handle it.
        CheckKeyboardInput();
        CheckGamepadInput();
        _player.Update(gameTime, _roomBounds);

        foreach(RoomTransition transition in _transitions)
        {
            transition.CheckIfPlayerEnter(_player);
        }

        _player.ObstacleInteraction(_obstacles, _roomBounds);

        // Loop through each enemy
        foreach (Enemy enemy in _enemies)
        {
            // Check if player is sticking their sword out
            if (_player.WeaponExtended && _player.Weapon.InHitFrame)
            {
                // If the enemy is touching the swords hitbox, set them to a new position and gain score
                if (enemy.Hitbox.Intersects(_player.Weapon.Hitbox))
                {
                    if (!enemy.InvincibleAfterBeingHurt) Core.Audio.PlaySoundEffect(_bounceSoundEffect);
                    enemy.TakeDamage(_player.Weapon.Damage, _player.Weapon.Knockback, _player.Weapon.Position);

                    
                    if (enemy.IsDead)
                    {
                        Random rand = new Random();
                        double pickupRoll = rand.NextDouble();
                        if (pickupRoll <= 0.8) _pickups.Add(new Gold(enemy.GetCenter(), _goldSprite, 10));
                        else _pickups.Add(new HeartPickup(enemy.GetCenter(), _heartSprite, 1));
                        continue;
                    }
                }
            }

            if (enemy.Hitbox.Intersects(_player.Hitbox) && !_player.InIFrames)
            {
                if (!_player.InvincibleAfterBeingHurt) Core.Audio.PlaySoundEffect(_bounceSoundEffect);

                _player.TakeDamage(1);
            }
        }

        foreach(Pickup pickup in _pickups)
        {
            // If the pickup isn't intersecting with the player then just skip to the next item in the list
            if (!pickup.Bounds.Intersects(_player.Bounds)) continue;

            Core.Audio.PlaySoundEffect(_collectSoundEffect);

            pickup.Collect(_player);

            pickup.IsCollected = true;
        }

        _enemies.RemoveAll(enemy => enemy.IsDead);
        _pickups.RemoveAll(pickup => pickup.IsCollected);

        _camera.Follow(_player.Position, _roomSize);
        CalculateTranslation(_camera);

        if (_player.Dead) Core.ChangeScene(new TitleScene());
    }

    protected Vector2 GetRandomTile()
    {
        Vector2 targetPosition = new Vector2(0, 0);

        while (true)
        {
            // Choose a random row and column based on the total number of each
            int column = Random.Shared.Next(1, _tilemap.Columns - 1);
            int row = Random.Shared.Next(1, _tilemap.Rows - 1);

            targetPosition = new Vector2(column * _tilemap.TileWidth, row * _tilemap.TileHeight);

            if (Vector2.Distance(targetPosition, _player.Position) > _player.Bounds.Radius * 5) break;
        }

        return targetPosition;
    }

    protected Vector2 GetSpecificTile(int x, int y)
    {
        // Choose a random row and column based on the total number of each
        int column = Math.Clamp(x, 1, _tilemap.Columns - 1);
        int row = Math.Clamp(y, 1, _tilemap.Columns - 1);

        return new Vector2(column * _tilemap.TileWidth, row * _tilemap.TileHeight);
    }

    protected void PauseGame()
    {
        // Make the pause panel UI element visible.
        _pausePanel.IsVisible = true;

        // Set the resume button to have focus
        _resumeButton.IsFocused = true;
    }

    protected void CheckKeyboardInput()
    {
        KeyboardInfo keyboard = Core.Input.Keyboard;

        // Exiting the level and returning to the title screen

        // If the escape key is pressed, pause the game.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            PauseGame();
            return;
        }

        if (keyboard.WasKeyJustPressed(Keys.P))
        {
            _pauseEnemiesForTesting = !_pauseEnemiesForTesting;
        }

        if (keyboard.WasKeyJustPressed(Keys.F))
        {
            Core.ToggleFullscreen();
        }

        if (keyboard.WasKeyJustPressed(Keys.T))
        {
            _showHitboxes = !_showHitboxes;
        }

        // Audio controls

        if (keyboard.WasKeyJustPressed(Keys.M))
        {
            Core.Audio.ToggleMute();
        }

        if (keyboard.WasKeyJustPressed(Keys.OemPlus))
        {
            Core.Audio.ChangeMasterVolume(0.1f);
        }

        if (keyboard.WasKeyJustPressed(Keys.OemMinus))
        {
            Core.Audio.ChangeMasterVolume(-0.1f);
        }

        if (keyboard.WasKeyJustPressed(Keys.D1))
        {
            Core.Audio.ChangeMusicVolume(-0.1f);
        }

        if (keyboard.WasKeyJustPressed(Keys.D2))
        {
            Core.Audio.ChangeMusicVolume(0.1f);
        }
    }

    protected void CheckGamepadInput()
    {
        GamePadInfo gamepadOne = Core.Input.GamePads[(int)PlayerIndex.One];

        Vector2 movementVector = Vector2.Zero;

        // If the start button is pressed, pause the game
        if (gamepadOne.WasButtonJustPressed(Buttons.Start))
        {
            PauseGame();
            return;
        }
    }

    protected void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();

        CreatePausePanel();
    }

    protected void CreatePausePanel()
    {
        _pausePanel = new Panel();
        _pausePanel.Anchor(Anchor.Center);
        _pausePanel.WidthUnits = DimensionUnitType.Absolute;
        _pausePanel.HeightUnits = DimensionUnitType.Absolute;
        _pausePanel.Height = 70;
        _pausePanel.Width = 264;
        _pausePanel.IsVisible = false;
        _pausePanel.AddToRoot();

        TextureRegion backgroundRegion = _atlas.GetRegion("panel-background");

        NineSliceRuntime background = new NineSliceRuntime();
        background.Dock(Dock.Fill);
        background.Texture = backgroundRegion.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.TextureHeight = backgroundRegion.Height;
        background.TextureLeft = backgroundRegion.SourceRectangle.Left;
        background.TextureTop = backgroundRegion.SourceRectangle.Top;
        background.TextureWidth = backgroundRegion.Width;
        _pausePanel.AddChild(background);

        TextRuntime textInstance = new TextRuntime();
        textInstance.Text = "PAUSED";
        textInstance.CustomFontFile = @"fonts/04b_30.fnt";
        textInstance.UseCustomFont = true;
        textInstance.FontScale = 0.5f;
        textInstance.X = 10f;
        textInstance.Y = 10f;
        _pausePanel.AddChild(textInstance);

        _resumeButton = new AnimatedButton(_atlas);
        _resumeButton.Text = "RESUME";
        _resumeButton.Anchor(Anchor.BottomLeft);
        _resumeButton.X = 9f;
        _resumeButton.Y = -9f;
        _resumeButton.Click += HandleResumeButtonClicked;
        _pausePanel.AddChild(_resumeButton);

        AnimatedButton quitButton = new AnimatedButton(_atlas);
        quitButton.Text = "QUIT";
        quitButton.Anchor(Anchor.BottomRight);
        quitButton.X = -9f;
        quitButton.Y = -9f;
        quitButton.Click += HandleQuitButtonClicked;

        _pausePanel.AddChild(quitButton);
    }

    protected void HandleResumeButtonClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Make the pause panel invisible to resume the game.
        _pausePanel.IsVisible = false;
        _resumeButton.IsFocused = false;
    }

    protected void HandleQuitButtonClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Go back to the title scene.
        Core.ChangeScene(new TitleScene());
    }

    protected void CalculateTranslation(FollowCamera camera)
    {
        var dx = (Core.Bounds.Width / 2) - camera.Position.X;
        var dy = (Core.Bounds.Height / 2) - camera.Position.Y;
        _translation = Matrix.CreateTranslation(dx, dy, 0);
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Core.BackgroundColor);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _translation);

        // Draw the tilemap
        _tilemap.Draw(Core.SpriteBatch);



        foreach (Obstacle obstacle in _obstacles)
        {
            obstacle.Draw();
        }

        foreach (Pickup pickup in _pickups) pickup.Draw();

        // Draw the player sprite.
        _player.Draw();

        foreach (Enemy enemy in _enemies)
        {
            enemy.Draw();
        }


        if (_showHitboxes)
        {
            foreach (Obstacle obstacle in _obstacles) Core.DrawRectangleOutline(obstacle.Bounds, Color.LimeGreen);
            foreach (Enemy enemy in _enemies)
            {
                Core.DrawRectangleOutline(enemy.Bounds, Color.Cyan);
                Core.DrawRectangleOutline(enemy.Hitbox, Color.Red);
            } 
            foreach (RoomTransition transition in _transitions) Core.DrawRectangleOutline(transition.Bounds, Color.Cyan);
            if (_player.WeaponExtended) Core.DrawRectangleOutline(_player.Weapon.Hitbox, Color.Red);
            Core.DrawRectangleOutline(_player.Bounds, Color.Cyan);
            Core.DrawRectangleOutline(_player.Hitbox, Color.Red);
            Core.DrawRectangleOutline(_roomBounds, Color.LimeGreen);
        }

        // Restart the spritebatch for UI elements to not use the translation matrix
        Core.SpriteBatch.End();
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the score.
        Core.SpriteBatch.DrawString(
            _font,              // spriteFont
            $"Gold: {_player.Gold}", // text
            _scoreTextPosition, // position
            Color.Red,        // color
            0.0f,               // rotation
            _scoreTextOrigin,   // origin
            1.0f,               // scale
            SpriteEffects.None, // effects
            0.0f                // layerDepth
        );

        Core.SpriteBatch.DrawString(
            _font,              // spriteFont
            $"HP: {_player.Health.CurrentHealth}", // text
            _healthTextPosition, // position
            Color.Red,        // color
            0.0f,               // rotation
            _healthTextOrigin,   // origin
            1.0f,               // scale
            SpriteEffects.None, // effects
            0.0f                // layerDepth
        );

        if (Core.ShowFPS)
        {
            float framerate = (float)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            var fps = string.Format("FPS: {0:F0}", framerate);

            Core.SpriteBatch.DrawString(
                _font,
                fps,
                new Vector2(0, 0),
                Color.Black,
                0.0f,
                new Vector2(0, 0),
                1.0f,
                SpriteEffects.None,
                0.0f
            );
        } 

        Core.SpriteBatch.End();

        // Draw the Gum UI
        GumService.Default.Draw();
    }
}