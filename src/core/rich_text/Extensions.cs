using Espejismo.Core.RichText;
using Godot;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Espejismo.Core;

public static partial class Extensions
{
#pragma warning disable CA1034 // Nested types should not be visible
    /// <summary>
    /// Extension methods for a span of <see cref="TagAttribute"/> values.
    /// </summary>
    /// <param name="attributes">The span of attributes to extend.</param>
    extension(ReadOnlySpan<TagAttribute> attributes)
    {
        /// <summary>
        /// Searches for the <see cref="TagAttribute"/> associated with the specified name.
        /// </summary>
        /// <param name="name">
        /// The attribute name, case-sensitive; use <c>"&lt;main&gt;"</c> or an empty span for the main attribute.
        /// </param>
        /// <param name="attribute">
        /// When this method returns, contains the attribute associated with <paramref name="name"/>, if it exists;
        /// otherwise, the default value.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a matching <see cref="TagAttribute"/> was found; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryFind(ReadOnlySpan<char> name, out TagAttribute attribute)
        {
            if (name.SequenceEqual("<main>"))
            {
                name = string.Empty;
            }

            for (var i = 0; i < attributes.Length; i++)
            {
                attribute = attributes[i];

                if (!attribute.IsDefault && attribute.IsNamed(name))
                {
                    return true;
                }
            }

            attribute = default;
            return false;
        }

        /// <summary>
        /// Searches for the <see cref="TagAttribute"/> with the specified name and attempts to parse it into a
        /// <typeparamref name="T"/> value.
        /// </summary>
        /// <typeparam name="T">The type of the value to parse.</typeparam>
        /// <param name="name">
        /// The attribute name, case-sensitive; use <c>"&lt;main&gt;"</c> or an empty span for the main attribute.
        /// </param>
        /// <param name="value">
        /// When this method returns, contains the parsed result from the attribute's value, if the attribute was found
        /// and it could be parsed; otherwise, the default value for <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a matching <see cref="TagAttribute"/> was found and parsed; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetValue<T>(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out T value)
            where T : ISpanParsable<T>
        {
            if (!attributes.TryFind(name, out var attribute)
                || !T.TryParse(attribute.Value, CultureInfo.InvariantCulture, out value))
            {
                value = default;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Searches for the <see cref="TagAttribute"/> with the specified name and attempts to parse it into a
        /// <typeparamref name="TEnum"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to parse the value into.</typeparam>
        /// <param name="name">
        /// The attribute name, case-sensitive; use <c>"&lt;main&gt;"</c> or an empty span for the main attribute.
        /// </param>
        /// <param name="ignoreCase">
        /// <see langword="true"/> to ignore casing when parsing the attribute's value; <see langword="false"/>
        /// otherwise.
        /// </param>
        /// <param name="value">
        /// When this method returns, contains the parsed enum value from the attribute's value, if the attribute was
        /// found and it could be parsed; otherwise, the default value for <typeparamref name="TEnum"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a matching <see cref="TagAttribute"/> was found and parsed; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetValue<TEnum>(ReadOnlySpan<char> name, bool ignoreCase, out TEnum value)
            where TEnum : struct, Enum
        {
            if (!attributes.TryFind(name, out var attribute)
                || !Enum.TryParse(attribute.Value, ignoreCase, out value))
            {
                value = default;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Searches and gets the value of the <see cref="TagAttribute"/> with the specified name.
        /// </summary>
        /// <param name="name">
        /// The attribute name, case-sensitive; use <c>"&lt;main&gt;"</c> or an empty span for the main attribute.
        /// </param>
        /// <param name="value">
        /// When this method returns, contains the value of the attribute, if the attribute was found; otherwise, an
        /// empty span.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a matching <see cref="TagAttribute"/> was found; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool TryGetValue(ReadOnlySpan<char> name, out ReadOnlySpan<char> value)
        {
            if (!attributes.TryFind(name, out var attribute))
            {
                value = [];
                return false;
            }

            value = attribute.Value;
            return true;
        }

        /// <summary>
        /// Searches for the <see cref="TagAttribute"/> with the specified name and attempts to parse it into a
        /// <see cref="Color"/> value.
        /// </summary>
        /// <param name="name">
        /// The attribute name, case-sensitive; use <c>"&lt;main&gt;"</c> or an empty span for the main attribute.
        /// </param>
        /// <param name="value">
        /// When this method returns, contains the parsed color from the attribute's value, if the attribute was found
        /// and it could be parsed; otherwise, the default value for <see cref="Color"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a matching <see cref="TagAttribute"/> was found and parsed; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The method accepts the same color formats as <see cref="Color.FromString(string, Color)"/>.
        /// </remarks>
        public bool TryGetValue(ReadOnlySpan<char> name, out Color value)
        {
            if (!attributes.TryFind(name, out var attribute) || !TryParseColor(attribute.Value, out value))
            {
                value = default;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Searches for the <see cref="TagAttribute"/> with the specified name and attempts to parse it into a
        /// <see cref="Vector2"/> value.
        /// </summary>
        /// <param name="name">
        /// The attribute name, case-sensitive; use <c>"&lt;main&gt;"</c> or an empty span for the main attribute.
        /// </param>
        /// <param name="sep">The separator character between the X and Y components.</param>
        /// <param name="value">
        /// When this method returns, contains the parsed vector from the attribute's value, if the attribute was found
        /// and it could be parsed; otherwise, the default value for <see cref="Vector2"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a matching <see cref="TagAttribute"/> was found and parsed; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The attribute's value must be in the format <c>"X<paramref name="sep"/>Y"</c>. For example, if
        /// <paramref name="sep"/> is a comma, then a valid value would be <c>"8,8"</c>.
        /// </para>
        /// <para>
        /// Whitespace around each vector component is trimmed before parsing.
        /// </para>
        /// </remarks>
        public bool TryGetValue(ReadOnlySpan<char> name, char sep, out Vector2 value)
        {
            if (!attributes.TryFind(name, out var attribute) || !TryParseVector2(attribute.Value, sep, out value))
            {
                value = default;
                return false;
            }

            return true;
        }
    }
#pragma warning restore CA1034 // Nested types should not be visible
}
