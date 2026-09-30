using System;
using System.Collections.Generic;

namespace Espejismo.Core.NeoInput;

/// <summary>
///   Represents a logical game action, bound to one or more <see cref="InputSource"/> instances.
/// </summary>
public readonly struct InputAction
{
	private readonly HashSet<InputSource> _sources = [];

	/// <summary>
	///   Creates a new <see cref="InputAction"/> with no bound sources.
	/// </summary>
	public InputAction()
	{
	}

	/// <summary>
	///   Creates a new <see cref="InputAction"/> bound to the specified <see cref="InputSource"/> instances.
	/// </summary>
	/// <param name="sources">
	///   The sources to bind.
	/// </param>
	public InputAction(params ReadOnlySpan<InputSource> sources)
	{
		foreach (var source in sources)
		{
			_sources.Add(source);
		}
	}

	/// <summary>
	///   Gets a collection of <see cref="InputSource"/> instances that can trigger the action.
	/// </summary>
	public IReadOnlySet<InputSource> Sources => _sources;

	/// <summary>
	///   Binds an <see cref="InputSource"/> to the action.
	/// </summary>
	/// <param name="source">
	///   The source to bind.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the <paramref name="source"/> is successfully bound; <see langword="false"/> if it
	///   was already bound.
	/// </returns>
	public bool Bind(InputSource source)
	{
		ArgumentNullException.ThrowIfNull(source);
		return _sources.Add(source);
	}

	/// <summary>
	///   Unbinds an <see cref="InputSource"/> to the action.
	/// </summary>
	/// <param name="source">
	///   The source to unbind.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the <paramref name="source"/> is successfully unbound; <see langword="false"/> if
	///   it was already unbound.
	/// </returns>
	public bool Unbind(InputSource source)
	{
		ArgumentNullException.ThrowIfNull(source);
		return _sources.Remove(source);
	}

	/// <summary>
	///   Unbinds every <see cref="InputSource"/> associated with the action.
	/// </summary>
	public void UnbindAll()
	{
		_sources.Clear();
	}
}
