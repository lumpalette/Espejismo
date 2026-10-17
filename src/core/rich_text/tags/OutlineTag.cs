using Godot;
using System;

namespace Espejismo.Core.RichText.Tags;

/// <summary>
/// A text tag that changes the outline properties of a specific segment of text.
/// </summary>
/// <remarks>
/// <b>Attributes:</b>
/// <list type="bullet">
///     <item>
///         <term><c>[size]</c></term>
///         <description>The size of the text outline, in pixels.</description>
///     </item>
///     <item>
///         <term><c>[color]</c></term>
///         <description>
///             The color of the outline, in the same color formats that <see cref="Color.FromString(string, Color)"/>
///             accepts.
///         </description>
///     </item>
/// </list>
/// </remarks>
[GlobalClass, Tool]
public sealed partial class OutlineTag : TextTag
{
    /// <inheritdoc/>
    public override bool IsVoid => false;

    /// <inheritdoc/>
    public override bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes)
    {
        var style = builder.TopStyle;

        var size = style.OutlineSize;
        var color = style.OutlineColor;

        if (attributes.TryGetValue("size", out ushort psize))
        {
            size = psize;
        }

        if (attributes.TryGetValue("color", out Color pcolor))
        {
            color = pcolor;
        }

        if (style.OutlineSize != size || style.OutlineColor != color)
        {
            _ = builder.PushStyle(style with { OutlineSize = size, OutlineColor = color });
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public override void End(TextBuilder builder)
    {
        _ = builder.PopStyle();
    }
}
