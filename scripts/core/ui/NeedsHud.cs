using System;
using System.Collections.Generic;
using Godot;
using GotchaFurry.Core.Logging;
using GotchaFurry.Pet;

namespace GotchaFurry.UI;

/// <summary>
/// Zeigt pro Bedürfnis einen Balken. Die Balken werden zur Laufzeit aus NeedType erzeugt.
/// </summary>
public partial class NeedsHud : VBoxContainer
{
    private static readonly GameLogger Log = LogManager.GetLogger<NeedsHud>();

    private const double WarnThreshold = 50.0;
    private const double CriticalThreshold = 25.0;

    private static readonly Color GoodColor = new(0.30f, 0.75f, 0.35f);
    private static readonly Color WarnColor = new(0.95f, 0.75f, 0.20f);
    private static readonly Color CriticalColor = new(0.90f, 0.25f, 0.25f);

    [Export] public Vector2 BarSize { get; set; } = new(200, 20);
    [Export] public float LabelWidth { get; set; } = 100f;

    private readonly Dictionary<NeedType, NeedBar> _bars = new();

    private sealed record NeedBar(ProgressBar Bar, StyleBoxFlat Fill);

    public override void _Ready()
    {
        foreach (NeedType type in Enum.GetValues<NeedType>())
        {
            _bars[type] = CreateBar(type);
        }

        Log.Debug($"{_bars.Count} Bedürfnis-Balken erstellt.");
    }

    public void UpdateNeeds(PetNeeds needs)
    {
        foreach (var (type, needBar) in _bars)
        {
            double value = needs.Get(type);
            needBar.Bar.Value = value;

            Color color = GetColor(value);
            if (needBar.Fill.BgColor != color)
            {
                needBar.Fill.BgColor = color;
            }
        }
    }

    private NeedBar CreateBar(NeedType type)
    {
        var row = new HBoxContainer();

        var label = new Label
        {
            Text = GetDisplayName(type),
            CustomMinimumSize = new Vector2(LabelWidth, 0)
        };

        var bar = new ProgressBar
        {
            MinValue = PetNeeds.Min,
            MaxValue = PetNeeds.Max,
            Value = PetNeeds.Max,
            ShowPercentage = false,
            CustomMinimumSize = BarSize
        };

        // Eigene StyleBox pro Balken, sonst ändern sich alle Farben gleichzeitig
        var fill = new StyleBoxFlat { BgColor = GoodColor };
        bar.AddThemeStyleboxOverride("fill", fill);

        row.AddChild(label);
        row.AddChild(bar);
        AddChild(row);

        return new NeedBar(bar, fill);
    }

    private static Color GetColor(double value)
    {
        if (value < CriticalThreshold)
        {
            return CriticalColor;
        }

        return value < WarnThreshold ? WarnColor : GoodColor;
    }

    private static string GetDisplayName(NeedType type) => type switch
    {
        NeedType.Satiety => "Sättigung",
        NeedType.Energy => "Energie",
        NeedType.Fun => "Spaß",
        NeedType.Hygiene => "Hygiene",
        NeedType.Affection => "Zuneigung",
        _ => type.ToString()
    };
}