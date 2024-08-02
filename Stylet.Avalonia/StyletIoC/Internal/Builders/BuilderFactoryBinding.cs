using Stylet.Avalonia.StyletIoC.Creation;
using Stylet.Avalonia.StyletIoC.Internal.Creators;
using System;
using System.Collections.Generic;

namespace Stylet.Avalonia.StyletIoC.Internal.Builders;

internal class BuilderFactoryBinding<TImplementation> : BuilderBindingBase
{
    private readonly Func<IRegistrationContext, TImplementation> factory;

    public BuilderFactoryBinding(List<BuilderTypeKey> serviceTypes, Func<IRegistrationContext, TImplementation> factory)
        : base(serviceTypes)
    {
        foreach (var serviceType in ServiceTypes)
        {
            if (serviceType.Type.IsGenericTypeDefinition)
                throw new StyletIoCRegistrationException(string.Format("A factory cannot be used to implement unbound generic type {0}", serviceType.Type.GetDescription()));
            EnsureTypeAgainstServiceTypes(typeof(TImplementation), assertImplementation: false);
        }
        this.factory = factory;
    }

    public override void Build(Container container)
    {
        var creator = new FactoryCreator<TImplementation>(factory, container);
        var registration = CreateRegistration(container, creator);

        foreach (var serviceType in ServiceTypes)
        {
            container.AddRegistration(new TypeKey(serviceType.Type.TypeHandle, serviceType.Key), registration);
        }
    }
}
