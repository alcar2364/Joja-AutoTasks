// Purpose: Defines the immutable TaskID value type used for deterministic date-based identifiers.
namespace JojaAutoTasks.Domain.Identifiers;

internal readonly struct DayKey : IEquatable<DayKey>
{

    private static readonly StringComparer Comparer = StringComparer.Ordinal;
    private readonly string _dayKey;


    /// <summary>Initializes a new <see cref="DayKey"/> from a raw identifier string.</summary>
    /// <param name="dayKey">The raw day key identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dayKey"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="dayKey"/> cannot be validated for format, season, or day value.</exception>
    public DayKey(string dayKey)
    {
        string normalizedDayKey = IdentifierUtility.NormalizeIdentifier(dayKey);
        ValidateDayKey(normalizedDayKey);
        _dayKey = normalizedDayKey;
    }

    // Gets canonical DayKey string
    public string Value => _dayKey ?? string.Empty;
    public bool Equals(DayKey other) => Comparer.Equals(_dayKey, other._dayKey);
    public override bool Equals(object? obj) => obj is DayKey other && Equals(other);
    public override int GetHashCode() => Comparer.GetHashCode(_dayKey ?? string.Empty);
    public static bool operator ==(DayKey left, DayKey right) => left.Equals(right);
    public static bool operator !=(DayKey left, DayKey right) => !left.Equals(right);
    public override string ToString() => _dayKey ?? string.Empty;

    private static void ValidateDayKey(string? dayKey)
    {
        // TODO: Add additional validation if necessary (e.g., check for invalid characters, length constraints, etc.)

        IdentifierUtility.ValidateIdentifier(dayKey);

        // Allows nullable input, as null is handled by ValidateIdentifier above. Further validation
        // assumes non-null input.
        string[] parts = dayKey?.Split('_') ?? Array.Empty<string>();
        if (parts.Length != 2)
        {
            throw new ArgumentException("DayKey must match canonical format 'Year{N}_{Season}{D}'.", nameof(dayKey));
        }

        string yearPart = parts[0];
        string seasonDayPart = parts[1];

        if (!yearPart.StartsWith("Year", StringComparison.Ordinal))
        {
            throw new ArgumentException("DayKey year part must start with 'Year'.", nameof(dayKey));
        }

        string yearNumber = yearPart["Year".Length..];
        if (!int.TryParse(yearNumber, out int year) || year < 1)
        {
            throw new ArgumentException("DayKey year value must be a positive integer.", nameof(dayKey));
        }

        
        string[] allowedSeasons = { "Spring", "Summer", "Fall", "Winter" };

        string? matchedSeason = null;
        foreach (string season in allowedSeasons)
        {
            if (seasonDayPart.StartsWith(season, StringComparison.Ordinal))
            {
                matchedSeason = season;
                break;
            }
        }

        if (matchedSeason == null)
        {
            throw new ArgumentException($"DayKey season must be one of: {string.Join(", ", allowedSeasons)}.", nameof(dayKey));
        }

        string dayText = seasonDayPart[matchedSeason.Length..];
        if (!int.TryParse(dayText, out int day) || day < 1 || day > 28)
        {
            throw new ArgumentException("DayKey day value must be an integer between 1 and 28.", nameof(dayKey));
        }
    }
}
