// Purpose: Defines utility methods for identifier normalization and validation.
namespace JojaAutoTasks.Domain.Identifiers;

internal static class IdentifierUtility
{
    /// <summary>
    /// Validates null and normalizes the identifier by trimming whitespace. 
    /// Returns the normalized identifier.
    /// </summary>
    /// <param name="identifier">The raw identifier string to normalize.</param>
    /// <returns>The normalized identifier string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="identifier"/> is null.</exception>
    internal static string NormalizeIdentifier(string? identifier)
    {
        return identifier == null
            ? throw new ArgumentNullException(nameof(identifier), $"{nameof(identifier)} cannot be null.")
            : identifier.Trim();
    }

    /// <summary>
    /// Validates that the identifier is not null, empty, or whitespace.
    /// </summary>
    /// <param name="identifier">The identifier string to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="identifier"/> is null, empty, 
    /// or whitespace.
    /// </exception>
    /// <returns>void</returns>
    internal static void ValidateIdentifier(string? identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException($"{nameof(identifier)} cannot be null or whitespace.", nameof(identifier));
        }
    }
}
