using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.RichText;

/// <summary>
/// Contains extension methods for the various types defined in this namespace.
/// </summary>
public static class Extensions
{
#pragma warning disable CA1034 // Nested types should not be visible
	extension(ReadOnlySpan<TagAttribute> attributes)
#pragma warning restore CA1034 // Nested types should not be visible
	{
		/// <summary>
		///   Searches for the <see cref="TagAttribute"/> associated with the specified name.
		/// </summary>
		/// <param name="name">
		///   The name of the attribute to search.
		/// </param>
		/// <param name="attribute">
		///   When this method returns, contains the attribute associated with <paramref name="name"/>, if exists;
		///   otherwise, <see langword="false"/>.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if a matching <see cref="TagAttribute"/> was found; otherwise,
		///   <see langword="false"/>.
		/// </returns>
		public bool TryFind(ReadOnlySpan<char> name, out TagAttribute attribute)
		{
			for (var i = 0; i < attributes.Length; i++)
			{
				attribute = attributes[i];

				if (attribute.IsNamed(name))
				{
					return true;
				}
			}

			attribute = default;
			return false;
		}

		/// <summary>
		///   Searches for the <see cref="TagAttribute"/> with the specified name and attempts to parse it into a
		///   <typeparamref name="T"/> value.
		/// </summary>
		/// <typeparam name="T">
		///   The type of the value to parse.
		/// </typeparam>
		/// <param name="name">
		///   The name of the attribute to search.
		/// </param>
		/// <param name="value">
		///   When this method returns, contains the parsed result from the attribute's value, if the attribute was
		///   found and it could be parsed; otherwise, the default value for <typeparamref name="T"/>.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if a matching <see cref="TagAttribute"/> was successfully found and parsed;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public bool TryGetValue<T>(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out T value)
			where T : ISpanParsable<T>
		{
			if (!attributes.TryFind(name, out var attribute) || !T.TryParse(attribute.Value, null, out value))
			{
				value = default;
				return false;
			}

			return true;
		}

		/// <summary>
		///   Searches for the <see cref="TagAttribute"/> with the specified name and attempts to parse it into a
		///   <typeparamref name="TEnum"/> value.
		/// </summary>
		/// <typeparam name="TEnum">
		///   The enum type to parse the value into.
		/// </typeparam>
		/// <param name="name">
		///   The name of the attribute to search.
		/// </param>
		/// <param name="ignoreCase">
		///   <see langword="true"/> to ignore casing when parsing the attribute's value; <see langword="false"/> otherwise.
		/// </param>
		/// <param name="value">
		///   When this method returns, contains the parsed enum value from the attribute's value, if the attribute was
		///   found and it could be parsed; otherwise, the default value for <typeparamref name="TEnum"/>.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if a matching <see cref="TagAttribute"/> was successfully found and parsed;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public bool TryGetValue<TEnum>(ReadOnlySpan<char> name, bool ignoreCase, out TEnum value)
			where TEnum : struct, Enum
		{
			if (!attributes.TryFind(name, out var attribute) || !Enum.TryParse(attribute.Value, ignoreCase, out value))
			{
				value = default;
				return false;
			}

			return true;
		}

		/// <summary>
		///   Searches and gets the value of the <see cref="TagAttribute"/> with the specified name
		/// </summary>
		/// <param name="name">
		///   The name of the attribute to search.
		/// </param>
		/// <param name="value">
		///   When this method returns, contains the value of the attribute, if found; otherwise, the default value for
		///   <see cref="ReadOnlySpan{T}"/>.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if a matching <see cref="TagAttribute"/> was found; otherwise,
		///   <see langword="false"/>.
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
		///   Searches for the <see cref="TagAttribute"/> with the specified name and attempts to parse it into a
		///   <see cref="Color"/> value.
		/// </summary>
		/// <param name="name">
		///   The name of the attribute to search.
		/// </param>
		/// <param name="value">
		///   When this method returns, contains the parsed color from the attribute's value, if the attribute was
		///   found and it could be parsed; otherwise, the default value for <see cref="Color"/>.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if a matching <see cref="TagAttribute"/> was successfully found and parsed;
		///   otherwise, <see langword="false"/>.
		/// </returns>
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
		///   Searches for the <see cref="TagAttribute"/> with the specified name and attempts to parse it into a
		///   <see cref="Vector2"/> value.
		/// </summary>
		/// <remarks>
		///   The attribute's value must be in the format <c>"X<paramref name="sep"/>Y"</c>. For example, if
		///   <paramref name="sep"/> = <c>','</c>, then a valid value would be <c>"8,8"</c>.
		/// </remarks>
		/// <param name="name">
		///   The name of the attribute to search.
		/// </param>
		/// <param name="sep">
		///   The separator character between the X and Y components.
		/// </param>
		/// <param name="value">
		///   When this method returns, contains the parsed vector from the attribute's value, if the attribute was
		///   found and it could be parsed; otherwise, the default value for <see cref="Vector2"/>.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if a matching <see cref="TagAttribute"/> was successfully found and parsed;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public bool TryGetValue(ReadOnlySpan<char> name, char sep, out Vector2 value)
		{
			if (!attributes.TryFind(name, out var attribute) || !TryParseVector2(attribute.Value, sep, out value))
			{
				value = default;
				return false;
			}

			return true;
		}

		private static bool TryParseVector2(ReadOnlySpan<char> s, char sep, out Vector2 result)
		{
			var sepIndex = s.IndexOf(sep);

			if (sepIndex == -1)
			{
				result = Vector2.Zero;
				return false;
			}

			var xSpan = s[..sepIndex].Trim();
			var ySpan = s[(sepIndex + 1)..].Trim();

			if (!float.TryParse(xSpan, out var x) || !float.TryParse(ySpan, out var y))
			{
				result = Vector2.Zero;
				return false;
			}

			result = new Vector2(x, y);
			return true;
		}

		private static bool TryParseColor(ReadOnlySpan<char> s, out Color color)
		{
			var fallback = new Color(-1f, -1f, -1f, -1f);
			color = Color.FromString(s.ToString(), fallback);

			if (color == fallback)
			{
				color = default;
				return false;
			}

			return true;
		}
	}
}
