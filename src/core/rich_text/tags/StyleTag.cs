using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
/// Represents a tag that changes the entire styling of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
/// <b>Type:</b> Normal Element.
/// </para>
/// <para>
/// <b>Attributes:</b>
/// <list type="bullet">
///   <item>
///     <term><c>id</c></term>
///     <description>
///       Identifier for the <see cref="StyleTemplate"/> to use, as defined in <see cref="ResourceDB"/>.
///     </description>
///   </item>
/// </list>
/// </para>
/// <para>
/// <b>Example:</b> <c>"Main style, and &lt;style id=fantasy&gt;fantasy style.&lt;/style&gt;"</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class StyleTag() : TextTag(requiredAttributes: [RequiredId])
{
	private const string RequiredId = "id";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		foreach (var attr in attributes)
		{
			if (attr.IsNamed(RequiredId))
			{
				if (!ResourceDB.TryGetResource<StyleTemplate>(attr.Value, out var template))
				{
					return false;
				}

				builder.PushStyle(template.Create());
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
}
