using System.Collections.Generic;
using GotchaFurry.Core.Logging;

namespace GotchaFurry.Pet;

public static class SpeciesCatalog
{
    private static readonly GameLogger Log = LogManager.GetLogger(nameof(SpeciesCatalog));

    private static readonly Dictionary<PetSpecies, SpeciesDefinition> Definitions = new()
    {
        [PetSpecies.Kobold] = new SpeciesDefinition(
            PetSpecies.Kobold,
            "Kobold",
            new Dictionary<NeedType, double>
            {
                [NeedType.Satiety] = 8.0,
                [NeedType.Energy] = 5.0,
                [NeedType.Fun] = 10.0,
                [NeedType.Hygiene] = 4.0,
                [NeedType.Affection] = 6.0
            })
    };

    public static SpeciesDefinition Get(PetSpecies species)
    {
        if (Definitions.TryGetValue(species, out SpeciesDefinition definition))
        {
            return definition;
        }

        Log.Error($"Keine Definition für Spezies {species}. Fallback auf Kobold.");
        return Definitions[PetSpecies.Kobold];
    }
}