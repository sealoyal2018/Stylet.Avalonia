using Stylet.Avalonia.StyletIoC.Creation;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Stylet.Avalonia.StyletIoC.Internal.Creators;

/// <summary>
/// Creator which knows how to create an instance of a type, by finding a suitable constructor and calling it
/// </summary>
// Sealed so Code Analysis doesn't moan about us setting the virtual Type property
internal sealed class TypeCreator : CreatorBase
{
    private readonly string _attributeKey;
    public string AttributeKey
    {
        get { return _attributeKey; }
    }
    private Expression creationExpression;

    public TypeCreator(Type type, IRegistrationContext parentContext)
        : base(parentContext)
    {
        TypeHandle = type.TypeHandle;

        // Use the key from InjectAttribute (if present), and let someone else override it if they want
        var attribute = type.GetCustomAttribute<InjectAttribute>(true);
        if (attribute != null)
            _attributeKey = attribute.Key;
    }

    private string KeyForParameter(ParameterInfo parameter)
    {
        var attribute = parameter.GetCustomAttribute<InjectAttribute>(true);
        return attribute == null ? null : attribute.Key;
    }

    [SuppressMessage("StyleCop.CSharp.Readability", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Honestly, it's clearer like this")]
    public override Expression GetInstanceExpression(ParameterExpression registrationContext)
    {
        if (creationExpression != null)
            return creationExpression;

        var type = Type.GetTypeFromHandle(TypeHandle);

        // Find the constructor which has the most parameters which we can fulfill, accepting default values which we can't fulfill
        ConstructorInfo ctor;
        var ctorsWithAttribute = type.GetConstructors().Where(x => x.GetCustomAttribute<InjectAttribute>(true) != null).ToList();
        if (ctorsWithAttribute.Count > 1)
        {
            throw new StyletIoCFindConstructorException(string.Format("Found more than one constructor with [Inject] on type {0}.", type.GetDescription()));
        }
        else if (ctorsWithAttribute.Count == 1)
        {
            ctor = ctorsWithAttribute[0];
            var key = ctorsWithAttribute[0].GetCustomAttribute<InjectAttribute>(true).Key;
            var cantResolve = ctor.GetParameters().FirstOrDefault(p => !ParentContext.CanResolve(p.ParameterType, key) && !p.HasDefaultValue);
            if (cantResolve != null)
                throw new StyletIoCFindConstructorException(string.Format("Found a constructor with [Inject] on type {0}, but can't resolve parameter '{1}' (of type {2}, and doesn't have a default value).", type.GetDescription(), cantResolve.Name, cantResolve.ParameterType.GetDescription()));
        }
        else
        {
            // Since we don't look for recursive includes, do at least check for copy constructors
            ctor = type.GetConstructors()
                .Where(c => c.GetParameters().All(p => p.ParameterType != type && (ParentContext.CanResolve(p.ParameterType, KeyForParameter(p)) || p.HasDefaultValue)))
                .OrderByDescending(c => c.GetParameters().Count(p => !p.HasDefaultValue))
                .FirstOrDefault();

            if (ctor == null)
            {
                // Get us a bit more information....
                Func<ParameterInfo, string> ctorParameterPrinter = p =>
                {
                    var key = KeyForParameter(p);
                    var canResolve = p.ParameterType != type && (ParentContext.CanResolve(p.ParameterType, key) || p.HasDefaultValue);
                    var keyStr = key == null ? "" : string.Format(" [Key = {0}]", key);
                    var usingDefaultStr = !ParentContext.CanResolve(p.ParameterType, key) && p.HasDefaultValue ? " [Using Default]" : "";
                    var recursiveStr = p.ParameterType == type ? " [Recursive]" : "";
                    return string.Format("   {0}{1}: {2}{3}{4}", p.ParameterType.GetDescription(), keyStr, canResolve ? "Success" : "Failure", usingDefaultStr, recursiveStr);
                };

                var info = string.Join("\n\n", type.GetConstructors().Select(c => string.Format("Constructor:\n{0}\n\n", string.Join("\n", c.GetParameters().Select(ctorParameterPrinter)))));

                throw new StyletIoCFindConstructorException(string.Format("Unable to find a constructor for type {0} which we can call:\n{1}", type.GetDescription(), info));
            }
        }

        // If we get circular dependencies, we'll just blow the stack. They're a pain to resolve.

        // If there parameter's got an InjectAttribute with a key, use that key to resolve
        var ctorParams = ctor.GetParameters().Select(x =>
        {
            var key = KeyForParameter(x);
            if (ParentContext.CanResolve(x.ParameterType, key))
            {
                try
                {
                    return ParentContext.GetSingleRegistration(x.ParameterType, key, true).GetInstanceExpression(registrationContext);
                }
                catch (StyletIoCRegistrationException e)
                {
                    throw new StyletIoCRegistrationException(string.Format("{0} Required by parameter '{1}' of type {2} (which is a {3}).", e.Message, x.Name, type.GetDescription(), x.ParameterType.GetDescription()), e);
                }
            }
            // For some reason we need this cast...
            return Expression.Convert(Expression.Constant(x.DefaultValue), x.ParameterType);
        });

        var creator = Expression.New(ctor, ctorParams);

        var completeExpression = CompleteExpressionFromCreator(creator, registrationContext);

        if (StyletIoCContainer.CacheGeneratedExpressions)
        {
            Interlocked.CompareExchange(ref creationExpression, completeExpression, null);
            return creationExpression;
        }
        else
        {
            return completeExpression;
        }
    }
}
