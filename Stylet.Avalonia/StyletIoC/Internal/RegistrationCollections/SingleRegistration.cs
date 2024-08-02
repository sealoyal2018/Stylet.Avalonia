using Stylet.Avalonia.StyletIoC.Creation;
using StyletIoC;
using System;
using System.Collections.Generic;

namespace Stylet.Avalonia.StyletIoC.Internal.RegistrationCollections;

internal class SingleRegistration : IRegistrationCollection
{
    private readonly IRegistration registration;

    public SingleRegistration(IRegistration registration)
    {
        this.registration = registration;
    }

    public IRegistration GetSingle()
    {
        return registration;
    }

    public List<IRegistration> GetAll()
    {
        return new List<IRegistration>() { registration };
    }

    public IRegistrationCollection AddRegistration(IRegistration registration)
    {
        if (this.registration.TypeHandle.Equals(registration.TypeHandle))
            throw new StyletIoCRegistrationException(string.Format("Multiple registrations for type {0} found.", Type.GetTypeFromHandle(registration.TypeHandle).GetDescription()));
        return new RegistrationCollection(new List<IRegistration>() { this.registration, registration });
    }
}
