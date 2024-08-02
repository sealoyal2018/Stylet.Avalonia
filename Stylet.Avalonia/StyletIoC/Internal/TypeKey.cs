using System;

namespace Stylet.Avalonia.StyletIoC.Internal;

/// <summary>
/// Type + key tuple, used as a dictionary key
/// </summary>
internal class TypeKey : IEquatable<TypeKey>
{
    public readonly RuntimeTypeHandle TypeHandle;
    public readonly string Key;

    public TypeKey(RuntimeTypeHandle typeHandle, string key)
    {
        TypeHandle = typeHandle;
        Key = key;
    }

    public override int GetHashCode()
    {
        if (Key == null)
            return TypeHandle.GetHashCode();
        return TypeHandle.GetHashCode() ^ Key.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as TypeKey);
    }

    public bool Equals(TypeKey other)
    {
        return other != null && TypeHandle.Equals(other.TypeHandle) && Key == other.Key;
    }
}
