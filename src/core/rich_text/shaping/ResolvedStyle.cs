using Godot;

namespace Espejismo.Core.RichText.Shaping;

internal readonly struct ResolvedStyle
{
	public required Font Font { get; init; }

	public required ushort FontSize { get; init; }

	public required GlyphStyle Style { get; init; }
}
