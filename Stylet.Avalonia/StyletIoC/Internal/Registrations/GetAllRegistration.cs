using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Stylet.Avalonia.StyletIoC.Creation;
using System.Diagnostics;

namespace Stylet.Avalonia.StyletIoC.Internal.Registrations;

/// <summary>
/// Knows how to generate an IEnumerable{T}, which contains all implementations of T
/// </summary>
internal class GetAllRegistration : IRegistration
{
    private readonly IRegistrationContext parentContext;

    public string Key { get; private set; }
    private readonly RuntimeTypeHandle _type;
    public RuntimeTypeHandle TypeHandle
    {
        get { return _type; }
    }

    private Expression expression;
    private readonly object generatorLock = new object();
    private Func<IRegistrationContext, object> generator;

    public GetAllRegistration(RuntimeTypeHandle typeHandle, IRegistrationContext parentContext, string key)
    {
        Key = key;
        _type = typeHandle;
        this.parentContext = parentContext;
    }

    public Func<IRegistrationContext, object> GetGenerator()
    {
        if (generator != null)
            return generator;

        lock (generatorLock)
        {
            if (generator == null)
            {
                var registrationContext = Expression.Parameter(typeof(IRegistrationContext), "registrationContext");
                generator = Expression.Lambda<Func<IRegistrationContext, object>>(GetInstanceExpression(registrationContext), registrationContext).Compile();
            }
            return generator;
        }
    }

    public Expression GetInstanceExpression(ParameterExpression registrationContext)
    {
        if (expression != null)
            return expression;

        var type = Type.GetTypeFromHandle(TypeHandle);

        var instanceExpressions = parentContext.GetAllRegistrations(type.GenericTypeArguments[0], Key, false).Select(x => x.GetInstanceExpression(registrationContext)).ToArray();
        var listCtor = type.GetConstructor(new[] { typeof(int) }); // ctor which takes capacity
        Debug.Assert(listCtor != null);
        var listNew = Expression.New(listCtor, Expression.Constant(instanceExpressions.Length));
        Expression list = instanceExpressions.Any() ? Expression.ListInit(listNew, instanceExpressions) : listNew;

        if (StyletIoCContainer.CacheGeneratedExpressions)
        {
            Interlocked.CompareExchange(ref expression, list, null);
            return expression;
        }
        else
        {
            return list;
        }
    }
}
