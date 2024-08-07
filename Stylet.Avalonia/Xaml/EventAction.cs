﻿using System;
using System.Reflection;
using Avalonia;
using Stylet.Avalonia.Logging;

namespace Stylet.Avalonia.Xaml;

/// <summary>
/// Created by ActionExtension, this can return a delegate suitable adding binding to an event, and can call a method on the View.ActionTarget
/// </summary>
public class EventAction : ActionBase
{
    private static readonly ILogger logger = LogManager.GetLogger(typeof(EventAction));
    private static readonly MethodInfo[] invokeCommandMethodInfos = new[]
    {
        typeof(EventAction).GetMethod("InvokeEventArgsCommand", BindingFlags.NonPublic | BindingFlags.Instance),
        typeof(EventAction).GetMethod("InvokeDependencyCommand", BindingFlags.NonPublic | BindingFlags.Instance),
    };

    /// <summary>
    /// Type of event handler
    /// </summary>
    private readonly Type eventHandlerType;

    /// <summary>
    /// Initialises a new instance of the <see cref="EventAction"/> classto use <see cref="View.ActionTargetProperty"/> to get the target
    /// </summary>
    /// <param name="subject">View whose View.ActionTarget we watch</param>
    /// <param name="backupSubject">Backup subject to use if no ActionTarget could be retrieved from the subject</param>
    /// <param name="eventHandlerType">Type of event handler we're returning a delegate for</param>
    /// <param name="methodName">The MyMethod in {s:Action MyMethod}, this is what we call when the event's fired</param>
    /// <param name="targetNullBehaviour">Behaviour for it the relevant View.ActionTarget is null</param>
    /// <param name="actionNonExistentBehaviour">Behaviour for if the action doesn't exist on the View.ActionTarget</param>
    public EventAction(AvaloniaObject subject, AvaloniaObject backupSubject, Type eventHandlerType, string methodName, ActionUnavailableBehaviour targetNullBehaviour, ActionUnavailableBehaviour actionNonExistentBehaviour)
        : base(subject, backupSubject, methodName, targetNullBehaviour, actionNonExistentBehaviour, logger)
    {
        AssertBehaviours(targetNullBehaviour, actionNonExistentBehaviour);
        this.eventHandlerType = eventHandlerType;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="EventAction"/> class to use an explicit target
    /// </summary>
    /// <param name="target">Target to find the method on</param>
    /// <param name="eventHandlerType">Type of event handler we're returning a delegate for</param>
    /// <param name="methodName">The MyMethod in {s:Action MyMethod}, this is what we call when the event's fired</param>
    /// <param name="targetNullBehaviour">Behaviour for it the relevant View.ActionTarget is null</param>
    /// <param name="actionNonExistentBehaviour">Behaviour for if the action doesn't exist on the View.ActionTarget</param>
    public EventAction(object target, Type eventHandlerType, string methodName, ActionUnavailableBehaviour targetNullBehaviour, ActionUnavailableBehaviour actionNonExistentBehaviour)
        : base(target, methodName, targetNullBehaviour, actionNonExistentBehaviour, logger)
    {
        AssertBehaviours(targetNullBehaviour, actionNonExistentBehaviour);
        this.eventHandlerType = eventHandlerType;
    }

    private static void AssertBehaviours(ActionUnavailableBehaviour targetNullBehaviour, ActionUnavailableBehaviour actionNonExistentBehaviour)
    {
        if (targetNullBehaviour == ActionUnavailableBehaviour.Disable)
            throw new ArgumentException("Setting NullTarget = Disable is unsupported when used on an Event");
        if (actionNonExistentBehaviour == ActionUnavailableBehaviour.Disable)
            throw new ArgumentException("Setting ActionNotFound = Disable is unsupported when used on an Event");
    }

    /// <summary>
    /// Invoked when a new non-null target is set, which has non-null MethodInfo. Used to assert that the method signature is correct
    /// </summary>
    /// <param name="targetMethodInfo">MethodInfo of method on new target</param>
    /// <param name="newTargetType">Type of new target</param>
    private protected override void AssertTargetMethodInfo(MethodInfo targetMethodInfo, Type newTargetType)
    {
        var methodParameters = targetMethodInfo.GetParameters();
        if (!(methodParameters.Length == 0 ||
            methodParameters.Length == 1 && (typeof(EventArgs).IsAssignableFrom(methodParameters[0].ParameterType) || methodParameters[0].ParameterType == typeof(AvaloniaPropertyChangedEventArgs)) ||
            methodParameters.Length == 2 && (typeof(EventArgs).IsAssignableFrom(methodParameters[1].ParameterType) || methodParameters[1].ParameterType == typeof(AvaloniaPropertyChangedEventArgs))))
        {
            var e = new ActionSignatureInvalidException(string.Format("Method {0} on {1} must have the signatures Method(), Method(EventArgsOrSubClass e), or Method(object sender, EventArgsOrSubClass e)", MethodName, newTargetType.Name));
            logger.Error(e);
            throw e;
        }
    }

    /// <summary>
    /// Return a delegate which can be added to the targetProperty
    /// </summary>
    /// <returns>An event hander, which, when invoked, will invoke the action</returns>
    public Delegate GetDelegate()
    {
        Delegate del = null;
        foreach (var invokeCommandMethodInfo in invokeCommandMethodInfos)
        {
            del = Delegate.CreateDelegate(eventHandlerType, this, invokeCommandMethodInfo, false);
            if (del != null)
                break;
        }

        if (del == null)
        {
            var msg = string.Format("Event being bound to does not have a signature we know about. Method {0} on target {1}. Valid signatures are:" +
                "Valid signatures are:\n" +
                " - '(object sender, EventArgsOrSubclass e)'\n" +
                " - '(object sender, DependencyPropertyChangedEventArgs e)'", MethodName, Target);
            var e = new ActionEventSignatureInvalidException(msg);
            logger.Error(e);
            throw e;
        }

        return del;
    }

    private void InvokeDependencyCommand(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        InvokeCommand(sender, e);
    }

    private void InvokeEventArgsCommand(object sender, EventArgs e)
    {
        InvokeCommand(sender, e);
    }

    // ReSharper disable once UnusedMember.Local
    private void InvokeCommand(object sender, object e)
    {
        AssertTargetSet();

        // Any throwing will have been handled above
        if (Target == null || TargetMethodInfo == null)
            return;

        object[] parameters;
        switch (TargetMethodInfo.GetParameters().Length)
        {
            case 1:
                parameters = new object[] { e };
                break;

            case 2:
                parameters = new[] { sender, e };
                break;

            default:
                parameters = null;
                break;
        }
        InvokeTargetMethod(parameters);
    }
}

/// <summary>
/// You tried to use an EventAction with an event that doesn't follow the EventHandler signature
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
public class ActionEventSignatureInvalidException : Exception
{
    internal ActionEventSignatureInvalidException(string message) : base(message) { }
}
