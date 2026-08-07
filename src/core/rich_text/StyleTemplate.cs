using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties that serves as a template for creating <see cref="TextStyle"/> instances.
/// </summary>
[GlobalClass]
public partial class StyleTemplate : Resource
{
	/// <summary>
	/// Gets the <see cref="Godot.Font"/> resource used for the text.
	/// </summary>
	[ExportGroup("Typography")]
	[Export, NotNull]
	public Font? Font { get; private set; }

	/// <summary>
	/// Gets the size of the text, in pixels. Defaults to 8px.
	/// </summary>
	[Export]
	public ushort FontSize { get; private set; } = 8;

	/// <summary>
	/// Gets the color tint of the text. Defaults to white.
	/// </summary>
	[Export]
	public Color Color { get; private set; } = Colors.White;

	/// <summary>
	/// Gets the visual effect applied to the text.
	/// </summary>
	[ExportGroup("Effects")] // kinda useless but whatever
	[Export]
	public TextEffect? Effect { get; private set; }

	/// <summary>
	/// Gets the additional spacing added between text characters or icons.
	/// </summary>
	[ExportGroup("Spacing")]
	[Export]
	public int LetterSpacing { get; private set; }

	/// <summary>
	/// Gets the additional spacing added between lines of text. Defaults to 8px.
	/// </summary>
	[Export]
	public int LineSpacing { get; private set; } = 8;

	/// <summary>
	/// Gets the size for the shadow effect, in pixels.
	/// </summary>
	[ExportGroup("Shadow", "Shadow")]
	[Export]
	public ushort ShadowSize { get; private set; }

	/// <summary>
	/// Gets the color for the shadow effect. Defaults to black.
	/// </summary>
	[Export]
	public Color ShadowColor { get; private set; } = Colors.Black;

	/// <summary>
	/// Gets the displacement for the shadow effect. relative to the main text. Defaults to <c>(1,1)</c>.
	/// </summary>
	[Export]
	public Vector2 ShadowOffset { get; private set; } = Vector2.One;

	/// <summary>
	/// Gets the size for the text outline, in pixels. Defaults to 4px.
	/// </summary>
	[ExportGroup("Outline", "Outline")]
	[Export]
	public ushort OutlineSize { get; private set; } = 4;

	/// <summary>
	/// Gets the color for the text outline. Defaults to black.
	/// </summary>
	[Export]
	public Color OutlineColor { get; private set; } = Colors.Black;

	/// <summary>
	/// Creates a new <see cref="TextStyle"/> based on the data of this template.
	/// </summary>
	/// <returns>
	/// The created <see cref="TextStyle"/>, fully set.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if <see cref="Font"/> is <see langword="null"/>.
	/// </exception>
	public TextStyle Create()
	{
		return CreateFrom(default);
	}

	/// <summary>
	/// Creates a copy of the specified <see cref="TextStyle"/>, replacing any unset property with the data from this
	/// template.
	/// </summary>
	/// <param name="style">
	/// The style to copy; its properties will take precedence over the ones defined by the template.
	/// </param>
	/// <returns>
	/// The created <see cref="TextStyle"/>, fully set.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if <see cref="Font"/> is <see langword="null"/>.
	/// </exception>
	public TextStyle CreateFrom(in TextStyle style)
	{
		if (Font is null)
		{
			throw new InvalidOperationException("Template's font was not specified from the editor");
		}

		return new TextStyle
		{
			Font = style.Font ?? Font,
			FontSize = style.FontSize ?? FontSize,
			Color = style.Color ?? Color,
			Effect = style.Effect ?? Effect,
			LetterSpacing = style.LetterSpacing ?? LetterSpacing,
			LineSpacing = style.LineSpacing ?? LineSpacing,
			ShadowSize = style.ShadowSize ?? ShadowSize,
			ShadowColor = style.ShadowColor ?? ShadowColor,
			ShadowOffset = style.ShadowOffset ?? ShadowOffset,
			OutlineSize = style.OutlineSize ?? OutlineSize,
			OutlineColor = style.OutlineColor ?? OutlineColor
		};
	}
}
