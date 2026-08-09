using Godot;
using System;

namespace Espejismo.Core.RichText.BuiltinTags;

/// <summary>
/// Represents a tag that changes the entire styling of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
/// Syntax: <c>&lt;style id="id"&gt;...&lt;/style&gt;</c>
/// </para>
/// <para>
/// Where <c>id</c> is the identifier for the <see cref="StyleTemplate"/>, as defiend in <see cref="ResourceDB"/>.
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class StyleTag() : TextTag([RequiredId])
{
	private const string RequiredId = "id";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var idA = FindAttribute(attributes, RequiredId);

		if (!ResourceDB.TryGetResource<StyleTemplate>(idA.Value, out var template))
		{
			return false;
		}

		builder.PushStyle(template.Create());
		return true;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopStyle();
	}
}
