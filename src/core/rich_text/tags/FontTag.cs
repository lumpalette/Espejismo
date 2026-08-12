using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
///   Represents a tag that changes the font properties of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
///   <b>Type:</b> Normal Element.
/// </para>
/// <para>
///   <b>Attributes:</b>
///   <list type="bullet">
///     <item>
///       <term><c>[id]</c></term>
///       <description>Identifier for the new <see cref="Font"/>, as defined in <see cref="ResourceDB"/>.</description>
///     </item>
///     <item>
///       <term><c>[size]</c></term>
///       <description>The size of the font, in pixels. Must be greater than 0.</description>
///     </item>
///   </list>
/// </para>
/// <para>
///   <b>Example:</b> <c>"Font 1,&lt;font id=example&gt;\nFont 2.&lt;/font&gt;</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class FontTag() : TextTag(requiredAttributes: [])
{
	private const string OptionalId = "id";
	private const string OptionalSize = "size";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var changed = false;
		var font = builder.TopStyle.Font;
		var size = builder.TopStyle.FontSize;

		foreach (var attr in attributes)
		{
			if (attr.IsNamed(OptionalId))
			{
				if (!ResourceDB.TryGetFont(attr.Value, out font))
				{
					return false;
				}

				changed = true;
			}
			else if (attr.IsNamed(OptionalSize))
			{
				if (!ushort.TryParse(attr.Value, out var psize))
				{
					return false;
				}
				
				size = psize;
				changed = true;
			}
		}

		if (!changed)
		{
			return false;
		}

		builder.PushStyle(builder.TopStyle with { Font = font, FontSize = size });
		return true;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopStyle();
	}
}
