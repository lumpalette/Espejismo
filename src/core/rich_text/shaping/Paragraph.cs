using Godot;

namespace Espejismo.Core.RichText.Shaping;

// A container of shaped text that shares the same alignment.
internal readonly struct Paragraph(TextServer TS, TextServer.Direction direction, TextServer.Orientation orientation)
{
	public Rid Shaped { get; init; } = TS.CreateShapedText(direction, orientation);

	public HorizontalAlignment Alignment { get; init; }

	// Whether the shaped buffer has any content (runs, icons or markers).
	public bool HasContent { get; init; }
}
