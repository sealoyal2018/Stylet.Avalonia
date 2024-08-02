using Stylet.Avalonia.StyletIoC.Creation;
using Stylet.Avalonia.StyletIoC.Internal.Creators;
using Stylet.Avalonia.StyletIoC.Internal.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stylet.Avalonia.StyletIoC.Internal.Builders;

internal abstract class BuilderBindingBase : IInScopeOrWithKeyOrAsWeakBinding, IWithKeyOrAsWeakBinding
{
    protected List<BuilderTypeKey> ServiceTypes { get; private set; }
    protected RegistrationFactory RegistrationFactory { get; set; }
    public bool IsWeak { get; protected set; }

    protected BuilderBindingBase(List<BuilderTypeKey> serviceTypes)
    {
        ServiceTypes = serviceTypes;

        // Default is transient
        RegistrationFactory = (ctx, services, creator) => new TransientRegistration(creator);
    }

    public IAsWeakBinding WithRegistrationFactory(RegistrationFactory registrationFactory)
    {
        if (registrationFactory == null)
            throw new ArgumentNullException("registrationFactory");
        RegistrationFactory = registrationFactory;
        return this;
    }

    /// <summary>
    /// Modify the scope of the binding to Singleton. One instance of this implementation will be generated for this binding.
    /// </summary>
    /// <returns>Fluent interface to continue configuration</returns>
    public IAsWeakBinding InSingletonScope()
    {
        return WithRegistrationFactory((ctx, serviceTypes, creator) => new SingletonRegistration(ctx, creator));
    }

    public IInScopeOrAsWeakBinding WithKey(string key)
    {
        foreach (var serviceType in ServiceTypes)
        {
            serviceType.Key = key;
        }
        return this;
    }

    protected void EnsureTypeAgainstServiceTypes(Type implementationType, bool assertImplementation = true)
    {
        foreach (var serviceType in ServiceTypes)
        {
            EnsureType(implementationType, serviceType.Type, assertImplementation);
        }
    }

    protected static void EnsureType(Type implementationType, Type serviceType, bool assertImplementation = true)
    {
        if (assertImplementation && (!implementationType.IsClass || implementationType.IsAbstract))
            throw new StyletIoCRegistrationException(string.Format("Type {0} is not a concrete class, and so can't be used to implemented service {1}", implementationType.GetDescription(), serviceType.GetDescription()));

        // Test this first, as it's a bit clearer than hitting 'type doesn't implement service'
        if (assertImplementation && implementationType.IsGenericTypeDefinition)
        {
            if (!serviceType.IsGenericTypeDefinition)
                throw new StyletIoCRegistrationException(string.Format("You can't use an unbound generic type to implement anything that isn't an unbound generic service. Service: {0}, Type: {1}", serviceType.GetDescription(), implementationType.GetDescription()));

            // This restriction may change when I figure out how to pass down the correct type argument
            if (serviceType.GetTypeInfo().GenericTypeParameters.Length != implementationType.GetTypeInfo().GenericTypeParameters.Length)
                throw new StyletIoCRegistrationException(string.Format("If you're registering an unbound generic type to an unbound generic service, both service and type must have the same number of type parameters. Service: {0}, Type: {1}", serviceType.GetDescription(), implementationType.GetDescription()));
        }
        else if (serviceType.IsGenericTypeDefinition)
        {
            if (implementationType.GetGenericArguments().Length > 0)
                throw new StyletIoCRegistrationException(string.Format("You cannot bind the bound generic type {0} to the unbound generic service {1}", implementationType.GetDescription(), serviceType.GetDescription()));
            else
                throw new StyletIoCRegistrationException(string.Format("You cannot bind the non-generic type {0} to the unbound generic service {1}", implementationType.GetDescription(), serviceType.GetDescription()));
        }

        if (!implementationType.Implements(serviceType))
            throw new StyletIoCRegistrationException(string.Format("Type {0} does not implement service {1}", implementationType.GetDescription(), serviceType.GetDescription()));
    }

    protected void BindImplementationToServices(Container container, Type implementationType)
    {
        if (ServiceTypes.Count > 1)
        {
            var firstGenericType = ServiceTypes.FirstOrDefault(x => x.Type.IsGenericTypeDefinition);

            if (firstGenericType != null)
                throw new StyletIoCRegistrationException(string.Format("Cannot create a multiple-service binding with an unbound generic type {0}", firstGenericType.Type.GetDescription()));

            var creator = new TypeCreator(implementationType, container);
            var registration = CreateRegistration(container, creator);

            foreach (var serviceType in ServiceTypes)
            {
                container.AddRegistration(new TypeKey(serviceType.Type.TypeHandle, serviceType.Key ?? creator.AttributeKey), registration);
            }
        }
        else
        {
            BindImplementationToSpecificService(container, implementationType, ServiceTypes[0].Type, ServiceTypes[0].Key);
        }
    }

    // Convenience...
    protected void BindImplementationToSpecificService(Container container, Type implementationType, Type serviceType, string key)
    {
        if (serviceType.IsGenericTypeDefinition)
        {
            var unboundGeneric = new UnboundGeneric(serviceType, implementationType, container, RegistrationFactory);
            container.AddUnboundGeneric(new TypeKey(serviceType.TypeHandle, key), unboundGeneric);
        }
        else
        {
            var creator = new TypeCreator(implementationType, container);
            var registration = CreateRegistration(container, creator);

            container.AddRegistration(new TypeKey(serviceType.TypeHandle, key ?? creator.AttributeKey), registration);
        }
    }

    // Convenience...
    protected IRegistration CreateRegistration(IRegistrationContext registrationContext, ICreator creator)
    {
        return RegistrationFactory(registrationContext, ServiceTypes, creator);
    }

    IAsWeakBinding IWithKeyOrAsWeakBinding.WithKey(string key)
    {
        ServiceTypes[ServiceTypes.Count - 1].Key = key;
        return this;
    }

    void IAsWeakBinding.AsWeakBinding()
    {
        IsWeak = true;
    }

    public abstract void Build(Container container);
}
