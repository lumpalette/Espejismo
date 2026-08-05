using Godot;

namespace Espejismo.Core.RichText.Shaping;

// Helper structure that describes the positioning of the shaped glyphs.
internal readonly struct LayoutOptions
{
	public required float MaxWidth { get; init; }

	public required HorizontalAlignment BaseAlignment { get; init; }

	public required TextServer.Direction Direction { get; init; }

	public required TextServer.Orientation Orientation { get; init; }
}
