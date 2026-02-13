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

public class GameScene : Scene
{
    // Defines the slime animated sprite.
    private AnimatedSprite _slime;

    // Defines the bat animated sprite.
    private AnimatedSprite _bat;

    // Speed multiplier when moving.
    private const float MOVEMENT_SPEED = 5.0f;

    // Defines the tilemap to draw.
    private Tilemap _tilemap;

    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;

    // The sound effect to play when the bat bounces off the edge of the screen.
    private SoundEffect _bounceSoundEffect;

    // The sound effect to play when the slime eats a bat.
    private SoundEffect _collectSoundEffect;

    // The SpriteFont Description used to draw text
    private SpriteFont _font;

    // Tracks the players score.
    private int _score;

    // Defines the position to draw the score text at.
    private Vector2 _scoreTextPosition;

    // Defines the position to draw the score text at.
    private Vector2 _healthTextPosition;

    // Defines the origin used when drawing the score text.
    private Vector2 _scoreTextOrigin;

    // Defines the origin used when drawing the score text.
    private Vector2 _healthTextOrigin;

    // A reference to the pause panel UI element so we can set its visibility
    // when the game is paused.
    private Panel _pausePanel;

    // A reference to the resume button UI element so we can focus it
    // when the game is paused.
    private AnimatedButton _resumeButton;

    // The UI sound effect to play when a UI event is triggered.
    private SoundEffect _uiSoundEffect;

    // Reference to the texture atlas that we can pass to UI elements when they
    // are created.
    private TextureAtlas _atlas;

    private Player _player;
    private Sprite _swordSprite;
    private List<Enemy> _enemies;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;

        Rectangle screenBounds = Core.Bounds;

        _roomBounds = new Rectangle(
             (int)_tilemap.TileWidth,
             (int)_tilemap.TileHeight,
             screenBounds.Width - (int)_tilemap.TileWidth * 2,
             screenBounds.Height - (int)_tilemap.TileHeight * 2
         );

        // Initial slime position will be the center tile of the tile map.
        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;
        Vector2 playerPosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);

        // Initialize the player
        _player = new Player(5, playerPosition, _slime, _swordSprite);

        _enemies = new List<Enemy>();

        for (int i = 0; i < 3; i++)
        {
            // Initial slime position to a random position on the screen
            Vector2 slimePosition = GetRandomTile();

            _enemies.Add(new Slime(5, slimePosition, new AnimatedSprite(_slime), _player));
        }

        for (int i = 0; i < 2; i++)
        {
            // Initial bat position to a random position on the screen
            Vector2 batPosition = GetRandomTile();

            _enemies.Add(new Bat(5, batPosition, new AnimatedSprite(_bat)));
        }

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

        _healthTextPosition = new Vector2(_roomBounds.Right, _tilemap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
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

        // Create the slime animated sprite from the atlas.
        _slime = _atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);

        _swordSprite = _atlas.CreateSprite("unfocused-button");
        _swordSprite.Origin = new Vector2(0, _swordSprite.Region.Height * 0.5f);
        _swordSprite.Scale = new Vector2(2.0f, 2.0f);

        // Create the bat animated sprite from the atlas.
        _bat = _atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        // Load the bounce sound effect.
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect.
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the font.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");

        // Load the sound effect to play when ui actions occur.
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");
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

        foreach (Enemy enemy in _enemies)
        {
            enemy.Update(gameTime, _roomBounds);
        }

        // Check for keyboard input and handle it.
        CheckKeyboardInput();
        CheckGamepadInput();
        _player.Update(gameTime, _roomBounds);


        // Loop through each enemy
        foreach (Enemy enemy in _enemies)
        {
            // Check if enemies are colliding with each other, if so push them away a bit
            // This is janky and needs to be improved tbh
            foreach(Enemy otherEnemy in _enemies)
            {
                if(enemy != otherEnemy && enemy.Bounds.Intersects(otherEnemy.Bounds))
                {
                    Vector2 awayDirection = otherEnemy.Position - enemy.Position;

                    //Debug.WriteLine(awayDirection);

                    if (awayDirection != Vector2.Zero) awayDirection.Normalize();

                    enemy.ResetPosition(enemy.Position - awayDirection);
                }
            }

            // Check if player is sticking their sword out
            if (_player.SwordExtended)
            {
                // If the enemy is touching the swords hitbox, set them to a new position and gain score
                if (enemy.Bounds.Intersects(_player.SwordHitbox))
                {
                    // Change the bat position by setting the x and y values equal to
                    // the column and row multiplied by the width and height.
                    enemy.ResetPosition(GetRandomTile());

                    // Play the collect sound effect.
                    Core.Audio.PlaySoundEffect(_collectSoundEffect);

                    // Increase the player's score.
                    _score += 100;
                }
            }

            if (enemy.Bounds.Intersects(_player.Bounds))
            {
                if (!_player.InvincibleAfterBeingHurt) Core.Audio.PlaySoundEffect(_bounceSoundEffect);

                _player.TakeDamage(1);

                if (_player.Health.CurrentHealth <= 0)
                {
                    Core.ChangeScene(new TitleScene());
                }
            }   
        } 
    }

    private Vector2 GetRandomTile()
    {
        // Choose a random row and column based on the total number of each
        int column = Random.Shared.Next(1, _tilemap.Columns - 1);
        int row = Random.Shared.Next(1, _tilemap.Rows - 1);

        return new Vector2(column * _tilemap.TileWidth, row * _tilemap.TileHeight);
    }

    private void PauseGame()
    {
        // Make the pause panel UI element visible.
        _pausePanel.IsVisible = true;

        // Set the resume button to have focus
        _resumeButton.IsFocused = true;
    }

    private void CheckKeyboardInput()
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

    private void CheckGamepadInput()
    {
        GamePadInfo gamepadOne = Core.Input.GamePads[(int)PlayerIndex.One];

        float speed = MOVEMENT_SPEED;

        Vector2 movementVector = Vector2.Zero;

        // If the start button is pressed, pause the game
        if (gamepadOne.WasButtonJustPressed(Buttons.Start))
        {
            PauseGame();
            return;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Core.BackgroundColor);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap
        _tilemap.Draw(Core.SpriteBatch);

        // Draw the slime sprite.
        _player.Draw();

        if (_player.SwordExtended)
        {
            Core.DrawRectangleOutline(_player.SwordHitbox);
        }

        foreach(Enemy enemy in _enemies)
        {
            enemy.Draw();
        }

        // Draw the score.
        Core.SpriteBatch.DrawString(
            _font,              // spriteFont
            $"Score: {_score}", // text
            _scoreTextPosition, // position
            Color.White,        // color
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
            Color.White,        // color
            0.0f,               // rotation
            _healthTextOrigin,   // origin
            1.0f,               // scale
            SpriteEffects.None, // effects
            0.0f                // layerDepth
        );

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        // Draw the Gum UI
        GumService.Default.Draw();
    }

    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();

        CreatePausePanel();
    }

    private void CreatePausePanel()
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

    private void HandleResumeButtonClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Make the pause panel invisible to resume the game.
        _pausePanel.IsVisible = false;
        _resumeButton.IsFocused = false;
    }

    private void HandleQuitButtonClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Go back to the title scene.
        Core.ChangeScene(new TitleScene());
    }
}