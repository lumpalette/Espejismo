using System;

namespace Espejismo.Core.RichText;

/// <summary>
///   Represents a named container of <see cref="TagAttribute"/> instances, embedded into shaped <see cref="Text"/>.
/// </summary>
public readonly struct TextMarker
{
	private readonly TagAttribute[] _attributes;

	internal TextMarker(string name, TagAttribute[] attributes, int index)
	{
		Name = name;
		_attributes = attributes;
		Index = index;
	}

	/// <summary>
	///   Gets the name of the marker.
	/// </summary>
	public string Name { get; }

	/// <summary>
	///   Gets the attributes associated to the marker.
	/// </summary>
	public ReadOnlySpan<TagAttribute> Attributes => _attributes;

	/// <summary>
	///   Gets the index into <see cref="Text.Glyphs"/> at which the mark is embedded.
	/// </summary>
	public int Index { get; }
}
