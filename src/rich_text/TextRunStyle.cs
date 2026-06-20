using Godot;

namespace Spectrum.RichText;

/// <summary>
///		Represents the processed, read-only form of a <see cref="TextStyle"/> that is applied to multiple
///		<see cref="TextRun"/> instances during parsing.
/// </summary>
public readonly struct TextRunStyle()
{
	/// <summary>
	///		Gets the font resource for the text run.
	/// </summary>
	public Font Font { get; init; }

	/// <summary>
	///		Gets the text size for the text run, in pixels.
	/// </summary>
	public uint FontSize { get; init; }

	/// <summary>
	///		Gets the color for the text run.
	/// </summary>
	public Color Color { get; init; }

	/// <summary>
	///		Gets a value indicating whether the instance was not properly initialized.
	/// </summary>
	public bool IsDefault { get; } = true;
}
