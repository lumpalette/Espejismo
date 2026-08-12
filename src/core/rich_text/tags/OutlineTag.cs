using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
///   Represents a tag that changes the outline properties of a specific segment of text.
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
///       <description>The size of the text outline, in pixels.</description>
///     </item>
///     <item>
///       <term><c>[color]</c></term>
///       <description>
///         The color of the text outline, which can be either the name of one of the colors in the <see cref="Colors"/>
///         class, case-insensitive, or a 3, 4, 6 or 8-digit HTML color code, optionally prefixed by a '#' character.
///       </description>
///     </item>
///   </list>
/// </para>
/// <para>
///   <b>Example:</b> <c>"&lt;outline size=8x color=black&gt;Black outline.&lt;/outline&gt;"</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class OutlineTag() : TextTag(requiredAttributes: [])
{
	private const string OptionalSize = "size";
	private const string OptionalColor = "color";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var changed = false;
		var size = builder.TopStyle.OutlineSize;
		var color = builder.TopStyle.OutlineColor;
		
		foreach (var attr in attributes)
		{
			if (attr.IsNamed(OptionalSize))
			{
				if (!ushort.TryParse(attr.Value, out var psize))
				{
					return false;
				}

				size = psize;
				changed = true;
			}
			else if (attr.IsNamed(OptionalColor))
			{
				if (!TryParseColor(attr.Value.ToString(), out Color pcolor))
				{
					return false;
				}

				color = pcolor;
				changed = true;
			}
		}

		if (!changed)
		{
			return false;
		}

		builder.PushStyle(builder.TopStyle with { OutlineSize = size, OutlineColor = color });
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
}
