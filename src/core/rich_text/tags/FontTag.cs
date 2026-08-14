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
public sealed partial class FontTag : TextTag
{
	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var style = builder.TopStyle;

		var font = style.Font;
		var size = style.FontSize;

		if (attributes.TryGetValue("id", out ReadOnlySpan<char> id) && ResourceDB.TryGetFont(id, out var pfont))
		{
			font = pfont;
		}

		if (attributes.TryGetValue("size", out ushort psize))
		{
			size = psize;
		}

		if (style.Font != font || style.FontSize != size)
		{
			builder.PushStyle(style with { Font = font, FontSize = size });
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopStyle();
	}
}
