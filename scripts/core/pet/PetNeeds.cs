using System;
using System.Collections.Generic;

namespace GotchaFurry.Pet;

public sealed class PetNeeds
{
    public const double Min = 0.0;
    public const double Max = 100.0;

    // Öffentlich mit Setter für die JSON-Serialisierung
    public Dictionary<NeedType, double> Levels { get; set; } = [];

    public static PetNeeds CreateFull()
    {
        var needs = new PetNeeds();
        foreach (NeedType type in Enum.GetValues<NeedType>())
        {
            needs.Levels[type] = Max;
        }

        return needs;
    }

    // Fehlender Eintrag = voll erfüllt. Relevant, wenn alte Spielstände ein neues Bedürfnis noch nicht kennen.
    public double Get(NeedType type) => Levels.TryGetValue(type, out double value) ? value : Max;

    public void Set(NeedType type, double value) => Levels[type] = Math.Clamp(value, Min, Max);

    public void Change(NeedType type, double delta) => Set(type, Get(type) + delta);
}