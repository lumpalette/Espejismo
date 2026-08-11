using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
/// Represents a tag that changes the color of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
/// <b>Type:</b> Normal Element.
/// </para>
/// <para>
/// <b>Attributes:</b>
/// <list type="bullet">
///   <item>
///     <term><c>value</c></term>
///     <description>
///       The color to apply, which can be either the name of one of the colors in the <see cref="Colors"/> class,
///       case-insensitive, or a 3, 4, 6 or 8-digit HTML color code, optionally prefixed by a '#' character.
///     </description>
///   </item>
/// </list>
/// </para>
/// <para>
/// <b>Example:</b> <c>"The following word &lt;color value=red&gt;is&lt;/color&gt; red."</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class ColorTag() : TextTag(requiredAttributes: [RequiredValue])
{
	private const string RequiredValue = "value";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		foreach (var attr in attributes)
		{
			if (attr.IsNamed(RequiredValue))
			{
				if (!TryParseColor(attr.Value.ToString(), out Color color))
				{
					return false;
				}

				builder.PushStyle(builder.TopStyle with { Color = color });
				return true;
			}
		}

		return false;
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
}
