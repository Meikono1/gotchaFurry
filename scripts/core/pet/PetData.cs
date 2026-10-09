using System;

namespace GotchaFurry.Pet;

/// <summary>
/// Reines Datenmodell des Haustiers. Keine Logik, wird so gespeichert und geladen.
/// </summary>
public sealed class PetData
{
    public string Name { get; set; }
    public PetSpecies Species { get; set; }
    public PetNeeds Needs { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastUpdated { get; set; }

    public static PetData CreateNew(string name, PetSpecies species)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return new PetData
        {
            Name = name,
            Species = species,
            Needs = PetNeeds.CreateFull(),
            CreatedAt = now,
            LastUpdated = now
        };
    }

    public override string ToString() => $"{Name} ({Species})";
}