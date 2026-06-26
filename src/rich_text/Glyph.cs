using Godot;

namespace Spectrum.RichText;

public struct Glyph
{
	public Rid Font { get; }

	public int FontSize { get; }

	public Color Color { get; set; }

	public Vector2 Position { get; }

	public Vector2 Offset { get; set; }
}
