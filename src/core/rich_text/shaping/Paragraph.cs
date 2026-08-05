using Godot;

namespace Espejismo.Core.RichText.Shaping;

// A container of shaped text that shares the same alignment.
internal readonly struct Paragraph(Rid shaped, HorizontalAlignment alignment, bool isVisible)
{
	public Rid Shaped { get; } = shaped;

	public HorizontalAlignment Alignment { get; } = alignment;

	public bool HasContent { get; } = isVisible;
}
