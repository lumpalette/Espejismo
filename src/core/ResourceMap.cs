using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core;

/// <summary>
///   Represents a read-only collection of <see cref="Resource"/> instances identified by string keys.
/// </summary>
/// <typeparam name="T">
///   The type of <see cref="Resource"/> contained in the map.
/// </typeparam>
public class ResourceMap<T> where T : Resource
{
	private readonly Dictionary<string, T> _data;
	private readonly Dictionary<string, T>.AlternateLookup<ReadOnlySpan<char>> _alt;

	/// <summary>
	///   Initializes a new instance of the <see cref="ResourceMap{T}"/> class by copying the entries of the specified
	///   collection.
	/// </summary>
	/// <param name="collection">
	///   The collection of key-value pair of resources to copy to the map.
	/// </param>
	public ResourceMap(IEnumerable<KeyValuePair<string, T>> collection)
	{
		_data = new Dictionary<string, T>(collection);
		_alt = _data.GetAlternateLookup<ReadOnlySpan<char>>();
	}

	/// <summary>
	///   Gets the <typeparamref name="T"/> resource associated to the specified name.
	/// </summary>
	/// <param name="name">
	///   The name of the resource to get.
	/// </param>
	/// <param name="res">
	///   When this method returns, contains the resource associated with <paramref name="name"/>, if found; otherwise,
	///   <see langword="false"/>.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the resource was found; otherwise, <see langword="false"/>.
	/// </returns>
	public bool TryGetResource(ReadOnlySpan<char> name, [NotNullWhen(true)] out T? res)
	{
		return _alt.TryGetValue(name, out res);
	}
}
