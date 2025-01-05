using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniValidation;

/// <summary>
/// Represents a validator that can validate an object.
/// </summary>
public interface IMiniValidator
{
    /// <summary>
    /// Determines if the specified <see cref="Type"/> has anything to validate.
    /// </summary>
    /// <remarks>
    /// Objects of types with nothing to validate will always return <c>true</c> when passed to <see cref="TryValidate{TTarget}(TTarget, bool, out IDictionary{string, string[]})"/>.
    /// </remarks>
    /// <param name="targetType">The <see cref="Type"/>.</param>
    /// <param name="recurse"><c>true</c> to recursively check descendant types; if <c>false</c> only simple values directly on the target type are checked.</param>
    /// <returns><c>true</c> if <paramref name="targetType"/> has anything to validate, <c>false</c> if not.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetType"/> is <c>null</c>.</exception>
    bool RequiresValidation(Type targetType, bool recurse = true);

    /// <summary>
    /// Determines whether the specific object is valid. This method recursively validates descendant objects.
    /// </summary>
    /// <param name="target">The object to validate.</param>
    /// <param name="errors">A dictionary that contains details of each failed validation.</param>
    /// <returns><c>true</c> if <paramref name="target"/> is valid; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
    bool TryValidate<TTarget>(TTarget target, out IDictionary<string, string[]> errors);

    /// <summary>
    /// Determines whether the specific object is valid.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target of validation.</typeparam>
    /// <param name="target">The object to validate.</param>
    /// <param name="recurse"><c>true</c> to recursively validate descendant objects; if <c>false</c> only simple values directly on <paramref name="target"/> are validated.</param>
    /// <param name="errors">A dictionary that contains details of each failed validation.</param>
    /// <returns><c>true</c> if <paramref name="target"/> is valid; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
    bool TryValidate<TTarget>(TTarget target, bool recurse, out IDictionary<string, string[]> errors);

    /// <summary>
    /// Determines whether the specific object is valid.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="target">The object to validate.</param>
    /// <param name="recurse"><c>true</c> to recursively validate descendant objects; if <c>false</c> only simple values directly on <paramref name="target"/> are validated.</param>
    /// <param name="allowAsync"><c>true</c> to allow asynchronous validation if an object in the graph requires it.</param>
    /// <param name="errors">A dictionary that contains details of each failed validation.</param>
    /// <returns><c>true</c> if <paramref name="target"/> is valid; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Throw when <paramref name="target"/> requires async validation and <paramref name="allowAsync"/> is <c>false</c>.</exception>
    bool TryValidate<TTarget>(TTarget target, bool recurse, bool allowAsync, out IDictionary<string, string[]> errors);

    /// <summary>
    /// Determines whether the specific object is valid.
    /// </summary>
    /// <param name="target">The object to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
#if NET6_0_OR_GREATER
    ValueTask<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync<TTarget>(TTarget target);
#else
    Task<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync<TTarget>(TTarget target);
#endif

    /// <summary>
    /// Determines whether the specific object is valid.
    /// </summary>
    /// <param name="target">The object to validate.</param>
    /// <param name="recurse"><c>true</c> to recursively validate descendant objects; if <c>false</c> only simple values directly on <paramref name="target"/> are validated.</param>
    /// <returns><c>true</c> if <paramref name="target"/> is valid; otherwise <c>false</c> and the validation errors.</returns>
    /// <exception cref="ArgumentNullException"></exception>
#if NET6_0_OR_GREATER
    ValueTask<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync<TTarget>(TTarget target, bool recurse);
#else
    Task<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync<TTarget>(TTarget target, bool recurse);
#endif
    
    /// <summary>
    /// Gets a validator for the specified target type.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns></returns>
    IMiniValidator<TTarget> GetValidator<TTarget>();
}

/// <summary>
/// Represents a validator that can validate an object of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMiniValidator<in T>
{

    /// <summary>
    /// Determines whether the specific object is valid. This method recursively validates descendant objects.
    /// </summary>
    /// <param name="target">The object to validate.</param>
    /// <param name="errors">A dictionary that contains details of each failed validation.</param>
    /// <returns><c>true</c> if <paramref name="target"/> is valid; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
    bool TryValidate(T target, out IDictionary<string, string[]> errors);

    /// <summary>
    /// Determines whether the specific object is valid.
    /// </summary>
    /// <param name="target">The object to validate.</param>
    /// <param name="recurse"><c>true</c> to recursively validate descendant objects; if <c>false</c> only simple values directly on <paramref name="target"/> are validated.</param>
    /// <param name="errors">A dictionary that contains details of each failed validation.</param>
    /// <returns><c>true</c> if <paramref name="target"/> is valid; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
    bool TryValidate(T target, bool recurse, out IDictionary<string, string[]> errors);

    /// <summary>
    /// Determines whether the specific object is valid.
    /// </summary>
    /// <param name="target">The object to validate.</param>
    /// <param name="recurse"><c>true</c> to recursively validate descendant objects; if <c>false</c> only simple values directly on <paramref name="target"/> are validated.</param>
    /// <param name="allowAsync"><c>true</c> to allow asynchronous validation if an object in the graph requires it.</param>
    /// <param name="errors">A dictionary that contains details of each failed validation.</param>
    /// <returns><c>true</c> if <paramref name="target"/> is valid; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Throw when <paramref name="target"/> requires async validation and <paramref name="allowAsync"/> is <c>false</c>.</exception>
    bool TryValidate(T target, bool recurse, bool allowAsync, out IDictionary<string, string[]> errors);

    /// <summary>
    /// Determines whether the specific object is valid.
    /// </summary>
    /// <param name="target">The object to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
#if NET6_0_OR_GREATER
    ValueTask<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync(T target);
#else
    Task<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync(T target);
#endif

    /// <summary>
    /// Determines whether the specific object is valid.
    /// </summary>
    /// <param name="target">The object to validate.</param>
    /// <param name="recurse"><c>true</c> to recursively validate descendant objects; if <c>false</c> only simple values directly on <paramref name="target"/> are validated.</param>
    /// <returns><c>true</c> if <paramref name="target"/> is valid; otherwise <c>false</c> and the validation errors.</returns>
    /// <exception cref="ArgumentNullException"></exception>
#if NET6_0_OR_GREATER
    ValueTask<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync(T target, bool recurse);
#else
    Task<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync(T target, bool recurse);
#endif
}