using Godot;
using System;

namespace Espejismo.Core.RichText.BuiltinTags;

/// <summary>
/// Represents a tag that changes the font properties of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
/// Syntax: <c>&lt;font [id="id"] [size="size"]&gt;...&lt;/font&gt;</c>
/// </para>
/// <para>
/// Where:
/// <list type="bullet">
///   <item>
///     <term><c>id</c></term>
///     <description>The identifier for the <see cref="Font"/>, as defined in <see cref="ResourceDB"/>.</description>
///   </item>
///   <item>
///     <term><c>size</c></term>
///     <description>The size of the font, in pixels. Must be greater than 0.</description>
///   </item>
/// </list>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class FontTag() : TextTag([])
{
	private const string OptionalId = "id";
	private const string OptionalSize = "size";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var idA = FindAttribute(attributes, OptionalId);
		var sizeA = FindAttribute(attributes, OptionalSize);

		var font = builder.TopStyle.Font;
		var size = builder.TopStyle.FontSize;

		if (idA.IsDefined && !ResourceDB.TryGetResource(idA.Value, out font))
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
