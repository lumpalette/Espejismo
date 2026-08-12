using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
///   Represents a set of style properties associated to a group of <see cref="Glyph"/> instances.
/// </summary>
public class GlyphStyle
{
	internal GlyphStyle()
	{
	}

	/// <summary>
	///   Gets the color tint applied to the glyphs.
	/// </summary>
	public Color Color { get; internal set; }

	/// <summary>
	///   Gets the visual effect applied to the glyphs.
	/// </summary>
	public TextEffect? Effect { get; internal set; }

	/// <summary>
	///   Gets the size of the shadow effect, in pixels.
	/// </summary>
	public ushort ShadowSize { get; internal set; }

	/// <summary>
	///   Gets the color of the shadow effect.
	/// </summary>
	public Color ShadowColor { get; internal set; }

	/// <summary>
	///   Gets the displacement applied to the shadow effect, relative to the glyph position.
	/// </summary>
	public Vector2 ShadowOffset { get; internal set; }

	/// <summary>
	///   Gets the size of the glyph outline, in pixels.
	/// </summary>
	public ushort OutlineSize { get; internal set; }

	/// <summary>
	///   Gets the color of the glyph outline.
	/// </summary>
	public Color OutlineColor { get; internal set; }
}
