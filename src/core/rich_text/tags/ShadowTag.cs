using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
///   Represents a tag that changes the shadow properties of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
///   <b>Type:</b> Normal Element.
/// </para>
/// <para>
///   <b>Attributes:</b>
///   <list type="bullet">
///     <item>
///       <term><c>[size]</c></term>
///       <description>The size of the shadow effect, in pixels.</description>
///     </item>
///     <item>
///       <term><c>[color]</c></term>
///       <description>
///         The color of the shadow effect, which can be either the name of one of the colors in the <see cref="Colors"/>
///         class, case-insensitive, or a 3, 4, 6 or 8-digit HTML color code, optionally prefixed by a '#' character.
///       </description>
///     </item>
///     <item>
///       <term><c>[offset]</c></term>
///       <description>
///         The displacement of the shadow effect relative to the text's position, formatted as <c>X,Y</c>.
///       </description>
///     </item>
///   </list>
/// </para>
/// <para>
///   <b>Example:</b> <c>"&lt;shadow size=8 color=black offset=2,2&gt;Black shadow.&lt;/shadow&gt;"</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class ShadowTag() : TextTag(requiredAttributes: [])
{
	private const string OptionalSize = "size";
	private const string OptionalColor = "color";
	private const string OptionalOffset = "offset";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var changed = false;
		var size = builder.TopStyle.ShadowSize;
		var color = builder.TopStyle.ShadowColor;
		var offset = builder.TopStyle.ShadowOffset;
		
		foreach (var attr in attributes)
		{
			switch (attr.Name)
			{
				case OptionalSize:
					if (!ushort.TryParse(attr.Value, out var psize))
					{
						return false;
					}

					size = psize;
					changed = true;
					break;
				case OptionalColor:
					if (!TryParseColor(attr.Value.ToString(), out Color pcolor))
					{
						return false;
					}

					color = pcolor;
					changed = true;
					break;
				case OptionalOffset:
					if (!TryParseOffset(attr.Value, out var poffset))
					{
						return false;
					}

					offset = poffset;
					changed = true;
					break;
			}
		}

		if (!changed)
		{
			return false;
		}

		builder.PushStyle(builder.TopStyle with { ShadowSize = size, ShadowColor = color, ShadowOffset = offset });
		return true;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopStyle();
	}

	private static bool TryParseColor(string s, out Color color)
	{
		var fallback = new Color(-1f, -2f, -3f, -4f);
		color = Color.FromString(s, fallback);
		return color != fallback;
	}

	private static bool TryParseOffset(ReadOnlySpan<char> s, out Vector2 offset)
	{
		var commaIndex = s.IndexOf(',');

		if (commaIndex == -1)
		{
			offset = Vector2.Zero;
			return false;
		}

		var xSpan = s[..commaIndex].Trim();
		var ySpan = s[(commaIndex + 1)..].Trim();

		if (!int.TryParse(xSpan, out var x) || !int.TryParse(ySpan, out var y))
		{
			offset = Vector2.Zero;
			return false;
		}

		offset = new Vector2(x, y);
		return true;
	}
}
