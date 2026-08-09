using System;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a name-value string attribute associated to a <see cref="TextTag"/>.
/// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public readonly struct TagAttribute
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
	private readonly string _source;
	private readonly int _nameStart;
	private readonly int _nameLength;
	private readonly int _valueStart;
	private readonly int _valueLength;

	/// <summary>
	/// Initializes a new instance of the <see cref="TagAttribute"/> struct using the specified source string and
	/// character ranges for the name and value.
	/// </summary>
	/// <param name="source">
	/// The source string containing the tag data. 
	/// </param>
	/// <param name="nameStart">
	/// The zero-based starting index of the name in <paramref name="source"/>.
	/// </param>
	/// <param name="nameLength">
	/// The number of characters in the name.
	/// </param>
	/// <param name="valueStart">
	/// The zero-based starting index of the value in <paramref name="source"/>.
	/// </param>
	/// <param name="valueLength">
	/// The number of characters in the value.
	/// </param>
	public TagAttribute(string source, int nameStart, int nameLength, int valueStart, int valueLength)
	{
		ArgumentNullException.ThrowIfNull(source, nameof(source));

		_source = source;
		_nameStart = nameStart;
		_nameLength = nameLength;
		_valueStart = valueStart;
		_valueLength = valueLength;
	}

	/// <summary>
	/// Gets the name of the attribute.
	/// </summary>
	public ReadOnlySpan<char> Name => _source.AsSpan(_nameStart, _nameLength);

	/// <summary>
	/// Gets the value of the attribute.
	/// </summary>
	public ReadOnlySpan<char> Value => _source.AsSpan(_valueStart, _valueLength);

	/// <summary>
	/// Gets a value indicating whether the attribute was specified in a tag.
	/// </summary>
	public bool IsDefined => _source is not null;
}
