using Godot;
using Godot.Collections;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents the visual shape of a text character or icon.
/// </summary>
public readonly struct Glyph // 64 bytes pesa la marranota
{
    private static readonly StringName s_propStart = "start";
    private static readonly StringName s_propEnd = "end";
    private static readonly StringName s_propCount = "count";
    private static readonly StringName s_propRepeat = "repeat";
    private static readonly StringName s_propFlags = "flags";
    private static readonly StringName s_propOffset = "offset";
    private static readonly StringName s_propAdvance = "advance";
    private static readonly StringName s_propFontRid = "font_rid";
    private static readonly StringName s_propFontSize = "font_size";
    private static readonly StringName s_propIndex = "index";

    /// <summary>
    /// Gets the position at which the glyph's characters begin in the text's content.
    /// </summary>
    /// <remarks>
    /// Positions are counted in codepoints over the content of the text, that is, with markup removed and with each
    /// icon counting as one character.
    /// </remarks>
    public int Start { get; private init; }

    /// <summary>
    /// Gets the position immediately after the last character of the glyph in the text's content.
    /// </summary>
    public int End { get; private init; }

    /// <summary>
    /// Gets the number of glyphs in the grapheme cluster.
    /// </summary>
    /// <remarks>
    /// This is only set in the first glyph of the grapheme cluster.
    /// </remarks>
    public byte Count { get; private init; }

    /// <summary>
    /// Gets the number of times the glyph is drawn. Each draw advances the pen position by <see cref="Advance"/>.
    /// </summary>
    public byte Repeat { get; private init; }

    /// <summary>
    /// Gets a combination of <see cref="TextServer.GraphemeFlag"/> values describing the characteristics of the glyph.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is only set in the first glyph of the grapheme cluster.
    /// </para>
    /// <para>
    /// The property is stored as a <see langword="ushort"/> instead of a <see cref="TextServer.GraphemeFlag"/> to keep
    /// the struct small, since the enum’s underlying type is 8 bytes wide.
    /// </para>
    /// </remarks>
    public ushort Flags { get; private init; }

    /// <summary>
    /// Gets the offset to the glyph's origin from the baseline.
    /// </summary>
    public Vector2 Offset { get; private init; }

    /// <summary>
    /// Gets the distance to the next glyph along the baseline.
    /// </summary>
    public float Advance { get; private init; }

    /// <summary>
    /// Gets the <see cref="TextServer"/> font resource used for the glyph.
    /// </summary>
    public Rid FontRid { get; private init; }

    /// <summary>
    /// Gets the size of the <see cref="FontRid"/>, in pixels.
    /// </summary>
    public ushort FontSize { get; private init; }

    /// <summary>
    /// Gets the index of the glyph in the source <see cref="FontRid"/>.
    /// </summary>
    public int Index { get; private init; }

    /// <summary>
    /// Gets the texture associated with the icon.
    /// </summary>
    public Texture2D? IconTexture { get; private init; }

    /// <summary>
    /// Gets the size of the rectangle used for drawing the <see cref="IconTexture"/>.
    /// </summary>
    public Vector2 IconSize { get; private init; }

    /// <summary>
    /// Gets the group of style properties associated with the glyph.
    /// </summary>
    public GlyphStyle Style { get; private init; }

    internal static Glyph CreateChar(Dictionary gl, GlyphStyle style)
    {
        return new Glyph
        {
            Start = (int)gl[s_propStart],
            End = (int)gl[s_propEnd],

            Count = (byte)gl[s_propCount],
            Repeat = (byte)gl[s_propRepeat],
            Flags = (ushort)gl[s_propFlags],

            Offset = (Vector2)gl[s_propOffset],
            Advance = (float)gl[s_propAdvance],

            FontRid = (Rid)gl[s_propFontRid],
            FontSize = (ushort)gl[s_propFontSize],
            Index = (int)gl[s_propIndex],
            Style = style,
        };
    }

    internal static Glyph CreateIcon(Dictionary gl, GlyphStyle style, Texture2D tex, Rect2 rect)
    {
        return new Glyph
        {
            Start = (int)gl[s_propStart],
            End = (int)gl[s_propEnd],

            Count = (byte)gl[s_propCount],
            Repeat = (byte)gl[s_propRepeat],
            Flags = (ushort)(TextServer.GraphemeFlag.Valid | TextServer.GraphemeFlag.EmbeddedObject),

            Offset = rect.Position,
            Advance = (float)gl[s_propAdvance],

            IconTexture = tex,
            IconSize = rect.Size,
            Style = style,
        };
    }
}
