namespace Espejismo.Core.RichText;

public sealed partial class Text
{
    private readonly struct ComputedStyle
    {
        public FontFamily Font { get; init; }

        public FontStyle FontStyle { get; init; }

        public ushort FontSize { get; init; }

        public int Leading { get; init; }

        public GlyphStyle Shared { get; init; }
    }
}
