using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents the visual shape of a text character or icon.
/// </summary>
public readonly struct Glyph
{
	/// <summary>
	/// Gets the start index of the glyph within the source string.
	/// </summary>
	public int Start { get; init; }
	
	/// <summary>
	/// Gets the end index of the glyph within the source string.
	/// </summary>
	public int End { get; init; }

	/// <summary>
	/// Gets the number of glyphs in the grapheme.
	/// </summary>
	/// <remarks>
	/// This is only set in the first glyph of the grapheme.
	/// </remarks>
	public byte Count { get; init; }

	/// <summary>
	/// Gets the number of consecutive times the glyph should be drawn.
	/// </summary>
	public byte Repeat { get; init; }

	/// <summary>
	/// Gets a value describing meta characteristics of the grapheme.
	/// </summary>
	public TextServer.GraphemeFlag Flags { get;init; }
	
	/// <summary>
	/// Gets the offset to the glyph's origin from the baseline.
	/// </summary>
	public Vector2 Offset { get;init; }

	/// <summary>
	/// Gets the distance to the next glyph along the baseline.
	/// </summary>
	public float Advance { get;init; }

	/// <summary>
	/// Gets the glyph index, specific to <see cref="Font"/>.
	/// </summary>
	public int Index { get; init; }

	/// <summary>
	/// Gets the <see cref="TextServer"/> font resource used by the glyph.
	/// </summary>
	public Rid Font { get; init; }

	/// <summary>
	/// Gets the size of the font, in pixels.
	/// </summary>
	public int FontSize { get; init; }

	/// <summary>
	/// Gets the texture resource representing the icon.
	/// </summary>
	public Texture2D? IconTexture { get; init; }

	/// <summary>
	/// Gets the base color tint of the glyph.
	/// </summary>
	public Color Color { get; init; }

	/// <summary>
	/// Gets the text effect attached to the glyph.
	/// </summary>
	public TextEffect? Effect { get; init; }

	/// <summary>
	/// Gets the size of the shadow effect, in pixels.
	/// </summary>
	public int ShadowSize { get; init; }

	/// <summary>
	/// Gets the color for the shadow effect.
	/// </summary>
	public Color ShadowColor { get; init; }

	/// <summary>
	/// Gets the displacement of the shadow effect, relative to the glyph's position.
	/// </summary>
	public Vector2 ShadowOffset { get; init; }

	/// <summary>
	/// Gets the size of the glyph's outline, in pixels.
	/// </summary>
	public int OutlineSize { get; init; }

	/// <summary>
	/// Gets the color of the glyph's outline.
	/// </summary>
	public Color OutlineColor { get; init; }
}
