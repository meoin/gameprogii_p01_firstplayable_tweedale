using FirstMonoGame.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Gum.Forms;
using Gum.Forms.Controls;
using MonoGameLibrary;
using MonoGameGum;

namespace FirstMonoGame;

public class Game1 : Core
{
    // The background theme song.
    private Song _themeSong;
    private static Color _backgroundColor = new Color(32, 40, 78, 255);

    private static int _defaultWidth = 1280;
    public static int _defaultHeight = 720;
    public static GameWindow GameWindowInstance;

    public Game1() : base("Dungeon Slime", _defaultWidth, _defaultHeight, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        // Start playing the background music.
        Audio.PlaySong(_themeSong);
        Audio.SetMasterVolume(1f);
        Audio.SetMusicVolume(0.2f);
        Audio.SetSoundEffectVolume(0.2f);

        GameWindowInstance = Window;

        // Initialize the Gum UI service
        InitializeGum();

        Core.BackgroundColor = _backgroundColor;

        // Start the game with the title scene.
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {
        // Load the background theme music.
        _themeSong = Content.Load<Song>("audio/theme");
    }

    private void InitializeGum()
    {
        // Initialize the Gum service. The second parameter specifies
        // the version of the default visuals to use. V3 is the latest
        // version.
        GumService.Default.Initialize(this, DefaultVisualsVersion.V3);

        // Tell the Gum service which content manager to use.  We will tell it to
        // use the global content manager from our Core.
        GumService.Default.ContentLoader.XnaContentManager = Core.Content;

        // Register keyboard input for UI control.
        FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

        // Register gamepad input for Ui control.
        FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

        // Customize the tab reverse UI navigation to also trigger when the keyboard
        // Up arrow key is pushed.
        FrameworkElement.TabReverseKeyCombos.Add(
           new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Up });

        // Customize the tab UI navigation to also trigger when the keyboard
        // Down arrow key is pushed.
        FrameworkElement.TabKeyCombos.Add(
           new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });

        // The assets created for the UI were done so at 1/4th the size to keep the size of the
        // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
        // the game's resolution then tell gum to zoom in by a factor of 4.
        GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
        GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
        GumService.Default.Renderer.Camera.Zoom = 4.0f;
    }
}