using Stylet.Avalonia.StyletIoC.Creation;
using Stylet.Avalonia.StyletIoC.Internal.Creators;
using System;
using System.Collections.Generic;

namespace Stylet.Avalonia.StyletIoC.Internal;

internal class UnboundGeneric
{
    private readonly Type serviceType;
    private readonly IRegistrationContext parentContext;
    public Type Type { get; private set; }
    public RegistrationFactory RegistrationFactory { get; private set; }

    public UnboundGeneric(Type serviceType, Type type, IRegistrationContext parentContext, RegistrationFactory registrationFactory)
    {
        this.serviceType = serviceType;
        Type = type;
        this.parentContext = parentContext;
        RegistrationFactory = registrationFactory;
    }

    public IRegistration CreateRegistrationForTypeAndKey(Type boundType, string boundKey)
    {
        var serviceTypes = new List<BuilderTypeKey>() { new BuilderTypeKey(serviceType, boundKey) };
        return RegistrationFactory(parentContext, serviceTypes, new TypeCreator(boundType, parentContext));
    }
}
