using Stylet.Avalonia.StyletIoC.Creation;
using System;
using System.Linq.Expressions;

namespace Stylet.Avalonia.StyletIoC.Internal.Registrations;

/// <summary>
/// Knows how to create a Func{T}, using a given IRegistration
/// </summary>
// We're only created when we're needed, so no point in trying to be lazy
internal class FuncRegistration : IRegistration
{
    private readonly RuntimeTypeHandle funcType;
    private readonly Func<IRegistrationContext, object> generator;
    private readonly IRegistration delegateRegistration;

    public RuntimeTypeHandle TypeHandle
    {
        get { return funcType; }
    }

    public FuncRegistration(IRegistration delegateRegistration)
    {
        this.delegateRegistration = delegateRegistration;
        funcType = Expression.GetFuncType(Type.GetTypeFromHandle(delegateRegistration.TypeHandle)).TypeHandle;

        var registrationContext = Expression.Parameter(typeof(IRegistrationContext), "registrationContext");
        generator = Expression.Lambda<Func<IRegistrationContext, object>>(GetInstanceExpression(registrationContext), registrationContext).Compile();
    }

    public Func<IRegistrationContext, object> GetGenerator()
    {
        return generator;
    }

    public Expression GetInstanceExpression(ParameterExpression registrationContext)
    {
        return Expression.Lambda(delegateRegistration.GetInstanceExpression(registrationContext));
    }
}
