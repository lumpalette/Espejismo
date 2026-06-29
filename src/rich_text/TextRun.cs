namespace Spectrum.RichText;

/// <summary>
/// Represents a segment of text with a <see cref="RichText.StyleOverride"/> applied to its characters.
/// </summary>
/// <param name="text">
/// The characters in the text run.
/// </param>
/// <param name="style">
/// The style override for the text run.
/// </param>
public readonly struct TextRun(string text, StyleOverride style)
{
	/// <summary>
	/// Gets the string of characters in the text run.
	/// </summary>
	public string Text { get; } = text;
	
	/// <summary>
	/// Gets the style properties overriden for this text run.
	/// </summary>
	public StyleOverride StyleOverride { get; } = style;
}
