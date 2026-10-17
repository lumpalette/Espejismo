using Godot;
using System.Collections.Generic;

namespace Espejismo.Core.RichText;

/// <summary>
/// Serves as the base class for any <see cref="Resource"/> that belongs to the rich-text system.
/// </summary>
[GlobalClass, Tool]
public abstract partial class TextResource : Resource
{
    /// <summary>
    /// Assigns the specified field to the specified value and emits <see cref="Resource.Changed"/> if it differs from
    /// the current value.
    /// </summary>
    /// <typeparam name="T">The type of the field to set.</typeparam>
    /// <param name="field">A reference to the backing field to update.</param>
    /// <param name="value">The new value.</param>
    /// <remarks>Values are compared with the default equality comparer for <typeparamref name="T"/>.</remarks>
    protected void SetAndNotify<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            EmitChanged();
        }
    }
}
