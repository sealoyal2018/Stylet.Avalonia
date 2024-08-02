using Stylet.Avalonia.StyletIoC.Creation;
using System;
using System.Linq.Expressions;

namespace Stylet.Avalonia.StyletIoC.Internal.Registrations;

/// <summary>
/// Convenience base class for all IRegistrations which want it
/// </summary>
internal abstract class RegistrationBase : IRegistration
{
    protected readonly ICreator Creator;
    public RuntimeTypeHandle TypeHandle { get { return Creator.TypeHandle; } }

    protected readonly object lockObject = new object();
    protected Func<IRegistrationContext, object> generator;

    protected RegistrationBase(ICreator creator)
    {
        Creator = creator;
    }

    public virtual Func<IRegistrationContext, object> GetGenerator()
    {
        if (generator != null)
            return generator;

        lock (lockObject)
        {
            if (generator == null)
                generator = GetGeneratorInternal();
            return generator;
        }
    }

    protected virtual Func<IRegistrationContext, object> GetGeneratorInternal()
    {
        var registrationContext = Expression.Parameter(typeof(IRegistrationContext), "registrationContext");
        return Expression.Lambda<Func<IRegistrationContext, object>>(GetInstanceExpression(registrationContext), registrationContext).Compile();
    }

    public abstract Expression GetInstanceExpression(ParameterExpression registrationContext);
}
