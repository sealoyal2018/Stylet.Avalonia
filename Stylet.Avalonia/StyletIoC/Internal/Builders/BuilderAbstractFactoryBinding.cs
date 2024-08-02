using System.Collections.Generic;
using System.Diagnostics;
using Stylet.Avalonia.StyletIoC.Creation;
using Stylet.Avalonia.StyletIoC.Internal.Creators;
using Stylet.Avalonia.StyletIoC.Internal.Registrations;

namespace Stylet.Avalonia.StyletIoC.Internal.Builders;

internal class BuilderAbstractFactoryBinding : BuilderBindingBase
{
    private BuilderTypeKey ServiceType { get { return ServiceTypes[0]; } }

    public BuilderAbstractFactoryBinding(List<BuilderTypeKey> serviceTypes)
        : base(serviceTypes)
    {
        // This should be ensured by the fluent interfaces
        Trace.Assert(serviceTypes.Count == 1);

        if (ServiceType.Type.IsGenericTypeDefinition)
            throw new StyletIoCRegistrationException(string.Format("Unbound generic type {0} can't be used as an abstract factory", ServiceType.Type.GetDescription()));
    }

    public override void Build(Container container)
    {
        var factoryType = container.GetFactoryForType(ServiceType.Type);
        var creator = new AbstractFactoryCreator(factoryType);
        var registration = new TransientRegistration(creator);

        container.AddRegistration(new TypeKey(ServiceType.Type.TypeHandle, ServiceType.Key), registration);
    }
}
