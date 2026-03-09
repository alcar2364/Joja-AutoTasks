namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>Represents a canonical in-game day identifier.</summary>
internal readonly struct DayKey : IEquatable<DayKey>
{
    private static readonly StringComparer Comparer = StringComparer.Ordinal;
    private readonly string _dayKey;

    public string Value => _dayKey ?? string.Empty;

    /// <summary>Initializes a new <see cref="DayKey"/> from a raw identifier string.</summary>
    /// <param name="dayKey">The raw day key identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dayKey"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="dayKey"/> cannot be validated for format, season, or day value.
    /// </exception>
    public DayKey(string dayKey)
    {
        string normalizedDayKey = IdentifierUtility.NormalizeIdentifier(dayKey);
        ValidateDayKey(normalizedDayKey);
        _dayKey = normalizedDayKey;
    }

    public bool Equals(DayKey other) => Comparer.Equals(_dayKey, other._dayKey);

    public override bool Equals(object? obj) => obj is DayKey other && Equals(other);

    public override int GetHashCode() => Comparer.GetHashCode(_dayKey ?? string.Empty);

    public static bool operator ==(DayKey left, DayKey right) => left.Equals(right);

    public static bool operator !=(DayKey left, DayKey right) => !left.Equals(right);

    public override string ToString() => _dayKey ?? string.Empty;

    private static void ValidateDayKey(string? dayKey)
    {
        // -- Guards -- //
        IdentifierUtility.ValidateIdentifier(dayKey);

        // Canonical day keys keep persistence and equality checks stable across save/load boundaries.
        string[] parts = dayKey?.Split('-') ?? Array.Empty<string>();
        if (parts.Length != 2)
        {
            throw new ArgumentException(
                "DayKey must match canonical format 'Year{N}-{Season}{D}' (example: 'Year1-Summer15').",
                nameof(dayKey));
        }

        string yearPart = parts[0];
        string seasonDayPart = parts[1];

        const string YearPrefix = "Year";
        if (!yearPart.StartsWith(YearPrefix, StringComparison.Ordinal))
        {
            throw new ArgumentException("DayKey must start with 'Year'.", nameof(dayKey));
        }

        string yearNumberStr = yearPart[YearPrefix.Length..];
        if (!int.TryParse(yearNumberStr, out int year) || year < 1)
        {
            throw new ArgumentException("DayKey year value must be a positive integer.", nameof(dayKey));
        }

        string[] allowedSeasons = { "Spring", "Summer", "Fall", "Winter" };
        string? matchedSeason = null;
        string? dayPartStr = null;

        foreach (string season in allowedSeasons)
        {
            if (seasonDayPart.StartsWith(season, StringComparison.Ordinal))
            {
                matchedSeason = season;
                dayPartStr = seasonDayPart[season.Length..];
                break;
            }
        }

        if (matchedSeason == null)
        {
            throw new ArgumentException(
                $"DayKey season must be one of: {string.Join(", ", allowedSeasons)}.",
                nameof(dayKey));
        }

        if (!int.TryParse(dayPartStr, out int day) || day < 1 || day > 28)
        {
            throw new ArgumentException("DayKey day value must be an integer between 1 and 28.", nameof(dayKey));
        }
    }
}
