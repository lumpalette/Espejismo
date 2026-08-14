using Godot;

namespace Espejismo.Core.RichText.Effects;

[GlobalClass]
public sealed partial class GradientEffect : TextEffect
{
	[Export]
	private Gradient _gradient = new();

	public override void Process(ref GlyphTransform trans)
	{
		throw new System.NotImplementedException();
	}
}
