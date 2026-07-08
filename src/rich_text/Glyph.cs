using Godot;

namespace Spectrum.RichText;

/// <summary>
///   Represents the visual shape of a text element or character within a specific font.
/// </summary>
public struct Glyph
{
	/// <summary>
	///   Gets the start index of the glyph within the parent <see cref="TextLine"/>.
	/// </summary>
	public int Start { get; init; }

	/// <summary>
	///   Gets the end index of the glyph within the parent <see cref="TextLine"/>.
	/// </summary>
	public int End { get; init; }

	/// <summary>
	///   Gets the number of glyphs in the current grapheme.
	/// </summary>
	/// <remarks>
	///   This is set only for the first glyph of the grapheme.
	/// </remarks>
	public byte Count { get; init; }

	/// <summary>
	///   Gets the number of consecutive times the glyph should be drawn.
	/// </summary>
	public byte Repeat { get; init; }

	/// <summary>
	///   Gets a value that describes meta characteristics of the glyph.
	/// </summary>
	public TextServer.GraphemeFlag Flags { get; init; }

	/// <summary>
	///   Gets the offset to the glyph's origin from the baseline.
	/// </summary>
	public Vector2 Offset { get; init; }

	/// <summary>
	///   Gets or sets the distance to the next glyph along the baseline.
	/// </summary>
	public float Advance { get; init; }

	/// <summary>
	///   Gets the <see cref="TextServer"/> font resource used to render the glyph.
	/// </summary>
	public Rid Font { get; init; }

	/// <summary>
	///   Gets the size of the font, in pixels.
	/// </summary>
	public int FontSize { get; init; }

	/// <summary>
	///   Gets or sets the glyph index, specific to <see cref="Font"/>.
	/// </summary>
	public int Index { get; set; }

	/// <summary>
	///   Gets or sets the color tint of the glyph.
	/// </summary>
	public Color Color { get; set; }

	/// <summary>
	///   Gets a value indicating whether the glyph should be drawn or omitted.
	/// </summary>
	public bool IsVisible { get; set; }
}
