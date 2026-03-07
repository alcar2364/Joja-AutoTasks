using System;
using System.Collections.Generic;


namespace JojaAutoTasks.Domain.Identifiers;

internal readonly struct SubjectID : IEquatable<SubjectID>
{
    private static readonly StringComparer Comparer = StringComparer.Ordinal;
    private readonly string _subjectID;

    /// <summary>
    /// Initializes a new <see cref="SubjectID"/> from a raw identifier string.
    /// Initializes a new <see cref="SubjectID"/> from a raw identifier string.
    /// </summary>
    /// <param name="subjectID">The raw subject identifier.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="subjectID"/> is null, empty, or consists only of whitespace after normalization.
    /// </exception>
    public SubjectID(string subjectID)
    {
        string normalizedSubjectID = IdentifierUtility.NormalizeIdentifier(subjectID);
        IdentifierUtility.ValidateIdentifier(normalizedSubjectID);
        _subjectID = normalizedSubjectID;
    }

    public string Value => _subjectID ?? string.Empty;

    public bool Equals(SubjectID other) => Comparer.Equals(_subjectID, other._subjectID);

    public override bool Equals(object? obj) => obj is SubjectID other && Equals(other);

    public override int GetHashCode() => Comparer.GetHashCode(_subjectID ?? string.Empty);
    public static bool operator ==(SubjectID left, SubjectID right) => left.Equals(right);
    public static bool operator !=(SubjectID left, SubjectID right) => !left.Equals(right);
    public override string ToString() => _subjectID ?? string.Empty;
}
