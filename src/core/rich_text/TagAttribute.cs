using System;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a name-value string attribute associated with a tag.
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
    /// Creates a new <see cref="TagAttribute"/> with the specified name and value strings.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="name"/> or <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public TagAttribute(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);

        _source = name + value;
        _nameStart = 0;
        _nameLength = name.Length;
        _valueStart = name.Length;
        _valueLength = value.Length;
    }

    internal TagAttribute(string source, int nameStart, int nameLength, int valueStart, int valueLength)
    {
        _source = source;
        _nameStart = nameStart;
        _nameLength = nameLength;
        _valueStart = valueStart;
        _valueLength = valueLength;
    }

    /// <summary>
    /// Gets the name of the attribute.
    /// </summary>
    /// <remarks>
    /// An empty name indicates that the current instance is a <b>main attribute</b>, that is, the implicit, nameless
    /// attribute specified directly after the tag name (e.g. <c>aqua</c> in <c>"&lt;c=aqua&gt;"</c>).
    /// </remarks>
    public ReadOnlySpan<char> Name => IsDefault ? [] : _source.AsSpan(_nameStart, _nameLength);

    /// <summary>
    /// Gets the value of the attribute.
    /// </summary>
    public ReadOnlySpan<char> Value => IsDefault ? [] : _source.AsSpan(_valueStart, _valueLength);

    /// <summary>
    /// Gets a value indicating whether the attribute is the <see langword="default"/> value,
    /// </summary>
    public bool IsDefault => _source is null;

    /// <summary>
    /// Gets a value indicating whether the attribute is the main, implicit attribute of a tag.
    /// </summary>
    public bool IsMain => !IsDefault && IsNamed(string.Empty);

    /// <summary>
    /// Determines whether the attribute has the specified name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>
    /// <see langword="true"/> if the attribute's name matches the specified name; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsNamed(ReadOnlySpan<char> name)
    {
        return Name.SequenceEqual(name);
    }
}
