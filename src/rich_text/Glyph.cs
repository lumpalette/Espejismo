using Godot;

namespace Spectrum.RichText;

public struct Glyph
{
	public required int Index { get; init; }

	public required Rid Font { get; init; }

	public required int FontSize { get; init; }

	public required Color Color { get; set; }

	public required Vector2 Position { get; init; }

	public required Vector2 Offset { get; set; }

	public required byte Repeat { get; init; }
}
