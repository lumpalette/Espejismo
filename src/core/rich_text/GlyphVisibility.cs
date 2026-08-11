namespace Espejismo.Core.RichText;

/// <summary>
/// Specifies the visibility states of a <see cref="Glyph"/> when it is rendered.
/// </summary>
public enum GlyphVisibility
{
	/// <summary>
	/// The glyph must be drawn by the renderer.
	/// </summary>
	Visible,

	/// <summary>
	/// The glyph must be skipped by the renderer, leaving an empty gap at the glyph's position.
	/// </summary>
	Invisible,

	/// <summary>
	/// The glyph must be skipped by the renderer, reflowing consecutive glyphs.
	/// </summary>
	Omitted
}
