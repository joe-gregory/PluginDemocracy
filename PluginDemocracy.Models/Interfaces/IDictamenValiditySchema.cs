using System;

/// <summary>
/// Interface for a strategy to validate Dictamen objects.
/// </summary>
public interface IDictamenValidityStrategy
{
    /// <summary>
    /// Determines whether the provided IDictamen instance is valid.
    /// </summary>
    /// <param name="dictamen">The IDictamen instance to validate.</param>
    /// <returns>true if the instance is valid; otherwise, false.</returns>
    bool IsValid(IDictamen dictamen);
}
