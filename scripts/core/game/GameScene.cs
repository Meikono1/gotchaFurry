using System.Linq;
using Godot;
using GotchaFurry.Core;
using GotchaFurry.Core.Logging;
using GotchaFurry.Pet;
using GotchaFurry.UI;

namespace GotchaFurry.Game;

/// <summary>
/// Root der Spielszene. Treibt vorerst nur die Simulation an und loggt die Bedürfnisse.
/// </summary>
public partial class GameScene : Node
{
    private static readonly GameLogger Log = LogManager.GetLogger<GameScene>();

    /// <summary>Zeitfaktor zum Testen. 360 = 10 Sekunden Echtzeit entsprechen einer Stunde.</summary>
    [Export] public double TimeScale { get; set; } = 1.0;

    [Export] public double DebugLogIntervalSeconds { get; set; } = 5.0;

    private PetData _pet;
    private NeedsSimulation _simulation;
    private double _debugLogTimer;
    private NeedsHud _hud;

    public override void _Ready()
    {
        _pet = GameManager.Instance?.CurrentPet;
        if (_pet == null)
        {
            Log.Error("Kein Haustier vorhanden. Wurde die Spielszene direkt gestartet statt über das Hauptmenü?");
            SetProcess(false);
            return;
        }

        _simulation = new NeedsSimulation(_pet);

        _hud = GetNodeOrNull<NeedsHud>("%NeedsHud");
        if (_hud == null)
        {
            Log.Warn("NeedsHud nicht gefunden. Ist der Node als eindeutiger Name (%) markiert? Balken werden nicht angezeigt.");
        }

        _hud?.UpdateNeeds(_pet.Needs);
        Log.Info($"Spielszene geladen mit {_pet}, TimeScale={TimeScale}");
        LogNeeds();
    }

    public override void _Process(double delta)
    {
        _simulation.Tick(delta, TimeScale);
        _hud?.UpdateNeeds(_pet.Needs);

        _debugLogTimer += delta;
        if (_debugLogTimer >= DebugLogIntervalSeconds)
        {
            _debugLogTimer = 0;
            LogNeeds();
        }
    }

    private void LogNeeds()
    {
        if (!Log.IsDebugEnabled)
        {
            return;
        }

        string levels = string.Join(", ", _pet.Needs.Levels.Select(kv => $"{kv.Key}={kv.Value:F1}"));
        Log.Debug($"{_pet}: {levels}");
    }
}