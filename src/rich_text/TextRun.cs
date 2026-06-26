namespace Spectrum.RichText;

/// <summary>
/// Represents a segment of text whose characters share the same style.
/// </summary>
/// <param name="text">
/// The characters in the text run.
/// </param>
/// <param name="style">
/// The style for the text run.
/// </param>
public readonly struct TextRun(string text, TextRunStyle style)
{
	/// <summary>
	/// Gets the string of characters in the text run.
	/// </summary>
	public string Text { get; } = text;

	/// <summary>
	/// Gets the style properties shared by every character in the text run.
	/// </summary>
	public TextRunStyle Style { get; } = style;
}
