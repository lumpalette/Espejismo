using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
///   Provides mutable access to specific properties of a <see cref="RichText.Glyph"/> for applying text effects.
/// </summary>
public struct GlyphTransform
{
	/// <summary>
	///   Gets the subject glyph being transformed.
	/// </summary>
	public required Glyph Glyph { get; init; }

	/// <summary>
	///   Gets the elapsed time since the text started rendering, in seconds.
	/// </summary>
	public required float Time { get; init; }

	/// <summary>
	///   Gets the index of the specific <see cref="RichText.Glyph"/> within the source <see cref="Text"/>.
	/// </summary>
	public required int Index { get; init; }

	/// <summary>
	///   Gets the normalized position of the glyph within the source line.
	/// </summary>
	/// <value>
	///   A floating-point number in the range [0,1].
	/// </value>
	public required float LineProgress { get; init; }

	/// <summary>
	///   Gets the number of glyphs in the source line.
	/// </summary>
	public required int LineLength { get; init; } 

	/// <summary>
	///   Gets or sets the color that the glyph will be drawn with.
	/// </summary>
	public Color Color { get; set; }

	/// <summary>
	///   Gets or sets the displacement applied to the glyph's draw position.
	/// </summary>
	public Vector2 Offset { get; set; }

	/// <summary>
	///   Gets or sets the visibility state of the glyph.
	/// </summary>
	public GlyphVisibility Visibility { get; set; }
}
