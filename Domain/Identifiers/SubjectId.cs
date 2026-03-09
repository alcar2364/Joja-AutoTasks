namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>Represents a canonical, immutable identifier for a task subject.</summary>
internal readonly struct SubjectId : IEquatable<SubjectId>
{
    private static readonly StringComparer Comparer = StringComparer.Ordinal;
    private readonly string _subjectId;

    public string Value => _subjectId ?? string.Empty;

    /// <summary>Initializes a new <see cref="SubjectId"/> from a raw identifier string.</summary>
    /// <param name="subjectId">The raw subject identifier.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="subjectId"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="subjectId"/> is empty or whitespace after normalization.
    /// </exception>
    public SubjectId(string subjectId)
    {
        string normalizedSubjectId = IdentifierUtility.NormalizeIdentifier(subjectId);
        IdentifierUtility.ValidateIdentifier(normalizedSubjectId);
        _subjectId = normalizedSubjectId;
    }

    public bool Equals(SubjectId other) => Comparer.Equals(_subjectId, other._subjectId);

    public override bool Equals(object? obj) => obj is SubjectId other && Equals(other);

    public override int GetHashCode() => Comparer.GetHashCode(_subjectId ?? string.Empty);

    public static bool operator ==(SubjectId left, SubjectId right) => left.Equals(right);

    public static bool operator !=(SubjectId left, SubjectId right) => !left.Equals(right);

    public override string ToString() => _subjectId ?? string.Empty;
}
