using Godot;
using System;

namespace Espejismo.Core.RichText.BuiltinTags;

/// <summary>
/// Represents a tag that changes the font properties of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
/// Syntax: <c>&lt;font [name="name"] [size="size"]&gt;...&lt;/font&gt;</c>
/// </para>
/// <para>
/// Where:<br/>
/// • <c>name</c> is the name of the <see cref="Font"/> resource, as defined in <see cref="ResourceDB"/>.<br/>
/// • <c>size</c> is the size of the font, in pixels. Must be greater than 0.
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class FontTag() : TextTag([])
{
	private const string OptionalName = "name";
	private const string OptionalSize = "size";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var nameA = FindAttribute(attributes, OptionalName);
		var sizeA = FindAttribute(attributes, OptionalSize);

		var font = builder.TopStyle.Font;
		var size = builder.TopStyle.FontSize;

		if (nameA.IsDefined && !ResourceDB.TryGetResource(nameA.Value, out font))
		{
			return false;
		}

		if (sizeA.IsDefined)
		{
			if (!ushort.TryParse(sizeA.Value, out var psize))
			{
				return false;
			}

			if (psize != 0)
			{
				size = psize;
			}
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
