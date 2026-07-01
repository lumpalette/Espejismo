using System;

namespace Spectrum.RichText;

/// <summary>
///   Represents a name-value string property associated to a rich-text tag or command.
/// </summary>
/// <param name="text">
///   The rich-text string containing the tag.
/// </param>
/// <param name="nameStart">
///   The position of the name in <paramref name="text"/>.
/// </param>
/// <param name="nameLength">
///   The number of characters in the name.
/// </param>
/// <param name="valueStart">
///   The position of the name in <paramref name="text"/>.
/// </param>
/// <param name="valueLength">
///   The number of characters in the value.
/// </param>
public readonly struct TagProperty(string text, int nameStart, int nameLength, int valueStart, int valueLength)
{
	/// <summary>
	///   Gets the name of the property.
	/// </summary>
	public ReadOnlySpan<char> Name => text.AsSpan(nameStart, nameLength);

	/// <summary>
	///   Gets the value of the property.
	/// </summary>
	public ReadOnlySpan<char> Value => text.AsSpan(valueStart, valueLength);

	/// <summary>
	///   Gets a value indicating whether the property was properly defined and initialized.
	/// </summary>
	public bool IsDefined { get; } = true;
}
