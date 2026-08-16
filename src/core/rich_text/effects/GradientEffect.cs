using Godot;

namespace Espejismo.Core.RichText.Effects;

[GlobalClass]
public sealed partial class GradientEffect : TextEffect
{
	[Export]
	private Gradient _gradient = new();
	[Export]
	private float _frequency;
	[Export]
	private float _speed;

	public override void Process(ref GlyphTransform trans)
	{
		var pos = (trans.LineProgress * _frequency) + (trans.Time * _speed);
		var mod = Mathf.PosMod(pos, 1f);

		trans.Color = _gradient.Sample(mod);
	}
}
