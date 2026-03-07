// Purpose: Defines the immutable RuleID value type used for deterministic rule-based identifiers.

namespace JojaAutoTasks.Domain.Identifiers;

internal readonly struct RuleID : IEquatable<RuleID>
{
    private static readonly StringComparer Comparer = StringComparer.Ordinal;
    private readonly string _ruleID;

    /// <summary>
    /// Initializes a new <see cref="RuleID"/> from a raw identifier string.
    /// </summary>
    /// <param name="ruleID">The raw rule identifier.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="ruleID"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="ruleID"/> is null, empty or whitespace after normalization.
    /// </exception>
    public RuleID(string ruleID)
    {
        string normalizedRuleID = IdentifierUtility.NormalizeIdentifier(ruleID);
        IdentifierUtility.ValidateIdentifier(normalizedRuleID);
        _ruleID = normalizedRuleID;
    }

    public string Value => _ruleID ?? string.Empty;
    public bool Equals(RuleID other) => Comparer.Equals(_ruleID, other._ruleID);
    public override bool Equals(object? obj) => obj is RuleID other && Equals(other);
    public override int GetHashCode() => Comparer.GetHashCode(_ruleID ?? string.Empty);
    public static bool operator ==(RuleID left, RuleID right) => left.Equals(right);
    public static bool operator !=(RuleID left, RuleID right) => !left.Equals(right);
    public override string ToString() => _ruleID ?? string.Empty;

}
