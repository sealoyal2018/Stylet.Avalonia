using Stylet.Avalonia.StyletIoC.Creation;
using System;
using System.Linq.Expressions;
using System.Threading;

namespace Stylet.Avalonia.StyletIoC.Internal.Registrations;

/// <summary>
/// Registration which generates a single instance, and returns that instance thereafter
/// </summary>
internal class SingletonRegistration : RegistrationBase
{
    private readonly IRegistrationContext parentContext;
    private object instance;

    public SingletonRegistration(IRegistrationContext parentContext, ICreator creator)
        : base(creator)
    {
        this.parentContext = parentContext;
        this.parentContext.Disposing += (o, e) =>
        {
            IDisposable disposable;
            lock (lockObject)
            {
                disposable = instance as IDisposable;
                instance = null;
                generator = null;
            }
            if (disposable != null)
                disposable.Dispose();
        };
    }

    public override Expression GetInstanceExpression(ParameterExpression registrationContext)
    {
        if (instance == null)
        {
            lock (lockObject)
            {
                if (instance == null)
                    instance = Expression.Lambda<Func<IRegistrationContext, object>>(Creator.GetInstanceExpression(registrationContext), registrationContext).Compile()(parentContext);
            }
        }

        // This expression yields the actual type of instance, not 'object'
        var instanceExpression = Expression.Constant(instance);
        return instanceExpression;
    }
}
