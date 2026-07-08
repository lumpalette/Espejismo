using Godot;
using System;
using System.Diagnostics;

namespace Spectrum.RichText.BuiltinTags;

/// <summary>
///   Represents a tag that changes the horizontal alignment of a text paragraph.
/// </summary>
/// <remarks>
///   <para>
///     Syntax:<br/>
///     <c>&lt;alignment>...&lt;/alignment></c>
///   </para>
///   <para>
///     where:<br/>
///     • <c>alignment</c> refers the name of the <paramref name="alignment"/> identifier, in lowercase.
///   </para>
/// </remarks>
/// <param name="alignment">
///   The alignment associated to the tag.
/// </param>
public class AlignmentTag(HorizontalAlignment alignment) : TagBehaviour(GetAlignmentName(alignment), [])
{
	public override bool Begin(ParseContext context, ReadOnlySpan<TagProperty> properties)
	{
		context.BeginAlignment(alignment);
		return true;
	}

	public override void End(ParseContext context)
	{
		context.EndAlignment();
	}

	private static string GetAlignmentName(HorizontalAlignment alignment)
	{
		return alignment switch
		{
			HorizontalAlignment.Left => "left",
			HorizontalAlignment.Center => "center",
			HorizontalAlignment.Right => "right",
			HorizontalAlignment.Fill => "fill",
			_ => throw new UnreachableException("how the fukc")
		};
	}
}
