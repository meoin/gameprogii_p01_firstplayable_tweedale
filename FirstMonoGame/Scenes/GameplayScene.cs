using System;
using FirstMonoGame.UI;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Xml;
using System.Xml.Linq;
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
using System.Linq;

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
    protected Sprite _shieldSprite;
    protected AnimatedSprite _slashSprite;
    protected Sprite _verticalWallSprite;
    protected Sprite _horizontalWallSprite;
    protected Sprite _heartContainerEmpty;
    protected Sprite _heartContainerFull;
    protected Sprite _shieldContainer;
    protected AnimatedSprite _campfireSprite;

    #endregion

    #region Positional Value Definition

    // Defines the position to draw the score text at.
    protected Vector2 _scoreTextPosition;

    // Defines the position to draw the score text at.
    protected Vector2 _heartContainerStartPosition;
    protected Vector2 _shieldContainerStartPosition;
    protected float _heartContainerSpacing;

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
    protected List<Campfire> _campfires;
    protected List<RoomTransition> _transitions;
    protected List<Pickup> _pickups;
    protected FollowCamera _camera;
    protected Matrix _translation;
    protected bool _roomComplete = false;
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

    public GameplayScene(string tilemapName, Player player)
    {
        _tilemapName = tilemapName;
        _player = player;
        _playerPosition = Vector2.Zero;
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

        // Initialize the player
        _player ??= new Player(5, GetRoomRespawnPoint(Content, "images/room-content.xml"));
        if(_playerPosition == Vector2.Zero) _player.SetPosition(GetRoomRespawnPoint(Content, "images/room-content.xml"));
        else _player.SetPosition(_playerPosition);

        _enemies = new List<Enemy>();
        _obstacles = new List<Obstacle>();
        _transitions = new List<RoomTransition>();
        _pickups = new List<Pickup>();
        _campfires = new List<Campfire>();

        GetRoomContentFromFile(Content, "images/room-content.xml");
        _obstacles = _tilemap.GetObstacles();

        foreach(Campfire campfire in _campfires)
        {
            _obstacles.Add(campfire);
        }

        if (_campfires.Count > 0) SetCheckpoint();

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(Core.Bounds.Left, _tilemap.TileHeight * 0.5f);

        _heartContainerSpacing = _heartContainerEmpty.Width * 1.25f;

        _heartContainerStartPosition = new Vector2(Core.Bounds.Right, _tilemap.TileHeight * 0.5f) - new Vector2(_heartContainerSpacing * 6, 0);
        _shieldContainerStartPosition = new Vector2(Core.Bounds.Right, _tilemap.TileHeight * 1f) - new Vector2(_heartContainerSpacing * 6, -1 * _heartContainerSpacing);

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

        _verticalWallSprite = _atlas.CreateSprite("vertical-wall", 4.0f);
        _horizontalWallSprite = _atlas.CreateSprite("horizontal-wall", 4.0f);

        // Load the bounce sound effect.
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect.
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the font.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");

        // Load the sound effect to play when ui actions occur.
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");

        _heartContainerEmpty = _atlas.CreateSprite("heart-container-empty", 2.0f);
        _heartContainerFull = _atlas.CreateSprite("heart-container-full", 2.0f);
        _shieldContainer = _atlas.CreateSprite("shield-container", 2.0f);

        _goldSprite = _atlas.CreateSprite("gold-1", 3.0f);
        _heartSprite = _atlas.CreateSprite("heart-1", 3.0f);
        _shieldSprite = _atlas.CreateSprite("cross-1", 3.0f);

        _campfireSprite = _atlas.CreateAnimatedSprite("campfire-animation", 4.0f);
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
            enemy.EntityInteraction(_enemies, _roomBounds);
        }

        // Check for keyboard input and handle it.
        CheckKeyboardInput();
        CheckGamepadInput();
        _player.Update(gameTime, _roomBounds);

        foreach(Campfire campfire in _campfires)
        {
            campfire.Update(gameTime, _roomBounds);
        }

        foreach(RoomTransition transition in _transitions)
        {
            transition.CheckIfPlayerEnter(_player);
        }

        _player.ObstacleInteraction(_obstacles, _roomBounds);

        // Check if any enemies have hit the player
        foreach (Enemy enemy in _enemies)
        {
            if(_player.EnemyHitPlayer(enemy)) Core.Audio.PlaySoundEffect(_bounceSoundEffect);
        }

        // Check if the player has hit any enemies
        if (_player.WeaponExtended && _player.Weapon.InHitFrame)
        {
            foreach (Enemy enemy in _enemies)
            {
                if (_player.HittingEnemy(enemy)) Core.Audio.PlaySoundEffect(_bounceSoundEffect);

                if (enemy.IsDead)
                {
                    Random rand = new Random();
                    double pickupRoll = rand.NextDouble();
                    bool playerAtMaxHealth = _player.Health.CurrentHealth == _player.Health.MaxHealth;

                    if (pickupRoll <= 0.8 || playerAtMaxHealth) _pickups.Add(new Gold(enemy.GetCenter(), _goldSprite, 10));
                    else _pickups.Add(new HeartPickup(enemy.GetCenter(), _heartSprite, 1));
                    continue;
                }
            }
        }

        if (_player.PlayChargeSound)
        {
            _player.PlayChargeSound = false;
            Core.Audio.PlaySoundEffect(_collectSoundEffect);
        }

        foreach (Pickup pickup in _pickups)
        {
            if (_player.CheckPickupInteraction(pickup)) Core.Audio.PlaySoundEffect(_collectSoundEffect);
        }

        _enemies.RemoveAll(enemy => enemy.IsDead);
        _pickups.RemoveAll(pickup => pickup.IsCollected);

        _camera.Follow(_player.Position, _roomSize);
        CalculateTranslation(_camera);

        if(_enemies.Count <= 0 && !_roomComplete)
        {
            _roomComplete = true;

            foreach(RoomTransition transition in _transitions)
            {
                transition.Open = true;
            }
        }

        if (_player.Dead)
        {
            _player.Revive();
            Core.ChangeScene(new GameplayScene(_player.LastCheckpoint, _player));
        } 
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

        if (keyboard.WasKeyJustPressed(Keys.F))
        {
            Core.ToggleFullscreen();
        }

        if (keyboard.WasKeyJustPressed(Keys.R))
        {
            SetCheckpoint();
        }


        if (keyboard.WasKeyJustPressed(Keys.P))
        {
            _pauseEnemiesForTesting = !_pauseEnemiesForTesting;
        }

        if (keyboard.WasKeyJustPressed(Keys.T))
        {
            _showHitboxes = !_showHitboxes;
        }

        if (keyboard.WasKeyJustPressed(Keys.D0))
        {
            _enemies.Clear();
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

        foreach(RoomTransition door in _transitions)
        {
            door.Draw();
        }

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

        foreach (Campfire campfire in _campfires)
        {
            campfire.Draw();
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

        for(int i = 0; i < _player.Health.CurrentHealth; i++)
        {
            _heartContainerFull.Draw(
                Core.SpriteBatch,
                _heartContainerStartPosition + new Vector2(_heartContainerSpacing * i, 0)
            );
        }

        for (int i = 0; i < _player.Health.MaxHealth - _player.Health.CurrentHealth; i++)
        {
            _heartContainerEmpty.Draw(
                Core.SpriteBatch, 
                _heartContainerStartPosition + new Vector2((_heartContainerSpacing * _player.Health.CurrentHealth) + (_heartContainerSpacing * i), 0)
            );
        }

        for (int i = 0; i < _player.Shield.CurrentHealth; i++)
        {
            _shieldContainer.Draw(
                Core.SpriteBatch,
                _heartContainerStartPosition + new Vector2(_heartContainerSpacing * i, _heartContainerSpacing)
            );
        }

        // Core.SpriteBatch.DrawString(
        //     _font,              // spriteFont
        //     $"HP: {_player.Health.CurrentHealth}", // text
        //     _healthTextPosition, // position
        //     Color.Red,        // color
        //     0.0f,               // rotation
        //     _healthTextOrigin,   // origin
        //     1.0f,               // scale
        //     SpriteEffects.None, // effects
        //     0.0f                // layerDepth
        // );

        // Core.SpriteBatch.DrawString(
        //     _font,              // spriteFont
        //     $"SH: {_player.Shield.CurrentHealth}", // text
        //     _shieldContainerStartPosition, // position
        //     Color.Blue,        // color
        //     0.0f,               // rotation
        //     _healthTextOrigin,   // origin
        //     1.0f,               // scale
        //     SpriteEffects.None, // effects
        //     0.0f                // layerDepth
        // );

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

    protected void SetCheckpoint()
    {
        _player.LastCheckpoint = _tilemapName;
    }

    protected Vector2 GetRoomRespawnPoint(ContentManager content, string fileName)
    {
        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                var rooms = root.Element("Rooms")?.Elements("Room");

                if (rooms != null)
                {
                    foreach (var room in rooms)
                    {
                        string name = room.Attribute("name")?.Value;

                        if (name != _tilemapName) continue;

                        int spawn_x = int.Parse(room.Attribute("spawn-x")?.Value);
                        int spawn_y = int.Parse(room.Attribute("spawn-y")?.Value);

                        return GetSpecificTile(spawn_x, spawn_y);
                    }
                }
            }
        }

        return GetSpecificTile(1, 1);
    }

    protected void GetRoomContentFromFile(ContentManager content,string fileName)
    {
        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                var rooms = root.Element("Rooms")?.Elements("Room");

                if (rooms != null)
                {
                    foreach (var room in rooms)
                    {
                        string name = room.Attribute("name")?.Value;

                        if (name != _tilemapName) continue;

                        var enemies = room.Element("Enemies")?.Elements();

                        if (enemies != null)
                        {
                            foreach (var enemy in enemies)
                            {
                                string enemyType = enemy.Name.LocalName;
                                int health = int.Parse(enemy.Attribute("health")?.Value);
                                int x = int.Parse(enemy.Attribute("x")?.Value);
                                int y = int.Parse(enemy.Attribute("y")?.Value);

                                switch (enemyType)
                                {
                                    case "Slime":
                                        _enemies.Add(new Slime(health, GetSpecificTile(x, y), _slimeSprite, _player));
                                        break;
                                    case "Bat":
                                        _enemies.Add(new Bat(health, GetSpecificTile(x, y), _batSprite));
                                        break;
                                    case "Spider":
                                        _enemies.Add(new Spider(health, GetSpecificTile(x, y), _spiderSprite, _player));
                                        break;
                                }
                            }
                        }

                        var pickups = room.Element("Pickups")?.Elements();

                        if (pickups != null)
                        {
                            foreach (var pickup in pickups)
                            {
                                string pickupType = pickup.Name.LocalName;
                                int value = int.Parse(pickup.Attribute("value")?.Value);
                                int x = int.Parse(pickup.Attribute("x")?.Value);
                                int y = int.Parse(pickup.Attribute("y")?.Value);

                                switch (pickupType)
                                {
                                    case "Gold":
                                        _pickups.Add(new Gold(GetSpecificTile(x, y), _goldSprite, value));
                                        break;
                                    case "Shield":
                                        _pickups.Add(new Shield(GetSpecificTile(x, y), _shieldSprite, value));
                                        break;
                                    case "Heart":
                                        _pickups.Add(new HeartPickup(GetSpecificTile(x, y), _heartSprite, value));
                                        break;
                                }
                            }
                        }

                        var campfires = room.Element("Campfires")?.Elements();

                        if (campfires != null)
                        {
                            foreach (var campfire in campfires)
                            {
                                int x = int.Parse(campfire.Attribute("x")?.Value);
                                int y = int.Parse(campfire.Attribute("y")?.Value);

                                _campfires.Add(new Campfire(_campfireSprite, GetSpecificTile(x, y), _player));
                            }
                        }

                        var transitions = room.Element("Transitions")?.Elements();

                        if (transitions != null)
                        {
                            foreach(var transition in transitions)
                            {
                                int x = int.Parse(transition.Attribute("x")?.Value);
                                int y = int.Parse(transition.Attribute("y")?.Value);
                                int width = int.Parse(transition.Attribute("width")?.Value);
                                int height = int.Parse(transition.Attribute("height")?.Value);
                                string destination = transition.Attribute("destination")?.Value;
                                float destination_x = int.Parse(transition.Attribute("destination-x")?.Value);
                                float destination_y = int.Parse(transition.Attribute("destination-y")?.Value);
                                var doors = transition.Elements();

                                if (x == -1) x = _tilemap.Columns - 1;
                                if (y == -1) y = _tilemap.Rows - 1;

                                if (destination_x == 0) destination_x = _tilemap.TileWidth + 10;
                                else destination_x = _tilemap.TileWidth * destination_x;
                                if (destination_y == 0) destination_y = _tilemap.TileHeight + 10;
                                else destination_y = _tilemap.TileHeight * destination_y;

                                Vector2 transitionDestination = new Vector2(destination_x, destination_y);
                                List<Obstacle> obstacles = new List<Obstacle>();

                                foreach(var door in doors)
                                {
                                    int door_x = int.Parse(door.Attribute("x")?.Value);
                                    int door_y = int.Parse(door.Attribute("y")?.Value);

                                    if (door_x == -1) door_x = _tilemap.Columns - 1;
                                    if (door_y == -1) door_y = _tilemap.Rows - 1;

                                    string spriteName = door.Attribute("sprite")?.Value;
                                    Sprite sprite;
                                    if (spriteName == "horizontal-wall") sprite = _horizontalWallSprite;
                                    else sprite = _verticalWallSprite;

                                    Debug.WriteLine($"Added door at: X{door_x} Y{door_y}");

                                    obstacles.Add(new Obstacle(sprite, GetSpecificTile(door_x, door_y)));
                                }

                                Scene destinationScene;

                                if (destination == "victory-screen") destinationScene = new VictoryScene(_player);
                                else destinationScene = new GameplayScene(destination, _player, transitionDestination);

                                RoomTransition newTransition = new RoomTransition
                                (
                                    GetSpecificTile(x, y),
                                    (int)_tilemap.TileWidth * width,
                                    (int)_tilemap.TileHeight * height,
                                    destinationScene,
                                    transitionDestination,
                                    obstacles
                                );

                                _transitions.Add(newTransition);
                            }
                        }
                    }
                }
            }
        }
    }
}