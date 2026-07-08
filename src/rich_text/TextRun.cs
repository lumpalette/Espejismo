namespace Spectrum.RichText;

/// <summary>
///   Represents a sequence of characters that share the same style attributes.
/// </summary>
/// <param name="index">
///   The index for the run within the parsed sequence.
/// </param>
/// <param name="text">
///   The characters in the run.
/// </param>
/// <param name="style">
///   The style overrides for the run.
/// </param>
public readonly struct TextRun(int index, string text, StyleOverride style)
{
	/// <summary>
	///   Gets the zero-based index for the run within the final parsed sequence of runs and icons.
	/// </summary>
	public int SequenceIndex { get; } = index;

	/// <summary>
	///   Gets the string of characters in the run.
	/// </summary>
	public string Text { get; } = text;

	/// <summary>
	///   Gets the style attributes overriden for this run.
	/// </summary>
	public StyleOverride StyleOverride { get; } = style;
}
