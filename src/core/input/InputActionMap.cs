using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.Input;

/// <summary>
///   Represents a collection of <see cref="InputAction"/> instances indexed by unique names.
/// </summary>
public sealed class InputActionMap : IEnumerable<KeyValuePair<string, InputAction>>
{
	private readonly Dictionary<string, InputAction> _actions = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	///   Gets a collection containing all names of the actions defined in the map.
	/// </summary>
	public IEnumerable<string> Names => _actions.Keys;

	/// <summary>
	///   Gets a collection containing all the <see cref="InputAction"/> instances defined in the map.
	/// </summary>
	public IEnumerable<InputAction> Actions => _actions.Values;

	/// <summary>
	///   Gets the number of <see cref="InputAction"/> instances defined in the map.
	/// </summary>
	public int Count => _actions.Count;

	/// <summary>
	///   Gets the <see cref="InputAction"/> defined with the specified name.
	/// </summary>
	/// <param name="name">
	///   The name of the action to get, case-insensitive.
	/// </param>
	/// <returns>
	///   The <see cref="InputAction"/> associated with <paramref name="name"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="name"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="KeyNotFoundException">
	///   Thrown if there is no action defined with <paramref name="name"/>.
	/// </exception>
	public InputAction this[string name]
	{
		get
		{
			ArgumentNullException.ThrowIfNull(name, nameof(name));

			if (!_actions.TryGetValue(name, out var action))
			{
				throw new KeyNotFoundException($"Action with name '{name}' is undefined");
			}

			return action;
		}
	}

	/// <summary>
	///   Defines an <see cref="InputAction"/> under the specified name.
	/// </summary>
	/// <param name="name">
	///   A unique name to identify the action in the map, case-insensitive.
	/// </param>
	/// <param name="action">
	///   The action to define.
	/// </param>
	/// <exception cref="ArgumentException">
	///   Thrown if an action with the same <paramref name="name"/> is already defined.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="name"/> or <paramref name="action"/> are <see langword="null"/>.
	/// </exception>
	public void Add(string name, InputAction action)
	{
		ArgumentNullException.ThrowIfNull(name, nameof(name));
		ArgumentNullException.ThrowIfNull(action, nameof(action));
		
		if (!_actions.TryAdd(name, action))
		{
			throw new ArgumentException($"Action with name '{name}' is already defined", nameof(name));
		}
	}

	/// <summary>
	///   Gets the <see cref="InputAction"/> defined with the specified name.
	/// </summary>
	/// <param name="name">
	///   The name of the action to get, case-insensitive.
	/// </param>
	/// <param name="action">
	///   When this method returns, contains the action associated with <paramref name="name"/>, if defined; otherwise,
	///   <see langword="null"/>.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if an action named <paramref name="name"/>; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="name"/> is <see langword="null"/>.
	/// </exception>
	public bool TryGetAction(string name, [NotNullWhen(true)] out InputAction? action)
	{
		ArgumentNullException.ThrowIfNull(name, nameof(name));
		return _actions.TryGetValue(name, out action);
	}

	/// <summary>
	///   Returns an enumerator that iterates through the <see cref="InputAction"/> instances defined in the map.
	/// </summary>
	/// <returns>
	///   An <see cref="IEnumerator{T}"/> for the <see cref="InputActionMap"/>.
	/// </returns>
	public IEnumerator<KeyValuePair<string, InputAction>> GetEnumerator()
	{
		return _actions.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
