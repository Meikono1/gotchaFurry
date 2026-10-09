using System.Collections.Generic;

namespace GotchaFurry.Pet;

/// <summary>
/// Spezies-spezifische Werte. Abnahme in Punkten pro Stunde Spielzeit.
/// </summary>
public sealed record SpeciesDefinition(
    PetSpecies Species,
    string DisplayName,
    IReadOnlyDictionary<NeedType, double> DecayPerHour);