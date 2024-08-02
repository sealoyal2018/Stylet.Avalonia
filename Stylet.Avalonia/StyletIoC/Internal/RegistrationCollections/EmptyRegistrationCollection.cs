using Stylet.Avalonia.StyletIoC.Creation;
using StyletIoC;
using System;
using System.Collections.Generic;

namespace Stylet.Avalonia.StyletIoC.Internal.RegistrationCollections;

internal class EmptyRegistrationCollection : IReadOnlyRegistrationCollection
{
    private readonly Type type;

    public EmptyRegistrationCollection(Type type)
    {
        this.type = type;
    }

    public IRegistration GetSingle()
    {
        throw new StyletIoCRegistrationException(string.Format("No registrations found for service {0}.", type.GetDescription()));
    }

    public List<IRegistration> GetAll()
    {
        return new List<IRegistration>();
    }
}
