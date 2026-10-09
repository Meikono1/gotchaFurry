using Godot;
using GotchaFurry.Core.Logging;

namespace GotchaFurry.Game;

/// <summary>
/// Darstellung des Haustiers. Läuft vorerst zwischen zwei Punkten hin und her.
/// Erwartet ein Kind vom Typ Sprite2D mit dem Namen "Sprite".
/// </summary>
public partial class PetView : Node2D
{
    private static readonly GameLogger Log = LogManager.GetLogger<PetView>();

    /// <summary>Pixel pro Sekunde.</summary>
    [Export] public float Speed { get; set; } = 80f;

    /// <summary>Maximaler Abstand zur Startposition in jede Richtung.</summary>
    [Export] public float WalkDistance { get; set; } = 200f;

    [Export] public float BobHeight { get; set; } = 4f;
    [Export] public float BobFrequency { get; set; } = 6f;

    /// <summary>Blickrichtung der Grafik ohne Spiegelung.</summary>
    [Export] public bool SpriteFacesRight { get; set; } = true;

    private Sprite2D _sprite;
    private Vector2 _origin;
    private int _direction = 1;
    private double _bobTime;

    public override void _Ready()
    {
        _sprite = GetNodeOrNull<Sprite2D>("Sprite");
        if (_sprite == null)
        {
            Log.Error("Kind-Node 'Sprite' (Sprite2D) fehlt. Haustier wird nicht bewegt.");
            SetProcess(false);
            return;
        }

        if (_sprite.Texture == null)
        {
            Log.Warn("Sprite hat keine Textur. Testgrafik im Inspector zuweisen.");
        }

        _origin = Position;
        UpdateFacing();
        Log.Debug($"PetView bereit. Start={_origin}, Distanz={WalkDistance}, Speed={Speed}");
    }

    public override void _Process(double delta)
    {
        float x = Position.X + _direction * Speed * (float)delta;
        float minX = _origin.X - WalkDistance;
        float maxX = _origin.X + WalkDistance;

        if (x >= maxX)
        {
            x = maxX;
            _direction = -1;
            UpdateFacing();
        }
        else if (x <= minX)
        {
            x = minX;
            _direction = 1;
            UpdateFacing();
        }

        Position = new Vector2(x, _origin.Y);

        // Wippen nur am Sprite, damit die Position des Haustiers sauber bleibt
        _bobTime += delta;
        float bob = -Mathf.Abs((float)Mathf.Sin(_bobTime * BobFrequency)) * BobHeight;
        _sprite.Position = new Vector2(0, bob);
    }

    private void UpdateFacing()
    {
        _sprite.FlipH = SpriteFacesRight ? _direction < 0 : _direction > 0;
    }

    // TODO: Verhalten abhängig von Stimmung (z.B. langsamer bei wenig Energie, Pausen, Schlafen)
}