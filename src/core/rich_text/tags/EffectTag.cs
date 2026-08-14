using Espejismo.Core.RichText.Effects;
using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
///   Represents a tag that changes the visual effect of a specific segment of text.
/// </summary>
/// <remarks>
/// <para>
///   <b>Type:</b> Normal Element.
/// </para>
/// <para>
///   <b>Attributes:</b> Varies (depends on the specific effect).
/// </para>
/// <para>
///   <b>Example:</b> <c>"&lt;shake&gt;This&lt;/shake&gt; word is shaking."</c>
/// </para>
/// </remarks>
[GlobalClass]
public sealed partial class EffectTag : TextTag
{
	/// <summary>
	/// Gets the effect applied by the tag, configured through the editor.
	/// </summary>
	[Export]
	public TextEffect? Effect { get; private set; }

	/// <inheritdoc/>
	public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
	{
		if (Effect is null)
		{
			return false;
		}

		var current = builder.TopStyle.Effect;
		var composite = CompositeEffect.Combine(current, Effect.Setup(attributes));
		
		builder.PushStyle(builder.TopStyle with { Effect = composite });
		return true;
	}

	/// <inheritdoc/>
	public override void End(TextBuilder builder)
	{
		builder.PopStyle();
	}
}
