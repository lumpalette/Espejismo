using Godot;

namespace Espejismo.Core.RichText;

internal abstract record class ShapeItem
{
    private ShapeItem()
    {
    }

    // Segment of text that shares the same style.
    public sealed record class Run(string Text, TextStyle Style, TextEffect[] Effects) : ShapeItem;

    // Embedded object containing a reference of a texture.
    public sealed record class Icon(Texture2D Texture, InlineAlignment Alignment, Vector2 Size, TextStyle Style, TextEffect[] Effects) : ShapeItem;

    // Embedded object containing arbitrary payload, described as a name/attributes pair.
    public sealed record class Marker(string Name, TagAttribute[] Attributes) : ShapeItem;

    // Marker indicating a paragraph break should occur at this position.
    public sealed record class LineBreak : ShapeItem;

    // Marker indicating a change in the paragraph's alignment.
    public sealed record class Align(HorizontalAlignment Alignment) : ShapeItem;
}
