using System;
using GotchaFurry.Core.Logging;

namespace GotchaFurry.Pet;

/// <summary>
/// Lässt die Bedürfnisse über die Zeit sinken. Kein Node, damit unabhängig von der Szene testbar.
/// </summary>
public sealed class NeedsSimulation
{
    private static readonly GameLogger Log = LogManager.GetLogger<NeedsSimulation>();
    private static readonly TimeSpan MaxOfflineTime = TimeSpan.FromHours(48);

    private readonly PetData _pet;
    private readonly SpeciesDefinition _species;

    public NeedsSimulation(PetData pet)
    {
        _pet = pet;
        _species = SpeciesCatalog.Get(pet.Species);
    }

    /// <param name="realDeltaSeconds">Vergangene Echtzeit seit dem letzten Tick.</param>
    /// <param name="timeScale">Zeitfaktor zum Testen. 1 = Echtzeit.</param>
    public void Tick(double realDeltaSeconds, double timeScale)
    {
        ApplyDecay(realDeltaSeconds * timeScale / 3600.0);
        _pet.LastUpdated = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Berechnet die Zeit seit dem letzten Speichern nach. Aufrufen nach dem Laden.
    /// </summary>
    public void ApplyOfflineProgress()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        TimeSpan elapsed = now - _pet.LastUpdated;

        if (elapsed < TimeSpan.Zero)
        {
            Log.Warn($"Letzte Aktualisierung liegt in der Zukunft ({_pet.LastUpdated:u}). Systemzeit verstellt? Offline-Fortschritt übersprungen.");
            _pet.LastUpdated = now;
            return;
        }

        if (elapsed > MaxOfflineTime)
        {
            Log.Info($"Offline-Zeit {elapsed.TotalHours:F1} h wird auf {MaxOfflineTime.TotalHours} h begrenzt.");
            elapsed = MaxOfflineTime;
        }

        Log.Info($"Offline-Fortschritt für {_pet}: {elapsed.TotalHours:F2} h");
        ApplyDecay(elapsed.TotalHours);
        _pet.LastUpdated = now;
    }

    private void ApplyDecay(double hours)
    {
        if (hours <= 0)
        {
            return;
        }

        foreach (var (type, ratePerHour) in _species.DecayPerHour)
        {
            double before = _pet.Needs.Get(type);
            _pet.Needs.Change(type, -ratePerHour * hours);

            if (before > PetNeeds.Min && _pet.Needs.Get(type) <= PetNeeds.Min)
            {
                Log.Warn($"{_pet}: {type} ist auf 0 gefallen.");
            }
        }
    }
}