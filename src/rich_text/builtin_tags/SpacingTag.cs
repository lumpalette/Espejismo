using System;

namespace Spectrum.RichText.BuiltinTags;

public class SpacingTag() : TagBehaviour("spacing", [])
{
	public override bool Begin(ParseContext context, ReadOnlySpan<TagProperty> properties)
	{
		//	var letter = FindProperty(properties, "letter");
		//	var line = FindProperty(properties, "line");
		return false;
	}
}
