using Godot;
using System;

namespace Espejismo.Core.RichText.Effects;

/// <summary>
///   A text effect that vibrates the text randomly.
/// </summary>
/// <remarks>
///   <b>Parameters:</b>
///   <list type="bullet">
///     <item>
///       <term>Intensity (<c>intensity</c>)</term>
///       <description>The distance the text is displaced from its origin, in pixels.</description>
///     </item>
///   </list>
/// </remarks>
[GlobalClass, Tool]
public sealed partial class ShakeEffect : TextEffect
{
	[Export]
	private float _intensity = 1;
	
	/// <inheritdoc/>
	public override bool Process(ref GlyphTransform trans)
	{
		trans.Offset += new Vector2
		{
			X = Range(-_intensity, _intensity),
			Y = Range(-_intensity, _intensity)
		};

		return true;
	}

	/// <inheritdoc/>
	public override TextEffect Setup(ReadOnlySpan<TagAttribute> attributes)
	{
		if (!attributes.TryGetValue("intensity", out float intensity))
		{
			intensity = _intensity;
		}

		if (intensity != _intensity)
		{
			return new ShakeEffect { _intensity = intensity };
		}

		return this;
	}

	private static float Range(float min, float max)
	{
#pragma warning disable CA5394 // Do not use insecure randomness
		return min + (Random.Shared.NextSingle() * (max - min));
#pragma warning restore CA5394 // Do not use insecure randomness
	}
}
