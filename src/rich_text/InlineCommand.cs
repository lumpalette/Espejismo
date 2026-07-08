using System;

namespace Spectrum.RichText;

/// <summary>
///   Contains data for a text command embedded in parsed text at a specific position.
/// </summary>
/// <param name="name">
///   The name of the command.
/// </param>
/// <param name="properties">
///   The properties passed to the command.
/// </param>
/// <param name="position">
///   The position of the command in the parsed text.
/// </param>
public readonly struct InlineCommand(string name, TagProperty[] properties, int position)
{
	/// <summary>
	///   Gets the name of the command.
	/// </summary>
	public string Name { get; } = name;

	/// <summary>
	///   Gets the <see cref="TagProperty"/> instances passed to the command.
	/// </summary>
	public ReadOnlySpan<TagProperty> Properties => properties;

	/// <summary>
	///   Gets the zero-based index position of the command in the parsed text.
	/// </summary>
	/// <remarks>
	///   Commands do not occupy physical space within the text, so this acts as a marker for the glyph over which the
	///   command is positioned on top.
	/// </remarks>
	public int Position { get; } = position;
}
