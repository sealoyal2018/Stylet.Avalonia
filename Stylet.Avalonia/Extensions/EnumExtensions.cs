using System;
using System.ComponentModel;
using System.Linq;

namespace Stylet.Avalonia.Extensions;

internal static class EnumExtensions
{
    public static string GetDescription(this Enum? self)
    {
        if (self is null)
        {
            return null;
        }
        var attribute = self.GetType()
            .GetField(self.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;
        return attribute == null ? self.ToString() : attribute.Description;
    }
}