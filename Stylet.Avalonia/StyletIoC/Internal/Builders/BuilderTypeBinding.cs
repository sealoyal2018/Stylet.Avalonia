using System;
using System.Collections.Generic;
using Stylet.Avalonia.StyletIoC.Creation;

namespace Stylet.Avalonia.StyletIoC.Internal.Builders;

internal class BuilderTypeBinding : BuilderBindingBase
{
    private readonly Type implementationType;

    public BuilderTypeBinding(List<BuilderTypeKey> serviceTypes, Type implementationType)
        : base(serviceTypes)
    {
        EnsureTypeAgainstServiceTypes(implementationType);
        this.implementationType = implementationType;
    }

    public override void Build(Container container)
    {
        BindImplementationToServices(container, implementationType);
    }
}
