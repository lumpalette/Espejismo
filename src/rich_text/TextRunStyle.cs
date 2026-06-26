using Godot;
using System;

namespace Spectrum.RichText;

/// <summary>
///		Represents the processed, read-only form of a <see cref="TextStyle"/> that is applied to multiple
///		<see cref="TextRun"/> instances during parsing.
/// </summary>
public readonly struct TextRunStyle
{
	/// <summary>
	///		Initializes a new instance of the <see cref="TextRunStyle"/> struct by copying the data from the specified
	///		<see cref="TextStyle"/>.
	/// </summary>
	/// <param name="style">
	///		The style properties to copy.
	/// </param>
	public TextRunStyle(TextStyle style)
	{
		ArgumentNullException.ThrowIfNull(style, nameof(style));
		ArgumentNullException.ThrowIfNull(style.Font, nameof(style.Font));

		Font = style.Font;
		FontSize = style.FontSize;
		Color = style.Color;
	}

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
}
