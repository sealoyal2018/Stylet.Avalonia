using System.Collections.Generic;
using Stylet.Avalonia.StyletIoC.Creation;
using Stylet.Avalonia.StyletIoC.Internal.Registrations;

namespace Stylet.Avalonia.StyletIoC.Internal.Builders;

internal class BuilderInstanceBinding : BuilderBindingBase, IWithKeyOrAsWeakBindingOrDisposeWithContainer
{
    private readonly object instance;
    private bool disposeWithContainer = true;

    public BuilderInstanceBinding(List<BuilderTypeKey> serviceTypes, object instance)
        : base(serviceTypes)
    {
        EnsureTypeAgainstServiceTypes(instance.GetType(), assertImplementation: false);
        this.instance = instance;
    }

    public override void Build(Container container)
    {
        var registration = new InstanceRegistration(container, instance, disposeWithContainer);

        foreach (var serviceType in ServiceTypes)
        {
            container.AddRegistration(new TypeKey(serviceType.Type.TypeHandle, serviceType.Key), registration);
        }
    }

    public IWithKeyOrAsWeakBinding DisposeWithContainer(bool disposeWithContainer)
    {
        this.disposeWithContainer = disposeWithContainer;
        return this;
    }
}
