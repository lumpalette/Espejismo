using Godot;

namespace Spectrum.RichText.Shaping;

// A container for shaped text that shares the same alignment.
internal readonly struct ShapedBlock(TextServer ts,
	TextServer.Direction direction,
	TextServer.Orientation orientation,
	HorizontalAlignment alignment)
{
	public Rid Shaped { get; } = ts.CreateShapedText(direction, orientation);

	public HorizontalAlignment Alignment { get; } = alignment;
}
