using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniValidation.Internal;

internal class MiniValidatorImpl : IMiniValidator
{
    private readonly IServiceProvider _serviceProvider;

    public MiniValidatorImpl(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public bool RequiresValidation(Type targetType, bool recurse = true)
    {
        return MiniValidator.RequiresValidation(targetType, recurse, _serviceProvider);
    }

    public bool TryValidate<TTarget>(TTarget target, out IDictionary<string, string[]> errors)
    {
        return MiniValidator.TryValidate(target, _serviceProvider, out errors);
    }

    public bool TryValidate<TTarget>(TTarget target, bool recurse, out IDictionary<string, string[]> errors)
    {
        return MiniValidator.TryValidate(target, _serviceProvider, recurse, out errors);
    }

    public bool TryValidate<TTarget>(TTarget target, bool recurse, bool allowAsync, out IDictionary<string, string[]> errors)
    {
        return MiniValidator.TryValidate(target, _serviceProvider, recurse, allowAsync, out errors);
    }

#if NET6_0_OR_GREATER
    public ValueTask<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync<TTarget>(TTarget target)
#else
    public Task<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync<TTarget>(TTarget target)
#endif
    {
        return MiniValidator.TryValidateAsync(target, _serviceProvider);
    }

#if NET6_0_OR_GREATER
    public ValueTask<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync<TTarget>(TTarget target, bool recurse)
#else
    public Task<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync<TTarget>(TTarget target, bool recurse)
#endif
    {
        return MiniValidator.TryValidateAsync(target, _serviceProvider, recurse);
    }
    
    public IMiniValidator<TTarget> GetValidator<TTarget>()
    {
        return new MiniValidatorImpl<TTarget>(_serviceProvider);
    }
}

internal class MiniValidatorImpl<T> : IMiniValidator<T>
{
    private readonly IServiceProvider _serviceProvider;

    public MiniValidatorImpl(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool TryValidate(T target, out IDictionary<string, string[]> errors)
    {
        return MiniValidator.TryValidate(target, _serviceProvider, out errors);
    }

    public bool TryValidate(T target, bool recurse, out IDictionary<string, string[]> errors)
    {
        return MiniValidator.TryValidate(target, _serviceProvider, recurse, out errors);
    }

    public bool TryValidate(T target, bool recurse, bool allowAsync, out IDictionary<string, string[]> errors)
    {
        return MiniValidator.TryValidate(target, _serviceProvider, recurse, allowAsync, out errors);
    }

#if NET6_0_OR_GREATER
    public ValueTask<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync(T target)
#else
    public Task<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync(T target)
#endif
    {
        return MiniValidator.TryValidateAsync(target, _serviceProvider);
    }

#if NET6_0_OR_GREATER
    public ValueTask<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync(T target, bool recurse)
#else
    public Task<(bool IsValid, IDictionary<string, string[]> Errors)> TryValidateAsync(T target, bool recurse)
#endif
    {
        return MiniValidator.TryValidateAsync(target, _serviceProvider, recurse);
    }
}