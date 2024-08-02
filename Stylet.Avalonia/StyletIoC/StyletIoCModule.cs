using Stylet.Avalonia.StyletIoC.Internal.Builders;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stylet.Avalonia.StyletIoC;

/// <summary>
/// Module which contains its own bindings, and can be added to a builder
/// </summary>
public abstract class StyletIoCModule
{
    private StyletIoCBuilder builder;
    private Func<IEnumerable<Assembly>, string, IEnumerable<Assembly>> getAssemblies;

    /// <summary>
    /// Bind the specified service (interface, abstract class, concrete class, unbound generic, etc) to something
    /// </summary>
    /// <param name="serviceType">Service to bind</param>
    /// <returns>Fluent interface to continue configuration</returns>
    protected IBindTo Bind(Type serviceType)
    {
        if (builder == null || getAssemblies == null)
            throw new InvalidOperationException("Bind should only be called from inside Load, and you must not call Load yourself");

        var builderBindTo = new BuilderBindTo(serviceType, getAssemblies);
        builder.AddBinding(builderBindTo);
        return builderBindTo;
    }

    /// <summary>
    /// Bind the specified service (interface, abstract class, concrete class, unbound generic, etc) to something
    /// </summary>
    /// <typeparam name="TService">Service to bind</typeparam>
    /// <returns>Fluent interface to continue configuration</returns>
    protected IBindTo Bind<TService>()
    {
        return Bind(typeof(TService));
    }

    /// <summary>
    /// Override, and use 'Bind' to add bindings to the module
    /// </summary>
    protected abstract void Load();

    internal void AddToBuilder(StyletIoCBuilder builder, Func<IEnumerable<Assembly>, string, IEnumerable<Assembly>> getAssemblies)
    {
        this.builder = builder;
        this.getAssemblies = getAssemblies;

        Load();

        this.builder = null;
        this.getAssemblies = null;
    }
}
