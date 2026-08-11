using Godot;

namespace Espejismo.Core.RichText.Shaping;

internal readonly struct ResolvedStyle
{
	public required Font Font { get; init; }

	public required ushort FontSize { get; init; }

	// The reasong this is here is because otherwise there is no other (cheaper) way to differentiate between the
	// line's descent and leading, since FontVariation combines them into one property (TextServer.font_get_descent).
	public required int LineSpacing { get; init; }

	public required GlyphStyle Style { get; init; }
}
