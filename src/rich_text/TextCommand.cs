using System;

namespace Spectrum.RichText;

/// <summary>
/// Contains data for a text command embedded in parsed text at a specific position.
/// </summary>
/// <param name="name">
/// The name of the command.
/// </param>
/// <param name="position">
/// The position of the command in the parsed text.
/// </param>
/// <param name="properties">
/// The properties passed to the command.
/// </param>
public readonly struct TextCommand(string name, int position, TagProperty[] properties)
{
	/// <summary>
	/// Gets the name of the command.
	/// </summary>
	public string Name { get; } = name;

	/// <summary>
	/// Gets the position index of the command in the parsed text.
	/// </summary>
	public int Position { get; } = position;

	/// <summary>
	/// Gets the properties that were passed to the command by the parser.
	/// </summary>
	public ReadOnlySpan<TagProperty> Properties => properties;
}
