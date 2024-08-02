using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Stylet.Avalonia.StyletIoC.Creation;

namespace Stylet.Avalonia.StyletIoC.Internal.Builders;

internal class BuilderToAllImplementationsBinding : BuilderBindingBase
{
    private readonly IEnumerable<Assembly> assemblies;
    private readonly bool allowZeroImplementations;

    private BuilderTypeKey ServiceType { get { return ServiceTypes[0]; } }

    public BuilderToAllImplementationsBinding(List<BuilderTypeKey> serviceTypes, IEnumerable<Assembly> assemblies, bool allowZeroImplementations)
        : base(serviceTypes)
    {
        // This should be ensured by the fluent interfaces
        Trace.Assert(ServiceTypes.Count == 1);

        this.assemblies = assemblies;
        this.allowZeroImplementations = allowZeroImplementations;
    }

    public override void Build(Container container)
    {
        var candidates = (from type in assemblies.Distinct().SelectMany(x => x.GetTypes())
                          let baseType = type.GetBaseTypesAndInterfaces().FirstOrDefault(x => x == ServiceType.Type || x.IsGenericType && x.GetGenericTypeDefinition() == ServiceType.Type)
                          where baseType != null
                          select new { Type = type, Base = baseType.ContainsGenericParameters ? baseType.GetGenericTypeDefinition() : baseType }).ToList();

        if (!allowZeroImplementations && candidates.Count == 0)
        {
            throw new StyletIoCRegistrationException(string.Format("Did not find any implementations of the type {0}", ServiceType.Type));
        }

        foreach (var candidate in candidates)
        {
            try
            {
                EnsureType(candidate.Type, candidate.Base);
                BindImplementationToSpecificService(container, candidate.Type, candidate.Base, ServiceType.Key);
            }
            catch (StyletIoCRegistrationException e)
            {
                Debug.WriteLine(string.Format("Unable to auto-bind type {0} to {1}: {2}", candidate.Base.Name, candidate.Type.GetDescription(), e.Message), "StyletIoC");
            }
        }
    }
}
