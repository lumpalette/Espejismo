using System;

namespace Espejismo.Core.RichText;

/// <summary>
/// Describes metadata associated to a rich-text tag, placed at a specific glyph position.
/// </summary>
public readonly struct TextMarker
{
	private readonly TagAttribute[] _attributes;

	internal TextMarker(string name, TagAttribute[] attributes, int index)
	{
		Name = name;
		_attributes = attributes;
		GlyphIndex = index;
	}

	/// <summary>
	/// Gets the name of the marker.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the attributes associated to the marker.
	/// </summary>
	public ReadOnlySpan<TagAttribute> Attributes => _attributes;

	/// <summary>
	/// Gets the index of the glyph the mark is positioned at.
	/// </summary>
	public int GlyphIndex { get; }
}
