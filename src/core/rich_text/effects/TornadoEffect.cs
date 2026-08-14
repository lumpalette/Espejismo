using Godot;
using System;

namespace Espejismo.Core.RichText.Effects;

/// <summary>
///   Represents an effect that moves the text around a circle.
/// </summary>
[GlobalClass]
public sealed partial class TornadoEffect : TextEffect
{
	[Export]
	private float _radius = 2.5f;
	[Export]
	private float _frequency = 4f;
	[Export]
	private float _spacing = 2f;

	/// <inheritdoc/>
	public override void Process(ref GlyphTransform trans)
	{
		var angle = (trans.Time * _frequency) + (trans.LineProgress * _spacing);

		var x = (float)Math.Sin(angle) * _radius;
		var y = (float)Math.Cos(angle) * _radius;

		trans.Offset = new Vector2(x, y);
	}

	/// <inheritdoc/>
	public override TextEffect Setup(ReadOnlySpan<TagAttribute> attributes)
	{
		if (!attributes.TryGetValue("radius", out float radius))
		{
			radius = _radius;
		}

		if (!attributes.TryGetValue("freq", out float freq))
		{
			freq = _frequency;
		}

		if (!attributes.TryGetValue("spacing", out float spacing))
		{
			spacing = _spacing;
		}

		if (radius != _radius || freq != _frequency || spacing != _spacing)
		{
			return new TornadoEffect
			{
				_radius = radius,
				_frequency = freq,
				_spacing = spacing
			};
		}

		return this;
	}
}
