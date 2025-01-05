using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniValidation;

/// <summary>
/// Provides a way to add a validator for a type outside the class.
/// </summary>
/// <typeparam name="T">The type to validate.</typeparam>
public interface IValidate<in T>
{
    /// <summary>
    /// Determines whether the specified object is valid.
    /// </summary>
    /// <param name="instance">The object instance to validate.</param>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection that holds failed-validation information.</returns>
    IEnumerable<ValidationResult> Validate(T instance, ValidationContext validationContext);
}