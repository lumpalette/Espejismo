using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties that describe how text is rendered.
/// </summary>
/// <remarks>
/// All properties in this struct are nullable. A <see langword="null"/> property indicates that the property must be
/// resolved elsewhere. How a property is resolved depends on the context in which the style is used.
/// </remarks>
public readonly record struct TextStyle
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TextStyle"/> struct fully unset.
	/// </summary>
	public TextStyle()
	{
	}

	/// <summary>
	/// Gets the <see cref="Godot.Font"/> resource used for the text.
	/// </summary>
	public Font? Font { get; init; }

	/// <summary>
	/// Gets the size of the text, in pixels.
	/// </summary>
	public ushort? FontSize { get; init; }

	/// <summary>
	/// Gets the color tint of the text.
	/// </summary>
	public Color? Color { get; init; }

	/// <summary>
	/// Gets the visual effect applied to the text.
	/// </summary>
	public TextEffect? Effect { get; init; }

	/// <summary>
	/// Gets the additional spacing added between glyphs.
	/// </summary>
	public int? LetterSpacing { get; init; }

	/// <summary>
	/// Gets the additional spacing added between lines of text.
	/// </summary>
	public int? LineSpacing { get; init; }

	/// <summary>
	/// Gets the size of the shadow effect, in pixels.
	/// </summary>
	public ushort? ShadowSize { get; init; }

	/// <summary>
	/// Gets the color for the shadow effect.
	/// </summary>
	public Color? ShadowColor { get; init; }
	
	/// <summary>
	/// Gets the displacement for the shadow effect, relative to the main text.
	/// </summary>
	public Vector2? ShadowOffset { get; init; }

	/// <summary>
	/// Gets the size for the text outline, in pixels.
	/// </summary>
	public ushort? OutlineSize { get; init; }

	/// <summary>
	/// Gets the color for the text outline.
	/// </summary>
	public Color? OutlineColor { get; init; }

	/// <summary>
	/// Creates a new <see cref="TextStyle"/> by combining the properties of this style with another.
	/// </summary>
	/// <param name="other">
	/// The fallback style to merge with when the properties of this style are unset.
	/// </param>
	/// <returns>
	/// The merged <see cref="TextStyle"/>.
	/// </returns>
	public TextStyle MergedWith(in TextStyle other)
	{
		return new TextStyle
		{
			Font = Font ?? other.Font,
			FontSize = FontSize ?? other.FontSize,
			Color = Color ?? other.Color,
			Effect = Effect ?? other.Effect,
			LetterSpacing = LetterSpacing ?? other.LetterSpacing,
			LineSpacing = LineSpacing ?? other.LineSpacing,
			ShadowSize = ShadowSize ?? other.ShadowSize,
			ShadowColor = ShadowColor ?? other.ShadowColor,
			ShadowOffset = ShadowOffset ?? other.ShadowOffset,
			OutlineSize = OutlineSize ?? other.OutlineSize,
			OutlineColor = OutlineColor ?? other.OutlineColor
		};
	}
}
