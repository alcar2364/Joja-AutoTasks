// Purpose: Defines the immutable RuleID value type used for deterministic rule-based identifiers.

namespace JojaAutoTasks.Domain.Identifiers;

internal readonly struct RuleId : IEquatable<RuleId>
{
    private static readonly StringComparer Comparer = StringComparer.Ordinal;
    private readonly string _ruleId;

    /// <summary>
    /// Initializes a new <see cref="RuleId"/> from a raw identifier string.
    /// </summary>
    /// <param name="ruleId">The raw rule identifier.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="ruleId"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="ruleId"/> is null, empty or whitespace after normalization.
    /// </exception>
    public RuleId(string ruleId)
    {
        string normalizedRuleId = IdentifierUtility.NormalizeIdentifier(ruleId);
        IdentifierUtility.ValidateIdentifier(normalizedRuleId);
        _ruleId = normalizedRuleId;
    }

    public string Value => _ruleId ?? string.Empty;
    public bool Equals(RuleId other) => Comparer.Equals(_ruleId, other._ruleId);
    public override bool Equals(object? obj) => obj is RuleId other && Equals(other);
    public override int GetHashCode() => Comparer.GetHashCode(_ruleId ?? string.Empty);
    public static bool operator ==(RuleId left, RuleId right) => left.Equals(right);
    public static bool operator !=(RuleId left, RuleId right) => !left.Equals(right);
    public override string ToString() => _ruleId ?? string.Empty;

}
