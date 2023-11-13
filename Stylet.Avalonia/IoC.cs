using System;
using System.Collections.Generic;
using System.Linq;

namespace Stylet.Avalonia;

public static class IoC
{
    public static Func<Type, string?, object> GetInstance = (service, key) => throw new InvalidOperationException("IoC not initailized");
    
    public static Func<Type, IEnumerable<object>> GetInstances = (service) => throw new InvalidOperationException("IoC not initailized");


    public static T Get<T>(string? key = null)
    {
        return (T)GetInstance(typeof(T), key);
    }

    public static IEnumerable<T> GetAll<T>()
    {
        return GetInstances(typeof(T)).Cast<T>();
    }
}