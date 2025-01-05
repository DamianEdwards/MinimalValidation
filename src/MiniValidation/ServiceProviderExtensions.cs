using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MiniValidation.Internal;

namespace MiniValidation;

/// <summary>
/// Extension methods for <see cref="IServiceProvider"/>.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Adds IValidator service to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMiniValidator(this IServiceCollection services)
    {
        services.AddSingleton<IMiniValidator, MiniValidatorImpl>();
        services.AddSingleton(typeof(IMiniValidator<>), typeof(MiniValidatorImpl<>));
        return services;
    }

    /// <summary>
    /// Adds a class based validator that implements <see cref="IValidate{T}"/> or <see cref="IAsyncValidate{T}"/> to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
    /// <typeparam name="TValidator">A class that implements <see cref="IValidate{T}"/> or <see cref="IAsyncValidate{T}"/></typeparam>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddClassMiniValidator<TValidator>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TValidator : class
    {
        var validators = typeof(TValidator)
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncValidate<>) || i.GetGenericTypeDefinition() == typeof(IValidate<>))
            .ToArray();
        
        foreach (var validator in validators)
        {
            services.Add(new ServiceDescriptor(validator, null, typeof(TValidator), lifetime));
        }
        
        return services;
    }
}