using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents the visual shape of a text character or icon.
/// </summary>
public readonly struct Glyph // 56 bytes pesa la marrana
{
	/// <summary>
	/// Gets the start position of the glyph within the source string.
	/// </summary>
	public int Start { get; internal init; }

	/// <summary>
	/// Gets the end position of the glyph within the source string.
	/// </summary>
	public int End { get; internal init; }

	/// <summary>
	/// Gets the index of the glyph in the source font, if applicable.
	/// </summary>
	public ushort Index { get; internal init; } // fonts cannot hold more than 0xFFFF glyphs, so ushort it's fine.

	/// <summary>
	/// Gets the <see cref="TextServer"/> font resource used for the glyphs, if applicable.
	/// </summary>
	public Rid Font { get; internal init; }

	/// <summary>
	/// Gets the size of the <see cref="Font"/>, in pixels.
	/// </summary>
	public ushort FontSize { get; internal init; }

	/// <summary>
	/// Gets the texture resource associated to the icon, if applicable.
	/// </summary>
	public Texture2D? IconTexture { get; internal init; }

	/// <summary>
	/// Gets a value indicating whether the glyph represents a text character.
	/// </summary>
	/// <value>
	/// <see langword="true"/> if the glyph represents a character; <see langword="false"/> if it represents an icon.
	/// </value>
	public bool IsChar => Index != 0;

	/// <summary>
	/// Gets the group of style properties associated to this glyph.
	/// </summary>
	public GlyphStyle Style { get; internal init; }

	/// <summary>
	/// Gets the distance to the next glyph along the baseline.
	/// </summary>
	public float Advance { get; internal init; }

	/// <summary>
	/// Gets the offset to the glyph's origin from the baseline.
	/// </summary>
	public Vector2 Offset { get; internal init; }

	/// <summary>
	/// Gets the number of glyphs in the grapheme cluster, only set in the first glyph.
	/// </summary>
	public byte Count { get; internal init; }

	/// <summary>
	/// Gets the number of consecutive times the glyph should be drawn.
	/// </summary>
	public byte Repeat { get; internal init; }

	/// <summary>
	/// Gets a value describing the category or characteristics of this glyph.
	/// </summary>
	public ushort Flags { get; internal init; } // This is a TextServer.GraphemeFlag value.
}
