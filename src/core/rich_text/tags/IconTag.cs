using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
/// Represents a self-closing tag that inserts an icon at the tag's position.
/// </summary>
/// <remarks>
/// <para>
/// <b>Type:</b> Void Element.
/// </para>
/// <para>
/// <b>Attributes:</b>
/// <list type="bullet">
///   <item>
///     <term><c>id</c></term>
///     <description>
///       Identifier for the <see cref="Texture2D"/> to insert, as defined in <see cref="ResourceDB"/>
///     </description>
///   </item>
///   <item>
///     <term><c>[align]</c></term>
///     <description>
///       The alignment of the icon, which can be <c>top</c>, <c>center</c> or <c>bottom</c>.
///     </description>
///   </item>
///   <item>
///     <term><c>[size]</c></term>
///     <description>The dimensions of the texture rect, formatted as <c>WxH</c>.</description>
///   </item>
/// </list>
/// </para>
/// <para>
/// <b>Example:</b> <c>"Smily &lt;icon id=smile/&gt; face!"</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class IconTag() : TextTag(requiredAttributes: [RequiredId])
{
	private const string RequiredId = "id";
	private const string OptionalAlign = "align";
	private const string OptionalSize = "size";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		Texture2D? tex = null;
		var align = InlineAlignment.Center;
		var size = Vector2.Zero;

		foreach (var attr in attributes)
		{
			switch (attr.Name)
			{
				case RequiredId:
					if (!ResourceDB.TryGetResource(attr.Value, out tex))
					{
						return false;
					}
					break;
				case OptionalAlign:
					if (!TryParseAlignment(attr.Value, out align))
					{
						return false;
					}
					break;
				case OptionalSize:
					if (!TryParseSize(attr.Value, out size))
					{
						return false;
					}
					break;
			}
		}

		if (tex is null)
		{
			return false;
		}

		if (size == Vector2.Zero)
		{
			size = tex.GetSize();
		}

		builder.AppendIcon(tex, align, size);
		return true;
	}

	private static bool TryParseAlignment(ReadOnlySpan<char> s, out InlineAlignment alignment)
	{
		switch (s)
		{
			case "top":
				alignment = InlineAlignment.Top;
				return true;
			case "center":
				alignment = InlineAlignment.Center;
				return true;
			case "bottom":
				alignment = InlineAlignment.Bottom;
				return true;
		}

		alignment = default;
		return false;
	}

	private static bool TryParseSize(ReadOnlySpan<char> s, out Vector2 size)
	{
		var multiplyIndex = s.IndexOf('x');

		if (multiplyIndex == -1)
		{
			size = Vector2.Zero;
			return false;
		}

		var wSpan = s[..multiplyIndex].Trim();
		var hSpan = s[(multiplyIndex + 1)..].Trim();

		if (!int.TryParse(wSpan, out var w) || !int.TryParse(hSpan, out var h))
		{
			size = Vector2.Zero;
			return false;
		}

		size = new Vector2(w, h);
		return true;
	}
}
