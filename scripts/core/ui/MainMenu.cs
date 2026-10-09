using Godot;
using PetGame.Core;
using PetGame.Core.Logging;

namespace PetGame.UI;

public partial class MainMenu : Control
{
    private static readonly GameLogger Log = LogManager.GetLogger<MainMenu>();

    private Button _startButton;
    private Button _loadButton;
    private Button _quitButton;

    public override void _Ready()
    {
        if (GameManager.Instance == null)
        {
            Log.Error("GameManager-Autoload fehlt. Projekteinstellungen -> Globals -> Autoload prüfen.");
            return;
        }

        _startButton = GetNodeOrNull<Button>("%StartButton");
        _loadButton = GetNodeOrNull<Button>("%LoadButton");
        _quitButton = GetNodeOrNull<Button>("%QuitButton");

        if (_startButton == null || _loadButton == null || _quitButton == null)
        {
            Log.Error("Buttons nicht gefunden. Sind StartButton, LoadButton und QuitButton als eindeutiger Name (%) markiert?");
            return;
        }

        _startButton.Pressed += OnStartPressed;
        _loadButton.Pressed += OnLoadPressed;
        _quitButton.Pressed += OnQuitPressed;

        UpdateLoadButton();
        _startButton.GrabFocus();

        Log.Info("Hauptmenü geladen.");
    }

    private void UpdateLoadButton()
    {
        bool hasSave = SaveService.HasSave();
        _loadButton.Disabled = !hasSave;
        Log.Debug($"Spielstand vorhanden: {hasSave}");
    }

    private void OnStartPressed()
    {
        Log.Info("Button 'Starten' gedrückt.");
        GameManager.Instance.StartNewGame();
    }

    private void OnLoadPressed()
    {
        Log.Info("Button 'Laden' gedrückt.");
        GameManager.Instance.LoadGame();
    }

    private void OnQuitPressed()
    {
        Log.Info("Button 'Beenden' gedrückt.");
        GameManager.Instance.QuitGame();
    }
}