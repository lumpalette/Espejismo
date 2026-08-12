using Godot;
using System;

namespace Espejismo.Core.RichText.Effects;

/// <summary>
///   Represents an effect that applies a sequence of <see cref="TextEffect"/> instances to a <see cref="Glyph"/>.
/// </summary>
[GlobalClass]
public sealed partial class CompositeEffect : TextEffect
{
	[Export]
	private TextEffect?[] _effects = [];

	/// <summary>
	///   Initializes a new instance of the <see cref="CompositeEffect"/> class that is empty.
	/// </summary>
	public CompositeEffect()
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="CompositeEffect"/> class using the specified sequence of
	///   <see cref="TextEffect"/> instances.
	/// </summary>
	/// <param name="effects">
	///   The effects to merge together.
	/// </param>
	public CompositeEffect(params ReadOnlySpan<TextEffect?> effects)
	{
		_effects = effects.ToArray();
	}

	/// <summary>
	///   Gets the <see cref="TextEffect"/> instances that make up this effect.
	/// </summary>
	public ReadOnlySpan<TextEffect?> Effects => _effects;

	/// <summary>
	///   Combines two <see cref="TextEffect"/> instances as a single effect.
	/// </summary>
	/// <param name="a">
	///   The first effect to combine.
	/// </param>
	/// <param name="b">
	///   The second effect to combine.
	/// </param>
	/// <returns>
	///   A new <see cref="CompositeEffect"/>, flattening any nested composite. If either <paramref name="a"/> or
	///   <paramref name="b"/> are <see langword="null"/>, the other one is returned. If both are not specified,
	///   returns <see langword="null"/>.
	/// </returns>
	public static TextEffect? Combine(TextEffect? a, TextEffect? b)
	{
		// Return the same effect if the other one is null.
		if (a is null)
		{
			return b;
		}

		if (b is null)
		{
			return a;
		}

		// Both effects are not null, so we need to merge them.
		var compA = a as CompositeEffect;
		var compB = b as CompositeEffect;
		
		if (compA is not null && compB is not null)
		{
			return new CompositeEffect([.. compA.Effects, ..compB.Effects]);
		}

		if (compA is not null)
		{
			return new CompositeEffect([.. compA.Effects, b]);
		}
		
		if (compB is not null)
		{
			return new CompositeEffect([a, .. compB.Effects]);
		}
		
		return new CompositeEffect(a, b);
	}

	/// <inheritdoc/>
	public override void Process(ref GlyphTransform trans)
	{
		foreach (var effect in Effects)
		{
			effect?.Process(ref trans);
		}
	}

	/// <inheritdoc/>
	public override TextEffect Setup(ReadOnlySpan<TagAttribute> attributes)
	{
		return this;
	}
}
