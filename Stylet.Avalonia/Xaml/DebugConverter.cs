﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Data.Converters;

namespace Stylet.Avalonia.Xaml;

/// <summary>
/// Converter which passes through values, but uses Debug.WriteLine to log them. Useful for debugging
/// </summary>
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Don't agree with prefixing static method calls with the class name")]
public class DebugConverter : AvaloniaObject, IValueConverter
{
    /// <summary>
    /// Singleton instance of this DebugConverter. Usage e.g. Converter={x:Static s:DebugConverter.Instance}"
    /// </summary>
    public static readonly DebugConverter Instance;

    /// <summary>
    /// Gets or sets the category to use with Debug.WriteLine
    /// </summary>
    public string Name
    {
        get { return (string)GetValue(NameProperty); }
        set { SetValue(NameProperty, value); }
    }

    /// <summary>
    /// Property specifying the category to use with Debug.WriteLine
    /// </summary>
    public static readonly AvaloniaProperty NameProperty =
        AvaloniaProperty.Register<DebugConverter, string>("Name", "DebugConverter");

    /// <summary>
    /// Gets or sets the Logger to use. Defaults to Debug.WriteLine. Arguments are: Message, Name
    /// </summary>
    public Action<string, string> Logger
    {
        get { return (Action<string, string>)GetValue(LoggerProperty); }
        set { SetValue(LoggerProperty, value); }
    }

    /// <summary>
    /// Property specifying an action, which when called will log an entry.
    /// </summary>
    public static readonly AvaloniaProperty LoggerProperty =
        AvaloniaProperty.Register<DebugConverter, Action<string, string>>("Logger", null);

    static DebugConverter()
    {
        // Have to set this from within a static constructor, as it's run after the field initializers
        // Otherwise it gets called before the DependencyProperties have been created, and that causes the (normal) constructor to fall over
        Instance = new DebugConverter();
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="DebugConverter"/> class
    /// </summary>
    public DebugConverter()
    {
        if (Logger == null)
            Logger = (msg, name) => Debug.WriteLine(msg, name);
    }

    /// <summary>
    /// Perform the conversion
    /// </summary>
    /// <param name="value">value as produced by source binding</param>
    /// <param name="targetType">target type</param>
    /// <param name="parameter">converter parameter</param>
    /// <param name="culture">culture information</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (parameter == null)
            Logger(string.Format(culture, "Convert: Value = '{0}' TargetType = '{1}'", value, targetType), Name);
        else
            Logger(string.Format(culture, "Convert: Value = '{0}' TargetType = '{1}' Parameter = '{2}'", value, targetType, parameter), Name);

        return value;
    }

    /// <summary>
    /// Perform the reverse conversion
    /// </summary>
    /// <param name="value">value, as produced by target</param>
    /// <param name="targetType">target type</param>
    /// <param name="parameter">converter parameter</param>
    /// <param name="culture">culture information</param>
    /// <returns>Converted back value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (parameter == null)
            Logger(string.Format(culture, "ConvertBack: Value = '{0}' TargetType = '{1}'", value, targetType), Name);
        else
            Logger(string.Format(culture, "ConvertBack: Value = '{0}' TargetType = '{1}' Parameter = '{2}'", value, targetType, parameter), Name);

        return value;
    }
}
