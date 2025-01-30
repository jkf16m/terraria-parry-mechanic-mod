using System.Collections.Generic;
using System;

public static class DIService
{
    private static readonly Dictionary<Type, object> _services = new();

    /// <summary>
    /// Registers a service instance.
    /// </summary>
    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
            throw new InvalidOperationException($"Service of type {type} is already registered.");

        _services[type] = service;
    }

    /// <summary>
    /// Resolves a service instance.
    /// </summary>
    public static T Resolve<T>() where T : class
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
            return service as T;

        throw new InvalidOperationException($"Service of type {type} is not registered.");
    }

    /// <summary>
    /// Clears all registered services.
    /// </summary>
    public static void Clear()
    {
        _services.Clear();
    }
}