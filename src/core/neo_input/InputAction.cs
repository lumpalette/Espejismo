using System;
using System.Collections.Generic;

namespace Espejismo.Core.NeoInput;

/// <summary>
/// Represents a logical game action defined by a collection of <see cref="InputSource"/> bindings.
/// </summary>
public class InputAction
{
    private readonly HashSet<InputSource> _sources = [];

    /// <summary>
    /// Creates a new <see cref="InputAction"/> instance with no bound sources.
    /// </summary>
    public InputAction()
    {
    }

    /// <summary>
    /// Creates a new <see cref="InputAction"/> instance bound to the specified <see cref="InputSource"/> instances.
    /// </summary>
    /// <param name="sources">The sources to bind.</param>
    public InputAction(params ReadOnlySpan<InputSource> sources)
    {
        foreach (var source in sources)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(sources));
            _ = _sources.Add(source);
        }
    }

    /// <summary>
    /// Gets a collection of <see cref="InputSource"/> instances bound to this action.
    /// </summary>
    public IReadOnlySet<InputSource> Sources => _sources;

    // A counter that increments every time a binding is addded or removed from the action.
    internal int Version { get; private set; }

    /// <summary>
    /// Binds an <see cref="InputSource"/> to the action.
    /// </summary>
    /// <param name="source">The source to bind.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="source"/> is successfully bound; <see langword="false"/> if it
    /// is already bound.
    /// </returns>
    public bool Bind(InputSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var bound = _sources.Add(source);

        if (bound)
        {
            Version++;
        }

        return bound;
    }

    /// <summary>
    /// Unbinds an <see cref="InputSource"/> to the action.
    /// </summary>
    /// <param name="source">The source to unbind.</param>
    /// <returns>
    /// s<see langword="true"/> if the <paramref name="source"/> was successfully unbound; <see langword="false"/> if
    /// it is already unbound.
    /// </returns>
    public bool Unbind(InputSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var unbound = _sources.Remove(source);

        if (unbound)
        {
            Version++;
        }

        return unbound;
    }

    /// <summary>
    /// Unbinds every <see cref="InputSource"/> associated with the action.
    /// </summary>
    public void UnbindAll()
    {
        if (_sources.Count > 0)
        {
            _sources.Clear();
            Version++;
        }
    }
}
