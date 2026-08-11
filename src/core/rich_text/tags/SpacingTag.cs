using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
/// Represents a tag that changes the letter or line spacing of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
/// <b>Type:</b> Normal Element.
/// </para>
/// <para>
/// <b>Attributes:</b>
/// <list type="bullet">
///   <item>
///     <term><c>[letter]</c></term>
///     <description>Extra space added between letters, in pixels. Can be negative.</description>
///   </item>
///   <item>
///     <term><c>[line]</c></term>
///     <description>Extra space added between lines of text, in pixels. Can be negative.</description>
///   </item>
/// </list>
/// </para>
/// <para>
/// <b>Example:</b> <c>"&lt;spacing letter=3&gt;Ominous&lt;/spacing&gt; text."</c>
/// </para>
/// </remarks>
public sealed partial class SpacingTag() : TextTag(requiredAttributes: [])
{
	private const string OptionalLetter = "letter";
	private const string OptionalLine = "line";

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		var changed = false;
		var spcX = builder.TopStyle.LetterSpacing;
		var spcY = builder.TopStyle.LineSpacing;

		foreach (var attr in attributes)
		{
			if (attr.IsNamed(OptionalLetter) && int.TryParse(attr.Value, out var pvalue))
			{
				spcX = pvalue;
				changed = true;
			}
			else if (attr.IsNamed(OptionalLine) && int.TryParse(attr.Value, out pvalue))
			{
				spcY = pvalue;
				changed = true;
			}
		}

		if (!changed)
		{
			return false;
		}

		builder.PushStyle(builder.TopStyle with { LetterSpacing = spcX, LineSpacing = spcY });
		return true;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopStyle();
	}
}
