﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MiniValidation;

/// <summary>
/// Provides a way for an object to be validated asynchronously.
/// </summary>
public interface IAsyncValidatableObject
{
    /// <summary>
    /// Determines whether the specified object is valid.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection that holds failed-validation information.</returns>
#if NET6_0_OR_GREATER
    ValueTask<IEnumerable<ValidationResult>> ValidateAsync(ValidationContext validationContext);
#else
    Task<IEnumerable<ValidationResult>> ValidateAsync(ValidationContext validationContext);
#endif
}
