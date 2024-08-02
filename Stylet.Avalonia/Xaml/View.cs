using System;
using Avalonia;
using Avalonia.Controls;

namespace Stylet.Avalonia.Xaml;

/// <summary>
/// Holds attached properties relating to various bits of the View which are used by Stylet
/// </summary>
public static class View
{
    /// <summary>
    /// Key which will be used to retrieve the ViewManager associated with the current application, from application's resources
    /// </summary>
    //public const string ViewManagerResourceKey = "b9a38199-8cb3-4103-8526-c6cfcd089df7";

    /// <summary>
    /// Initial value of the ActionTarget property.
    /// This can be used as a marker - if the property has this value, it hasn't yet been assigned to anything else.
    /// </summary>
    public static readonly object InitialActionTarget = new object();

    /// <summary>
    /// Get the ActionTarget associated with the given object
    /// </summary>
    /// <param name="obj">Object to fetch the ActionTarget for</param>
    /// <returns>ActionTarget associated with the given object</returns>
    public static object GetActionTarget(AvaloniaObject obj)
    {
        return obj.GetValue(ActionTargetProperty);
    }

    /// <summary>
    /// Set the ActionTarget associated with the given object
    /// </summary>
    /// <param name="obj">Object to set the ActionTarget for</param>
    /// <param name="value">Value to set the ActionTarget to</param>
    public static void SetActionTarget(AvaloniaObject obj, object value)
    {
        obj.SetValue(ActionTargetProperty, value);
    }

    /// <summary>
    /// The object's ActionTarget. This is used to determine what object to call Actions on by the ActionExtension markup extension.
    /// </summary>
    public static readonly AvaloniaProperty ActionTargetProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, object>("ActionTarget", typeof(View), InitialActionTarget, inherits: true);

    /// <summary>
    /// Fetch the ViewModel currently associated with a given object
    /// </summary>
    /// <param name="obj">Object to fetch the ViewModel for</param>
    /// <returns>ViewModel currently associated with the given object</returns>
    public static object GetModel(AvaloniaObject obj)
    {
        return obj.GetValue(ModelProperty);
    }

    /// <summary>
    /// Set the ViewModel currently associated with a given object
    /// </summary>
    /// <param name="obj">Object to set the ViewModel for</param>
    /// <param name="value">ViewModel to set</param>
    public static void SetModel(AvaloniaObject obj, object value)
    {
        obj.SetValue(ModelProperty, value);
    }

    private static readonly object defaultModelValue = new object();

    /// <summary>
    /// Property specifying the ViewModel currently associated with a given object
    /// </summary>
    public static readonly AvaloniaProperty ModelProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, object>("Model", typeof(View), defaultModelValue);

    /// <summary>
    /// Helper to set the Content property of a given object to a particular View
    /// </summary>
    /// <param name="targetLocation">Object to set the Content property on</param>
    /// <param name="view">View to set as the object's Content</param>
    public static void SetContentProperty(AvaloniaObject targetLocation, Control view)
    {
        var type = targetLocation.GetType();
        string propertyName = "Content";
        var property = type.GetProperty(propertyName);
        if (property == null)
            throw new InvalidOperationException(string.Format(
                "Unable to find a Content property on type {0}. Make sure you're using 's:View.Model' on a suitable container, e.g. a ContentControl",
                type.Name));
        property.SetValue(targetLocation, view);
    }


    static View()
    {
        ModelProperty.Changed.Subscribe(e =>
        {
            if (e.Sender is Control control)
            {
                var viewManager = IoC.Get<IViewManager>();
                var newValue = e.NewValue == defaultModelValue ? null : e.NewValue;
                if (newValue is null)
                    return;
                viewManager.OnModelChanged(e.Sender, e.OldValue, newValue);
                return;
            }
            else if (Execute.InDesignMode)
            {

                // var bindingExpression = BindingOperations.GetBindingExpression(d, ModelProperty);
                // string text;
                // if (bindingExpression == null)
                //     text = "View for [Broken Binding]";
                // else if (bindingExpression.ResolvedSourcePropertyName == null)
                //     text = $"View for child ViewModel on {bindingExpression.DataItem.GetType().Name}";
                // else
                //     text = String.Format("View for {0}.{1}", bindingExpression.DataItem.GetType().Name,
                //         bindingExpression.ResolvedSourcePropertyName);
                // SetContentProperty(e.Sender, new TextBlock() { Text = text });
            }
            else
            {
                throw new InvalidOperationException("The ViewManager resource is unassigned. This should have been set by the StyletApplication");
            }
        });
    }
}