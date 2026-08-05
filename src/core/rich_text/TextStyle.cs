using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties that describe how text is rendered.
/// </summary>
/// <remarks>
/// All properties in this struct are nullable. A <see langword="null"/> property indicates that the property must be
/// resolved elsewhere. How a property is resolved depends on the context in which the style is used. See
/// <see cref="Text.Style"/> and <see cref="ResourceDB.DefaultStyle"/> for more details.
/// </remarks>
public readonly record struct TextStyle
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TextStyle"/> struct.
	/// </summary>
	public TextStyle()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TextStyle"/> struct based on the specified
	/// <see cref="StyleTemplate"/>.
	/// </summary>
	/// <param name="template">
	/// The style template to copy.
	/// </param>
	public TextStyle(StyleTemplate template)
	{
		Font = template.Font;
		FontSize = template.FontSize;
		Color = template.Color;
		Effect = template.Effect;
		
		LetterSpacing = template.LetterSpacing;
		LineSpacing = template.LineSpacing;
		
		ShadowSize = template.ShadowSize;
		ShadowColor = template.ShadowColor;
		ShadowOffset = template.ShadowOffset;
		
		OutlineSize = template.OutlineSize;
		OutlineColor = template.OutlineColor;
	}

	/// <summary>
	/// Gets the <see cref="Godot.Font"/> resource used for the text.
	/// </summary>
	public Font? Font { get; init; }

	/// <summary>
	/// Gets the size of the text, in pixels.
	/// </summary>
	public int? FontSize { get; init; }

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
	public int? ShadowSize { get; init; }

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
	public int? OutlineSize { get; init; }

	/// <summary>
	/// Gets the color for the text outline.
	/// </summary>
	public Color? OutlineColor { get; init; }
}
