namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>Normalizes and validates canonical identifier strings.</summary>
internal static class IdentifierUtility
{
    /// <summary>Normalizes an identifier by trimming outer whitespace.</summary>
    /// <param name="identifier">The raw identifier string to normalize.</param>
    /// <returns>The normalized identifier string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="identifier"/> is null.</exception>
    internal static string NormalizeIdentifier(string? identifier)
    {
        return identifier == null
            ? throw new ArgumentNullException(nameof(identifier), $"{nameof(identifier)} cannot be null.")
            : identifier.Trim();
    }

    /// <summary>Validates that an identifier remains non-empty after normalization.</summary>
    /// <param name="identifier">The identifier string to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="identifier"/> is null, empty, or whitespace.
    /// </exception>
    internal static void ValidateIdentifier(string? identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException($"{nameof(identifier)} cannot be null or whitespace.", nameof(identifier));
        }
    }
}
