using Godot;

namespace Spectrum.RichText;

/// <summary>
///   Represents a set of style attributes applied to a <see cref="TextRun"/> that overrides the base
///   <see cref="TextStyle"/> of a <see cref="Text"/> instance.
/// </summary>
/// <remarks>
///   Every property from this struct is nullable. At glyph generation, the text shaper reads every
///   <see cref="TextRun"/> produced by the parser, and priorizes the style overrides assigned to the text runs over
///   the <see cref="TextStyle"/> assigned to the corresponding <see cref="Text"/> instance. A <see langword="null"/>
///   value for a property informs to the text shaper that it should use the corresponding property from the base
///   style.
/// </remarks>
public readonly struct StyleOverride
{
	/// <summary>
	///   Gets the font resource for the text run.
	/// </summary>
	public Font? Font { get; init; }

	/// <summary>
	///   Gets the font size for the text run, in pixels.
	/// </summary>
	public uint? FontSize { get; init; }

	/// <summary>
	///   Gets the color for the text run.
	/// </summary>
	public Color? Color { get; init; }

	/// <summary>
	///   Gets the character spacing for the text run, in pixels.
	/// </summary>
	public int? LetterSpacing { get; init; }

	/// <summary>
	///   Gets the line spacing for the text run, in pixels
	/// </summary>
	public int? LineSpacing { get; init; }
}
