using Godot;

namespace Espejismo.Core.RichText.Shaping;

// Union-like struct that holds every type of data associated to a shape item used by the shaping engine.
internal readonly struct ShapeItem
{
	public static ShapeItem CreateRun(string text, in TextStyle style)
	{
		return new ShapeItem { Type = ShapeItemType.Run, Text = text, Style = style };
	}

	public static ShapeItem CreateTexture(Texture2D tex, InlineAlignment alignment, TextStyle style)
	{
		return new ShapeItem { Type = ShapeItemType.Texture, Texture = tex, TextureAlignment = alignment, Style = style };
	}

	public static ShapeItem CreateMarker(string name, TagAttribute[] attributes)
	{
		return new ShapeItem { Type = ShapeItemType.Marker, Text = name, Attributes = attributes };
	}

	public static ShapeItem CreateBreak()
	{
		return new ShapeItem { Type = ShapeItemType.Break };
	}

	public static ShapeItem CreateAlign(HorizontalAlignment? alignment)
	{
		return new ShapeItem { Type = ShapeItemType.Align, Alignment = alignment };
	}

	public ShapeItemType Type { get; private init; }

	// For runs, represents the text of the run; for markers, represents the name of the marker.
	public string Text { get; private init; }

	public TextStyle Style { get; private init; }

	public Texture2D Texture { get; private init; }

	public InlineAlignment TextureAlignment { get; private init; }

	public TagAttribute[] Attributes { get; private init; }

	public HorizontalAlignment? Alignment { get; private init; }

	public ShapeItem WithStyle(in TextStyle style)
	{
		return this with { Style = style };
	}
}
