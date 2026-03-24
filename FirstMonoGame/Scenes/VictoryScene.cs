using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Graphics;
using FirstMonoGame.UI;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using Gum.Forms;
using FirstMonoGame.Objects;

namespace FirstMonoGame.Scenes;

public class VictoryScene : Scene
{
    private const string VICTORY_TEXT = "Victory!";
    private const string SCORE_TEXT = "Your score was: ";
    private const string PRESS_ENTER_TEXT = "Press Enter To Return to Menu";

    private string _score = "";

    // The font to use to render normal text.
    private SpriteFont _font;

    // The font used to render the title text.
    private SpriteFont _font5x;

    // The position to draw the dungeon text at.
    private Vector2 _victoryTextPos;

    // The origin to set for the dungeon text.
    private Vector2 _victoryTextOrigin;

    // The position to draw the slime text at.
    private Vector2 _scoreTextPos;

    // The origin to set for the slime text.
    private Vector2 _scoreTextOrigin;
    private Vector2 _scorePos;
    private Vector2 _scoreOrigin;

    // The position to draw the press enter text at.
    private Vector2 _pressEnterPos;

    // The origin to set for the press enter text when drawing it.
    private Vector2 _pressEnterOrigin;

    // The texture used for the background pattern.
    private Texture2D _backgroundPattern;

    // The destination rectangle for the background pattern to fill.
    private Rectangle _backgroundDestination;

    // The offset to apply when drawing the background pattern so it appears to
    // be scrolling.
    private Vector2 _backgroundOffset;

    // The speed that the background pattern scrolls.
    private float _scrollSpeed = 50.0f;

    // Fluctuating float to increase and decrease the scale of the title text
    private float _textScaleModifier;

    // The maximum variance in the text scale for the title text
    private float _maxTextScaleVariance = 0.05f;

    // GUM UI Elements
    private SoundEffect _uiSoundEffect;
    private Panel _victoryScreenButtonsPanel;

    // The options button used to open the options menu.
    private AnimatedButton _returnButton;

    // Reference to the texture atlas that we can pass to UI elements when they
    // are created.
    private TextureAtlas _atlas;
    private Player _player;

    public VictoryScene(Player player)
    {
        _player = player;
    }

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // While on the title screen, we can enable exit on escape so the player
        // can close the game by pressing the escape key.
        Core.ExitOnEscape = true;

        // Set the position and origin for the Dungeon text.
        Vector2 size = _font5x.MeasureString(VICTORY_TEXT);
        _victoryTextPos = new Vector2(Core.Bounds.Width/2, 100);
        _victoryTextOrigin = size * 0.5f;

        // Set the position and origin for the Slime text.
        size = _font.MeasureString(SCORE_TEXT);
        _scoreTextPos = new Vector2(Core.Bounds.Width / 2, 250);
        _scoreTextOrigin = size * 0.5f;

        // Set the position and origin for the press enter text.
        size = _font.MeasureString(PRESS_ENTER_TEXT);
        _pressEnterPos = new Vector2(Core.Bounds.Width / 2, 620);
        _pressEnterOrigin = size * 0.5f;

        // Initialize the offset of the background pattern at zero.
        _backgroundOffset = Vector2.Zero;

        // Set the background pattern destination rectangle to fill the entire
        // screen background.
        _backgroundDestination = Core.Bounds;

        int score = 0;
        if (_player != null) score = _player.Gold;

        _score += score;

        size = _font5x.MeasureString(_score);
        _scorePos = new Vector2(Core.Bounds.Width / 2, 350);
        _scoreOrigin = size * 0.5f;

        InitializeUI();
    }

    public override void LoadContent()
    {
        // Load the font for the standard text.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");

        // Load the font for the title text.
        _font5x = Content.Load<SpriteFont>("fonts/04B_30_5x");

        // Load the background pattern texture.
        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");

        // Load the sound effect to play when ui actions occur.
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");

        // Load the texture atlas from the xml configuration file.
        _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
    }

    public override void Update(GameTime gameTime)
    {
        // If the user presses enter, switch to the game scene.
        // if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        // {
        //     Core.ChangeScene(new TitleScene());
        // }

        _textScaleModifier = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * _maxTextScaleVariance;

        // Update the offsets for the background pattern wrapping so that it
        // scrolls down and to the right.
        float offset = _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _backgroundOffset.X += offset;
        _backgroundOffset.Y += offset;

        // Ensure that the offsets do not go beyond the texture bounds so it is
        // a seamless wrap.
        _backgroundOffset.X %= _backgroundPattern.Width;
        _backgroundOffset.Y %= _backgroundPattern.Height;

        GumService.Default.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Core.BackgroundColor);

        // Draw the background pattern first using the PointWrap sampler state.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(_backgroundPattern, _backgroundDestination, new Rectangle(_backgroundOffset.ToPoint(), _backgroundDestination.Size), Color.White * 0.5f);
        Core.SpriteBatch.End();

        if (_victoryScreenButtonsPanel.IsVisible)
        {
            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.Black * 0.5f;

            // Draw the Dungeon text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow.
            Core.SpriteBatch.DrawString(_font5x, VICTORY_TEXT, _victoryTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _victoryTextOrigin, 1.0f + _textScaleModifier, SpriteEffects.None, 1.0f);

            // Draw the Dungeon text on top of that at its original position.
            Core.SpriteBatch.DrawString(_font5x, VICTORY_TEXT, _victoryTextPos, Color.White, 0.0f, _victoryTextOrigin, 1.0f + _textScaleModifier, SpriteEffects.None, 1.0f);

            // Draw the Slime text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow.
            //Core.SpriteBatch.DrawString(_font5x, SCORE_TEXT, _scoreTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _scoreTextOrigin, 1.0f - _textScaleModifier, SpriteEffects.None, 1.0f);

            // Draw the Slime text on top of that at its original position.
            Core.SpriteBatch.DrawString(_font, SCORE_TEXT, _scoreTextPos, Color.White, 0.0f, _scoreTextOrigin, 2.0f, SpriteEffects.None, 1.0f);

            Core.SpriteBatch.DrawString(_font5x, _score, _scorePos, Color.White, 0.0f, _scoreOrigin, 1.0f - (_textScaleModifier * 2), SpriteEffects.None, 1.0f);

            // Always end the sprite batch when finished.
            Core.SpriteBatch.End();
        }



        GumService.Default.Draw();
    }

    private void CreateTitlePanel()
    {
        // Create a container to hold all of our buttons
        _victoryScreenButtonsPanel = new Panel();
        _victoryScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _victoryScreenButtonsPanel.AddToRoot();

        AnimatedButton returnButton = new AnimatedButton(_atlas);
        returnButton.Anchor(Gum.Wireframe.Anchor.Bottom);
        returnButton.X = 0;
        returnButton.Y = -12;
        returnButton.Width = 70;
        returnButton.Text = "Return to Menu";
        returnButton.Click += HandleReturnClicked;
        _victoryScreenButtonsPanel.AddChild(returnButton);

        returnButton.IsFocused = true;
    }

    private void HandleReturnClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Change to the game scene to start the game.
        Core.ChangeScene(new TitleScene());
    }
}