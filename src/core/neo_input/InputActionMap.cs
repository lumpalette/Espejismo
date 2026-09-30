using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.NeoInput;

/// <summary>
/// Represents a collection of <see cref="InputAction"/> instances indexed by a unique name.
/// </summary>
public sealed class InputActionMap
{
    private readonly Dictionary<string, InputAction> _actions = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets a collection of the names of all <see cref="InputAction"/> instances contained in the map.
    /// </summary>
    public IEnumerable<string> Names => _actions.Keys;

    /// <summary>
    /// Gets a collection of the <see cref="InputAction"/> instances contained in the map.
    /// </summary>
    public IEnumerable<InputAction> Actions => _actions.Values;

    /// <summary>
    /// Gets the number of <see cref="InputAction"/> instances contained in the map.
    /// </summary>
    public int Count => _actions.Count;

    /// <summary>
    /// Gets the <see cref="InputAction"/> with the specified name.
    /// </summary>
    /// <param name="actionName">The name of the action to get.</param>
    /// <returns>The <see cref="InputAction"/> associated with <paramref name="actionName"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if an action with the specified name does not exist in the map.</exception>
    public InputAction this[string actionName]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(actionName, nameof(actionName));

            if (!_actions.TryGetValue(actionName, out var action))
            {
                throw new KeyNotFoundException($"An action with the name '{actionName}' could not be found in the map.");
            }

            return action;
        }
    }

    /// <summary>
    /// Adds a new <see cref="InputAction"/> to the map with the specified name.
    /// </summary>
    /// <param name="actionName">The name of the action to add. Must be unique within the map.</param>
    /// <param name="action">The action to add.</param>
    /// <exception cref="ArgumentException">Thrown if an action with the same name already exists in the map.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    public void Add(string actionName, InputAction action)
    {
        ArgumentNullException.ThrowIfNull(actionName);

        if (!_actions.TryAdd(actionName, action))
        {
            throw new ArgumentException("An action with the specified name already exists in the map.", nameof(actionName));
        }
    }

    /// <summary>
    /// Removes the <see cref="InputAction"/> with the specified name from the map.
    /// </summary>
    /// <param name="actionName">The name of the action to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the action is successfully found and removed; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    public bool Remove(string actionName)
    {
        ArgumentNullException.ThrowIfNull(actionName);

        var success = _actions.Remove(actionName, out var action);

        if (success)
        {
            action!.Strength = 0f;
        }

        return success;
    }

    /// <summary>
    /// Removes all <see cref="InputAction"/> instances from the map.
    /// </summary>
    public void Clear()
    {
        _actions.Clear();
    }

    /// <summary>
    /// Gets the <see cref="InputAction"/> with the specified name, if exists.
    /// </summary>
    /// <param name="actionName">The name of the action to get.</param>
    /// <param name="action">
    /// When this method returns, contains the action with the specified name, if exists; otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the map contains an <see cref="InputAction"/> with the specified name; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionName"/> is <see langword="null"/>.</exception>
    public bool TryGetAction(string actionName, [NotNullWhen(true)] out InputAction? action)
    {
        ArgumentNullException.ThrowIfNull(actionName);
        return _actions.TryGetValue(actionName, out action);
    }
}
