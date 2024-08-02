using Stylet.Avalonia.StyletIoC.Creation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Stylet.Avalonia.StyletIoC.Internal.Builders;

internal class BuilderBindTo : IBindTo, IAndOrToMultipleServices
{
    private readonly Func<IEnumerable<Assembly>, string, IEnumerable<Assembly>> getAssemblies;
    public List<BuilderTypeKey> ServiceTypes { get; private set; }
    private BuilderBindingBase builderBinding;
    public bool IsWeak { get { return builderBinding?.IsWeak ?? false; } }

    public BuilderBindTo(Type serviceType, Func<IEnumerable<Assembly>, string, IEnumerable<Assembly>> getAssemblies)
    {
        ServiceTypes = new List<BuilderTypeKey>() { new BuilderTypeKey(serviceType) };
        this.getAssemblies = getAssemblies;
    }

    public IWithKeyOrAndOrToMultipleServices And<TService>()
    {
        return And(typeof(TService));
    }

    public IWithKeyOrAndOrToMultipleServices And(Type serviceType)
    {
        ServiceTypes.Add(new BuilderTypeKey(serviceType));
        return this;
    }

    public IAndOrToMultipleServices WithKey(string key)
    {
        // Should have been ensured by the fluent interface
        Trace.Assert(ServiceTypes.Count > 0);

        ServiceTypes[ServiceTypes.Count - 1].Key = key;
        return this;
    }

    public IInScopeOrWithKeyOrAsWeakBinding ToSelf()
    {
        // This should be ensured by the fluent interfaces
        Trace.Assert(ServiceTypes.Count == 1);

        return To(ServiceTypes[0].Type);
    }

    public IInScopeOrWithKeyOrAsWeakBinding To(Type implementationType)
    {
        builderBinding = new BuilderTypeBinding(ServiceTypes, implementationType);
        return builderBinding;
    }

    public IInScopeOrWithKeyOrAsWeakBinding To<TImplementation>()
    {
        return To(typeof(TImplementation));
    }

    public IInScopeOrWithKeyOrAsWeakBinding ToFactory<TImplementation>(Func<IRegistrationContext, TImplementation> factory)
    {
        builderBinding = new BuilderFactoryBinding<TImplementation>(ServiceTypes, factory);
        return builderBinding;
    }

    public IWithKeyOrAsWeakBindingOrDisposeWithContainer ToInstance(object instance)
    {
        var builderBinding = new BuilderInstanceBinding(ServiceTypes, instance);
        this.builderBinding = builderBinding;
        return builderBinding;
    }

    public IWithKeyOrAsWeakBinding ToAbstractFactory()
    {
        builderBinding = new BuilderAbstractFactoryBinding(ServiceTypes);
        return builderBinding;
    }

    public IInScopeOrWithKeyOrAsWeakBinding ToAllImplementations(IEnumerable<Assembly> assemblies, bool allowZeroImplementations = false)
    {
        builderBinding = new BuilderToAllImplementationsBinding(ServiceTypes, getAssemblies(assemblies, "ToAllImplementations"), allowZeroImplementations);
        return builderBinding;
    }

    public IInScopeOrWithKeyOrAsWeakBinding ToAllImplementations(bool allowZeroImplementations = false, params Assembly[] assemblies)
    {
        return ToAllImplementations(assemblies.AsEnumerable(), allowZeroImplementations);
    }

    internal void Build(Container container)
    {
        if (builderBinding == null)
            throw new StyletIoCRegistrationException(string.Format("Service type {0} is not bound to anything", ServiceTypes[0].Type.GetDescription()));

        builderBinding.Build(container);
    }
}
