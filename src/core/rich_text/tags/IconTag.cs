using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
///   Represents a self-closing tag that inserts an icon at the tag's position.
/// </summary>
/// <remarks>
/// <para>
///   <b>Type:</b> Void Element.
/// </para>
/// <para>
///   <b>Attributes:</b>
///   <list type="bullet">
///     <item>
///       <term><c>id</c></term>
///       <description>
///         Identifier for the <see cref="Texture2D"/> to insert, as defined in <see cref="ResourceDB"/>
///       </description>
///     </item>
///     <item>
///       <term><c>[align]</c></term>
///       <description>
///         One of the values in the <see cref="InlineAlignment"/> enum, case-insensitive.
///       </description>
///     </item>
///     <item>
///       <term><c>[size]</c></term>
///       <description>The dimensions of the texture rect, formatted as <c>WxH</c>.</description>
///     </item>
///   </list>
/// </para>
/// <para>
///   <b>Example:</b> <c>"Smily &lt;icon id=smile/&gt; face!"</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class IconTag : TextTag
{
	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		if (!attributes.TryFind("id", out var idAttr) || !ResourceDB.TryGetTexture(idAttr.Value, out var tex))
		{
			return false;
		}
		
		if (!attributes.TryGetValue("align", ignoreCase: true, out InlineAlignment align))
		{
			align = InlineAlignment.Center;
		}

		if (!attributes.TryGetValue("size", sep: 'x', out Vector2 size))
		{
			size = tex.GetSize();
		}

		builder.AppendIcon(tex, align, size);
		return true;
	}
}
