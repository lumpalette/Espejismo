using Godot;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties that serves as a template for creating <see cref="TextStyle"/> instances.
/// </summary>
[GlobalClass, Tool]
public partial class StyleTemplate : Resource
{
	/// <summary>
	/// Gets the <see cref="Godot.Font"/> resource used for the text.
	/// </summary>
	[Export, NotNull]
	public Font? Font { get; private set; }
	
	/// <summary>
	/// Gets the size of the text, in pixels. Defaults to 8px.
	/// </summary>
	[Export]
	public int FontSize { get; private set; } = 8;

	/// <summary>
	/// Gets the color tint of the text. Defaults to white.
	/// </summary>
	[Export]
	public Color Color { get; private set; } = Colors.White;

	/// <summary>
	/// Gets the visual effect applied to the text.
	/// </summary>
	[Export]
	public TextEffect? Effect { get; private set; }

	/// <summary>
	/// Gets the additional spacing added between text characters or icons.
	/// </summary>
	[Export]
	public int LetterSpacing { get; private set; }

	/// <summary>
	/// Gets the additional spacing added between lines of text.
	/// </summary>
	[Export]
	public int LineSpacing { get; private set; }

	/// <summary>
	/// Gets the size for the shadow effect, in pixels.
	/// </summary>
	[Export]
	public int ShadowSize { get; private set; }

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
	/// Gets the size for the text outline, in pixels.
	/// </summary>
	[Export]
	public int OutlineSize { get; private set; }

	/// <summary>
	/// Gets the color for the text outline. Defaults to black.
	/// </summary>
	[Export]
	public Color OutlineColor { get; private set; } = Colors.Black;
}
